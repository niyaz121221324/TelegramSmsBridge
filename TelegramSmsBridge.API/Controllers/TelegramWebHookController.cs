using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramSmsBridge.API.Controllers;

public class TelegramWebHookController : BaseApiController
{
    private readonly ITelegramBotClient _botClient;
    private readonly string _webhookSecret;

    public TelegramWebHookController(ITelegramBotClient botClient, IConfiguration configuration)
    {
        _botClient = botClient;
        _webhookSecret = configuration["TelegramBotSettings:SecretToken"] ?? string.Empty;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] Update update)
    {
        if (!Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out StringValues signature) ||
            signature != _webhookSecret)
        {
            return Unauthorized("Unauthorized Request");
        }

        if (update.Type == UpdateType.Message && HasMessageText(update.Message))
        {
            await HandleMessageAsync(update.Message!);
        }

        return Ok();
    }

    private async Task HandleMessageAsync(Message message)
    {
        var responseText = message.Text?.Split(' ')[0] switch
        {
            "/start" => "Welcome to the bot!",
            "/help" => "How can I help you?",
            _ => "Unknown command."
        };

        await _botClient.SendTextMessageAsync(message.Chat.Id, responseText);
    }

    private static bool HasMessageText(Message? message) => message?.Text != null;
}