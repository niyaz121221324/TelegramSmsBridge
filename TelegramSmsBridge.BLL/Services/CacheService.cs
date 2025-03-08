using Microsoft.Extensions.Caching.Memory;

namespace TelegramSmsBridge.BLL.Services;

public class CacheService<T> : ICacheService<T> where T : class
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
    }

    private static string GetCacheKey(string key)
    {
        return $"{typeof(T).Name}:{key}";
    } 

    public void Add(string key, T value) 
    {
        _memoryCache.Set(GetCacheKey(key), value, _cacheOptions);
    }

    public T? Get(string key) 
    { 
        return _memoryCache.TryGetValue(GetCacheKey(key), out T? value) ? value : default; 
    }

    public void Update(string key, T newValue)
    {
        var cacheKey = GetCacheKey(key);
        if (_memoryCache.TryGetValue(cacheKey, out _))
        {
            _memoryCache.Set(cacheKey, newValue, _cacheOptions);
        }
    }

    public void Remove(string key) 
    { 
        _memoryCache.Remove(GetCacheKey(key)); 
    }
}