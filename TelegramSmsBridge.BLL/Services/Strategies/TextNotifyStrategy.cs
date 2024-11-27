using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramSmsBridge.BLL.Services.Strategies;

class TextNotifyStrategy : INotifyStrategy
{
    private readonly ITelegramBotClient _botClient;

    public TextNotifyStrategy(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendNotificationAsync(Message message)
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