using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.API.Controllers;

public class TelegramController : BaseApiController
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramSettings _telegramSettings;

    public TelegramController(ITelegramBotClient botClient, IOptions<TelegramSettings> telegramSettings,
        IConfiguration configuration)
    {
        _botClient = botClient;
        _telegramSettings = telegramSettings.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Telegram bot was started");
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(Update update)
    {
        if (!Request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out StringValues signature) ||
            signature != _telegramSettings.WebhookSecretToken)
        {
            return Unauthorized("Unauthorized Request");
        }

        if (update?.Message == null || string.IsNullOrEmpty(update.Message.Text))
        {
            return BadRequest("Invalid update structure: Message or text is missing.");
        }

        if (update.Type == UpdateType.Message)
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

        await _botClient.SendMessage(message.Chat, responseText, replyMarkup: new ReplyKeyboardRemove());
    }
}