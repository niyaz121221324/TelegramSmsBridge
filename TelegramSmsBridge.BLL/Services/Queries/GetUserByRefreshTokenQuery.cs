using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetUserByRefreshTokenQuery : BaseQuery<User>
{
    private readonly ICacheService<User> _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly string _refreshToken;

    public GetUserByRefreshTokenQuery(ICacheService<User> cacheService, IUserRepository userRepository, string refreshToken)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _refreshToken = refreshToken;
    }

    protected override async Task<User?> GetFromCache()
    {
        return await Task.FromResult(_cacheService.Get(_refreshToken));
    }

    protected override async Task<User?> GetFromDb()
    {
        return await _userRepository.FirstOrDefaultAsync(user => user.RefreshToken == _refreshToken);
    }

    protected override async Task WriteToCache(User data)
    {
        await Task.Run(() => _cacheService.Add(_refreshToken, data));
    }
}