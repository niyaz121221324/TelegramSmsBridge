using Microsoft.Extensions.Caching.Memory;

namespace TelegramSmsBridge.BLL.Services;

public class CacheService<T> : ICacheService<T> where T : class
{
    private readonly IMemoryCache _memoryCache;
    
    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Add(string key, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
        var cacheKey = $"{nameof(T)}:{key}";

        _memoryCache.Set(cacheKey, value, cacheEntryOptions);
    }

    public T? Get(string key)
    {
        var cacheKey = $"{nameof(T)}:{key}";
        return _memoryCache.TryGetValue(cacheKey, out T? value) ? value : default;
    }

    public void Update(string key, T newValue)
    {
        var cacheKey = $"{nameof(T)}:{key}";
        if (_memoryCache.TryGetValue(cacheKey, out _))
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(cacheKey, newValue, cacheEntryOptions);
        }
    }

    public void Remove(string key)
    {
        var cacheKey = $"{nameof(T)}:{key}";
        _memoryCache.Remove(cacheKey);
    }
}