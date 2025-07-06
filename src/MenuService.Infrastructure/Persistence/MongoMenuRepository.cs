using MenuService.Domain.Entities;
using MenuService.Domain.Interfaces;
using MongoDB.Driver;

namespace MenuService.Infrastructure.Persistence;

public class MongoMenuRepository(IMongoDatabase database) : IMenuRepository
{
    private readonly IMongoCollection<MenuItem> _collection = database.GetCollection<MenuItem>("MenuItems");

    public async Task<IEnumerable<MenuItem>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<MenuItem?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task AddAsync(MenuItem item) =>
        await _collection.InsertOneAsync(item);

    public async Task UpdateAsync(MenuItem item) =>
        await _collection.ReplaceOneAsync(x => x.Id == item.Id, item);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}