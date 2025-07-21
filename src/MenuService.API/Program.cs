using System.Diagnostics.CodeAnalysis;
using System.Text;
using MassTransit;
using MenuService.Application.Handlers;
using MenuService.Domain.Interfaces;
using MenuService.Infrastructure.Configurations;
using MenuService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace MenuService.API;

[ExcludeFromCodeCoverage]
public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var jwtKey = builder.Configuration["JWT_KEY"];
        var jwtIssuer = builder.Configuration["JWT_ISSUER"];
        var jwtAudience = builder.Configuration["JWT_AUDIENCE"];
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey!);
        builder.Services.AddSwaggerGen(options =>
        {
            // Swagger JWT auth support
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Informe o token JWT no campo abaixo: Bearer {token}",
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });
        builder.Services.AddAuthorization();

        builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection("MongoDbSettings"));

        builder.Services.AddMassTransit(
            cfg =>
            {
                cfg.UsingRabbitMq((context, config) =>
                {
                    config.Host(builder.Configuration["RABBITMQ_HOST"], h =>
                    {
                        h.Username(builder.Configuration["RABBITMQ_USERNAME"]!);
                        h.Password(builder.Configuration["RABBITMQ_PASSWORD"]!);
                    });
                });
            });

        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = builder.Configuration["MONGO_CONNECTION_STRING"];
            return new MongoClient(connectionString);
        });

        builder.Services.AddScoped(sp =>
        {
            var databaseName = builder.Configuration["MONGO_DATABASE_NAME"];
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        builder.Services.AddScoped<IMenuRepository, MongoMenuRepository>();
        builder.Services.AddScoped<MenuServiceHandler>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}