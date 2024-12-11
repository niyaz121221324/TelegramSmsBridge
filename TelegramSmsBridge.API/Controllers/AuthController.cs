using Microsoft.AspNetCore.Mvc;
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
            var token = await GenerateJwtToken(telegramUserName);

            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error 500 Internal server exception while auth");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    } 

    private Task<string> GenerateJwtToken(string telegramUserName)
    {
        return Task.FromResult(_jwtProvider.GenerateAccesstoken(telegramUserName)); 
    }
}