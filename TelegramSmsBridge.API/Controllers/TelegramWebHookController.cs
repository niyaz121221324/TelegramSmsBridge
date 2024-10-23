using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramSmsBridge.API.Controllers;

public class TelegramWebHookController : BaseApiController
{
    private readonly ITelegramBotClient _botClient;

    public TelegramWebHookController(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] Update update)
    {
        Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out StringValues signature);

        if (string.IsNullOrEmpty(signature))
        {
            return Unauthorized("Unauthorized Request");
        }

        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;

            if (message.Text != null)
            {
                switch (message.Text.Split(' ')[0])
                {
                    case "/start":
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "Welcome to the bot!");
                        break;
                    case "/help":
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "How can I help you?");
                        break;
                    default:
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "Unknown command.");
                        break;
                }
            }
        }

        return Ok();
    }
}