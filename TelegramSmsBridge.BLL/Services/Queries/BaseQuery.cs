namespace TelegramSmsBridge.BLL.Services.Queries;

public abstract class BaseQuery<T>
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

    public abstract Task<T?> GetFromCache();

    public abstract Task<T?> GetFromDb();
    
    public abstract Task WriteToCache(T data);
}