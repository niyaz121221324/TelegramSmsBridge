using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetUserByTelegramQuery : BaseQuery<User>
{
    private readonly ICacheService<User> _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly string _telegramUserName;

    public GetUserByTelegramQuery(ICacheService<User> cacheService, IUserRepository userRepository, string telegramUserName)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _telegramUserName = telegramUserName;
    }

    protected override async Task<User?> GetFromCache()
    {
        return await Task.FromResult(_cacheService.Get(_telegramUserName));
    }

    protected override async Task<User?> GetFromDb()
    {
        return await _userRepository.FirstOrDefaultAsync(user => user.TelegramUserName == _telegramUserName);
    }

    protected override async Task WriteToCache(User data)
    {
        await Task.Run(() => _cacheService.Add(_telegramUserName, data));
    }
}