namespace TelegramSmsBridge.BLL.Services.Queries;

public abstract class BaseQuery<T> : IQuery<T> where T : class
{
    public virtual async Task<T?> GetData()
    {
        var cache = await GetFromCache();

        if (cache == null)
        {
            var fromDb = await GetFromDb();
            if (fromDb != null)
            {
                await WriteToCache(fromDb);
            }
            return fromDb;
        }

        return cache;
    }

    protected abstract Task<T?> GetFromCache();

    protected abstract Task<T?> GetFromDb();
    
    protected abstract Task WriteToCache(T data);
}