using MongoDB.Driver;

namespace TelegramSmsBridge.DAL.Repository;

public abstract class BaseMongoDbRepository<T> : IMongoDbRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public BaseMongoDbRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }
    
    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task<bool> AnyAsync(FilterDefinition<T> filter)
    {
        return await _collection.Find(filter).AnyAsync();
    }

    public async Task DeleteAsync(FilterDefinition<T> filter)
    {
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<T?> FirstOrDefaultAsync(FilterDefinition<T> filter)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(FilterDefinition<T> filter, T entity)
    {
        await _collection.ReplaceOneAsync(filter, entity);
    }
}