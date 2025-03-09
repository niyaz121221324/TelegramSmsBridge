using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramSmsBridge.BLL.Services.Queries;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Strategies;

class TextResponseNotificationStrategy : INotificationStrategy
{
    private readonly ICacheService<SmsMessage> _cacheService;
    private readonly IMongoDbRepository<SmsMessage> _smsMessageRepository;
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramHub _telegramHub;
    private readonly QueryHandler _queryHandler;

    public TextResponseNotificationStrategy(
        ICacheService<SmsMessage> cacheService, 
        IMongoDbRepository<SmsMessage> smsMessageRepository, 
        ITelegramBotClient botClient, 
        TelegramHub telegramHub)
    {
        _cacheService = cacheService;
        _smsMessageRepository = smsMessageRepository;
        _botClient = botClient;
        _telegramHub = telegramHub;
        _queryHandler = new QueryHandler();
    }

    public async Task SendNotificationAsync(Message message)
    {
        if (IsCommandMessage(message))
        {
            await SendCommandMessageAsync(message);
            return;    
        }

        var smsMessage = await GetSmsMessageToSend(message);
        
        if (smsMessage != null && !string.IsNullOrEmpty(message?.From?.Username))
        {
            await _telegramHub.SendMessageAsync(message.From.Username, smsMessage);
        }
        else if (message != null)
        {
            await SendCommandMessageAsync(message);
        }
    }

    private bool IsCommandMessage(Message message)
    {
        return !string.IsNullOrEmpty(message.Text) && (message.Text == "/start" || message.Text == "/help");
    } 

    private async Task SendCommandMessageAsync(Message message)
    {
        var responseText = message.Text?.Split(' ')[0].Trim() switch
        {
            "/start" => "Welcome to the bot!",
            "/help" => "How can I help you?",
            _ => "Unknown command."
        };

        await _botClient.SendMessage(message.Chat, responseText, replyMarkup: new ReplyKeyboardRemove());
    }

    private async Task<SmsMessage?> GetSmsMessageToSend(Message message)
    {
        if (message.ReplyToMessage != null)
        {
            return new SmsMessage(message);
        }

        var recentMessage = await _queryHandler.HandleQueryAsync(new GetSmsMessageByChatIdQuery(
            _cacheService, 
            _smsMessageRepository, 
            message.Chat.Id
        ));
        
        if (recentMessage != null)
        {
            return recentMessage;
        }

        return null;
    }
}