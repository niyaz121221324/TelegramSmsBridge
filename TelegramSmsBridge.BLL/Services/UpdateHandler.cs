using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramSmsBridge.BLL.Services.Strategies;

namespace TelegramSmsBridge.BLL.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> _logger;
    private readonly TelegramHub _telegramHub;
    private Message? messageToRespondTo;

    public UpdateHandler(ILogger<UpdateHandler> logger, TelegramHub telegramHub)
    {
        _logger = logger;
        _telegramHub = telegramHub;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);

        // Время восстановления в случае ошибки сетевого подключения
        if (exception is RequestException)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsBotMessage(update))
        {
            messageToRespondTo = update.Message;
            return;
        }

        switch (update)
        {
            case { Message: { } message }:
                await OnMessage(message, botClient);
                break;

            default:
                await UnknownUpdateHandlerAsync(update);
                break;
        }
    }

    private bool IsBotMessage(Update update)
    {
        return update.Message != null && update.Message.From != null && update.Message.From.IsBot;
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private async Task OnMessage(Message message, ITelegramBotClient botClient)
    {
        NotificationContext context = new NotificationContext(message);

        if (message.Type == MessageType.Text)
        {
            context.SetStrategy(new TextResponseNotificationStrategy(botClient, _telegramHub, messageToRespondTo));
        }
    
        await context.SendMessageAsync();
    }
}