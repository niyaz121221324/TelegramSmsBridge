using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.BLL.Services.Authentification;
using TelegramSmsBridge.BLL.Services.Queries;
using TelegramSmsBridge.DAL.Repository;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.BLL.Services;

namespace TelegramSmsBridge.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IJWTProvider _jwtProvider;
    private readonly ILogger<AuthController> _logger;
    private readonly JWTSettings _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService<User> _cacheService;
    private readonly QueryHandler _queryHandler;

    public AuthController(
        IOptions<JWTSettings> jwtSettings, 
        IJWTProvider jwtProvider, 
        ILogger<AuthController> logger, 
        IUserRepository userRepository, 
        ICacheService<User> cacheService)
    {
        _jwtProvider = jwtProvider;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _queryHandler = new QueryHandler();
    }

    [HttpPost("auth")]
    public async Task<IActionResult> Auth([FromBody] string telegramUserName)
    {
        try
        {
            var user = await _queryHandler.HandleQueryAsync(new GetUserByTelegramQuery(_cacheService, _userRepository, telegramUserName));

            string refreshToken = string.Empty;

            // Если пользователь не зарегистрирован 
            if (user == null)
            {
                refreshToken = _jwtProvider.GenerateRefreshToken();
                await _userRepository.AddUserAsync(new User() { TelegramUserName = telegramUserName, RefreshToken = refreshToken });
            }

            var accessToken = await GenerateJwtToken(telegramUserName);
            refreshToken = string.IsNullOrEmpty(user?.RefreshToken) || IsRefreshTokenExpired(user) 
                ? _jwtProvider.GenerateRefreshToken()
                : refreshToken;

            return Ok(new AuthResponse(accessToken, refreshToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error 500 Internal server exception while auth");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var user = await _queryHandler.HandleQueryAsync(new GetUserByRefreshTokenQuery(_cacheService, _userRepository, refreshToken));

        if (user == null || string.IsNullOrEmpty(user.TelegramUserName) || IsRefreshTokenExpired(user))
        {
            return Unauthorized();
        }

        var accessToken = await GenerateJwtToken(user.TelegramUserName);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        
        // Обновляем refresh токен у конкретного пользователя и обновляем дату
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenLastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);

        return Ok(new AuthResponse(accessToken, newRefreshToken));
    } 

    private bool IsRefreshTokenExpired(User? user)
    {
        if (user == null)
        {
            return false;
        }

        var expirationDate = user.RefreshTokenLastUpdated?.AddDays(_jwtSettings.ExpiryTime);
        return expirationDate < DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private Task<string> GenerateJwtToken(string telegramUserName)
    {
        return Task.FromResult(_jwtProvider.GenerateAccessToken(telegramUserName)); 
    }
}