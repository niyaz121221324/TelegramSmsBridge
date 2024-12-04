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
    private readonly Queue<Update> _receivedUpdates = new Queue<Update>();

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

    [HttpGet("getChatId")]
    public async Task<ActionResult<long>> GetChatId(string userName)
    {
        if (_receivedUpdates.Count == 0)
        {
            return NotFound("No received updates available");
        }

        var chatId = await GetChatIdForUserAsync(userName);

        if (!chatId.HasValue)
        {
            return NotFound($"No chat found for user: {userName}");
        }

        return Ok(chatId);
    }

    private Task<long?> GetChatIdForUserAsync(string userName)
    {
        var update = _receivedUpdates.FirstOrDefault(u => u?.Message?.Chat.Username == userName);

        return Task.FromResult(update?.Message?.Chat.Id);
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
        
        _receivedUpdates.Enqueue(update);
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