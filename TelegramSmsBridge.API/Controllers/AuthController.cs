using Microsoft.AspNetCore.Mvc;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.BLL.Services;
using TelegramSmsBridge.BLL.Services.Authentification;

namespace TelegramSmsBridge.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IJWTProvider _jwtProvider;
    private readonly ILogger _logger;

    public AuthController(IJWTProvider jwtProvider, ILogger logger)
    {
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    [HttpPost("auth")]
    public async Task<IActionResult> Auth([FromBody] string telegramUserName)
    {
        try
        {
            var user = UserCollection.Instance.FirstOrDefaultUser(u => u.TelegramUserName == telegramUserName);

            string refreshToken = string.Empty;

            // Если пользователь не зарегестрирован 
            if (user == null)
            {
                refreshToken = _jwtProvider.GenerateRefreshToken();
                UserCollection.Instance.AddUser(new AppUser(telegramUserName, refreshToken));
            }

            var accesToken = await GenerateJwtToken(telegramUserName);
            refreshToken = user?.RefreshToken ?? refreshToken;

            return Ok(new { accesToken, refreshToken });
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

        if (user == null)
        {
            return Unauthorized();
        }

        var accesToken = await GenerateJwtToken(user.TelegramUserName);

        return Ok(new { accesToken, refreshToken });
    } 

    private Task<string> GenerateJwtToken(string telegramUserName)
    {
        return Task.FromResult(_jwtProvider.GenerateAccesstoken(telegramUserName)); 
    }
}