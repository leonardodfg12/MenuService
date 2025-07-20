using MassTransit;
using MenuService.Application.Commands;
using MenuService.Application.Contracts;
using MenuService.Domain.Entities;
using MenuService.Domain.Interfaces;

namespace MenuService.Application.Handlers;

public class MenuServiceHandler(IMenuRepository repository, ISendEndpointProvider sendEndpointProvider)
{
    private const string MenuItemCreatedEvent = "menu-item-created";
    
    public async Task Handle(CreateMenuItemCommand command)
    {
        var item = new MenuItem(command.Name, command.Description, command.Price, command.IsAvailable);
        await repository.AddAsync(item);
        
        var menuItemCreatedEvent = new MenuItemCreatedEvent
        {
            Id = item.Id,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            IsAvailable = command.IsAvailable
        };

        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{MenuItemCreatedEvent}"));
        await endpoint.Send(menuItemCreatedEvent);
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync() =>
        await repository.GetAllAsync();

    public async Task<MenuItem?> GetByIdAsync(string id) =>
        await repository.GetByIdAsync(id);

    public async Task UpdateAsync(string id, CreateMenuItemCommand command)
    {
        var item = await repository.GetByIdAsync(id);
        if (item is null) return;

        item.Update(command.Name, command.Description, command.Price, command.IsAvailable);
        await repository.UpdateAsync(item);
    }

    public async Task DeleteAsync(string id) =>
        await repository.DeleteAsync(id);
}