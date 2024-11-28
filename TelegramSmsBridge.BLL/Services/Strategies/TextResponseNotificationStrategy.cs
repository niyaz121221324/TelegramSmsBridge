using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services.Strategies;

class TextResponseNotificationStrategy : INotificationStrategy
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramHub _telegramHub;
    private readonly Message? _messageToRespondTo;

    public TextResponseNotificationStrategy(ITelegramBotClient botClient, TelegramHub telegramHub, Message? messageResponseTo)
    {
        _botClient = botClient;
        _telegramHub = telegramHub;
        _messageToRespondTo = messageResponseTo;
    }

    public async Task SendNotificationAsync(Message message)
    {
        var smsMessage = GetSmsMessageToSend(message);

        if (smsMessage != null)
        {
            await _telegramHub.SendMessageAsync(message.Chat.Id, smsMessage);
        }

        await SendCommandMessageAsync(message);
    }
   
    private SmsMessage? GetSmsMessageToSend(Message message)
    {
        if (message.ReplyToMessage != null && message.ReplyToMessage.Contact != null)
        {
            return CreateSmsMessageFrom(message.ReplyToMessage);
        }

        if (_messageToRespondTo != null && string.IsNullOrEmpty(_messageToRespondTo.Text) && _messageToRespondTo.Contact != null)
        {
            return CreateSmsMessageFrom(_messageToRespondTo);
        }

        return null;
    }

    private SmsMessage CreateSmsMessageFrom(Message message)
    {
        var phoneNumber = GetContact(message);
        var messageContent = message.Text ?? string.Empty;

        return new SmsMessage
        {
            PhoneNumber = phoneNumber,
            MessageContent = messageContent
        }; 
    }

    private string GetContact(Message? message)
    {
        return message != null && message.Contact != null ? message.Contact.PhoneNumber : string.Empty;
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