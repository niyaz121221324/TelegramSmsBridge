using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramSmsBridge.API.Controllers;

public class TelegramWebHookController : BaseApiController
{
    private readonly ITelegramBotClient _botClient;

    public TelegramWebHookController(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
}