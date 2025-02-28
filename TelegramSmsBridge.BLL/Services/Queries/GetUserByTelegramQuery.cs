using Microsoft.Extensions.Caching.Memory;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetUserByTelegramQuery : BaseQuery<User>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IUserRepository _userRepository;
    private readonly string _telegramUserName;

    public GetUserByTelegramQuery(IMemoryCache memoryCache, IUserRepository userRepository, string telegramUserName)
    {
        _memoryCache = memoryCache;
        _userRepository = userRepository;
        _telegramUserName = telegramUserName;
    }

    public override async Task<User?> GetFromCache()
    {
        return await Task.FromResult((User?)_memoryCache.Get(_telegramUserName));
    }

    public override async Task<User?> GetFromDb()
    {
        return await _userRepository.FirstOrDefaultAsync(user => user.TelegramUserName == _telegramUserName);
    }

    public override async Task WriteToCache(User data)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
        await Task.FromResult(_memoryCache.Set(_telegramUserName, data, cacheEntryOptions));
    }
}