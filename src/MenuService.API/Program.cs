using MenuService.Application.Handlers;
using MenuService.Domain.Interfaces;
using MenuService.Infrastructure.Configurations;
using MenuService.Infrastructure.Persistence;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

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
app.UseAuthorization();
app.MapControllers();
app.Run();