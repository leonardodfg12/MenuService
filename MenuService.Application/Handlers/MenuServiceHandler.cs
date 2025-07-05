using MenuService.Application.Commands;
using MenuService.Domain.Entities;
using MenuService.Domain.Interfaces;

namespace MenuService.Application.Handlers;

public class MenuServiceHandler
{
    private readonly IMenuRepository _repository;

    public MenuServiceHandler(IMenuRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CreateMenuItemCommand command)
    {
        var item = new MenuItem(command.Name, command.Description, command.Price, command.IsAvailable);
        await _repository.AddAsync(item);
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync() =>
        await _repository.GetAllAsync();

    public async Task<MenuItem?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    public async Task UpdateAsync(string id, CreateMenuItemCommand command)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return;

        item.Update(command.Name, command.Description, command.Price, command.IsAvailable);
        await _repository.UpdateAsync(item);
    }

    public async Task DeleteAsync(string id) =>
        await _repository.DeleteAsync(id);
}