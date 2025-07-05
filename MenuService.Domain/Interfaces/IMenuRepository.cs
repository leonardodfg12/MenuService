using MenuService.Domain.Entities;

namespace MenuService.Domain.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<MenuItem>> GetAllAsync();
    Task<MenuItem?> GetByIdAsync(string id);
    Task AddAsync(MenuItem item);
    Task UpdateAsync(MenuItem item);
    Task DeleteAsync(string id);
}