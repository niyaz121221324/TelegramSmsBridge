using MongoDB.Driver;

namespace TelegramSmsBridge.DAL.Repository;

public interface IMongoDbRepository<T> where T : class
{
    Task<T?> FirstOrDefaultAsync(FilterDefinition<T> filter);

    Task AddAsync(T entity);

    Task UpdateAsync(FilterDefinition<T> filter, T entity);

    Task DeleteAsync(FilterDefinition<T> filter);
}