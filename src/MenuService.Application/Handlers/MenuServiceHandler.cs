using MenuService.Application.Commands;
using MenuService.Domain.Entities;
using MenuService.Domain.Interfaces;

namespace MenuService.Application.Handlers;

public class MenuServiceHandler(IMenuRepository repository)
{
    public async Task Handle(CreateMenuItemCommand command)
    {
        var item = new MenuItem(command.Name, command.Description, command.Price, command.IsAvailable);
        await repository.AddAsync(item);
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