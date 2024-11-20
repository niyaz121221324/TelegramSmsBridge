using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.BLL.Services;

namespace TelegramSmsBridge.API.Controllers;

public class TelegramController : BaseApiController
{
    private readonly TelegramSettings _telegramSettings;
    private readonly ILogger<UpdateHandler> _logger;

    public TelegramController(IOptions<TelegramSettings> telegramSettings, ILogger<UpdateHandler> logger)
    {
        _telegramSettings = telegramSettings.Value;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Telegram bot was started");
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] Update update,
        [FromServices] ITelegramBotClient bot,
        [FromServices] UpdateHandler handleUpdateService,
        CancellationToken ct)
    {
        if (!IsValidSecretToken(Request.Headers["X-Telegram-Bot-Api-Secret-Token"]))
        {
            return Forbid();
        }

        return await ProcessUpdateAsync(bot, update, handleUpdateService, ct);
    }

    private bool IsValidSecretToken(string? providedToken)
    {
        return providedToken == _telegramSettings.WebhookSecretToken;
    }

    private async Task<IActionResult> ProcessUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        UpdateHandler handleUpdateService,
        CancellationToken ct)
    {
        try
        {
            await handleUpdateService.HandleUpdateAsync(bot, update, ct);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error 500 Internal server exception");

            await handleUpdateService.HandleErrorAsync(bot, ex, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}