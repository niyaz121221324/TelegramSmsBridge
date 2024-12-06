using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services.Strategies;

class TextResponseNotificationStrategy : INotificationStrategy
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramHub _telegramHub;

    public TextResponseNotificationStrategy(ITelegramBotClient botClient, TelegramHub telegramHub)
    {
        _botClient = botClient;
        _telegramHub = telegramHub;
    }

    public async Task SendNotificationAsync(Message message)
    {
        if (IsCommandMessage(message))
        {
            await SendCommandMessageAsync(message);
            return;    
        }

        var smsMessage = GetSmsMessageToSend(message);

        if (smsMessage != null)
        {
            await _telegramHub.SendMessageAsync(message.Chat.Id, smsMessage);
        }

        await SendCommandMessageAsync(message);
    }

    private bool IsCommandMessage(Message message)
    {
        return string.IsNullOrEmpty(message.Text) && (message.Text == "/start" || message.Text == "/help");
    } 

    private SmsMessage? GetSmsMessageToSend(Message message)
    {
        if (message.ReplyToMessage != null && message.ReplyToMessage.Contact != null)
        {
            return SmsMessage.FromMessage(message.ReplyToMessage);
        }
        
        if (UserUpdateCollection.Instance.RecentMessagesByChat.TryGetValue(message.Chat.Id, out var recentMessage))
        {
            return recentMessage;
        }

        return null;
    }

    private async Task SendCommandMessageAsync(Message message)
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