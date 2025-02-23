using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.BLL.Services;
using TelegramSmsBridge.BLL.Services.Authentification;

namespace TelegramSmsBridge.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IJWTProvider _jwtProvider;
    private readonly ILogger<AuthController> _logger;
    private readonly JWTSettings _jwtSettings;

    public AuthController(IOptions<JWTSettings> jwtSettings, IJWTProvider jwtProvider, ILogger<AuthController> logger)
    {
        _jwtProvider = jwtProvider;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("auth")]
    public async Task<IActionResult> Auth([FromBody] string telegramUserName)
    {
        try
        {
            var user = UserCollection.Instance.FirstOrDefaultUser(u => u.TelegramUserName == telegramUserName);

            string refreshToken = string.Empty;

            // Если пользователь не зарегистрирован 
            if (user == null)
            {
                refreshToken = _jwtProvider.GenerateRefreshToken();
                UserCollection.Instance.AddUser(new AppUser(telegramUserName, refreshToken));
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
        var user = UserCollection.Instance.FirstOrDefaultUser(u => u.RefreshToken == refreshToken);

        if (user == null || IsRefreshTokenExpired(user))
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

    private bool IsRefreshTokenExpired(AppUser? user)
    {
        if (user == null)
        {
            return false;
        }

        var expirationDate = user.RefreshTokenLastUpdated.AddDays(_jwtSettings.ExpiryTime);
        return expirationDate < DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private Task<string> GenerateJwtToken(string telegramUserName)
    {
        return Task.FromResult(_jwtProvider.GenerateAccessToken(telegramUserName)); 
    }
}