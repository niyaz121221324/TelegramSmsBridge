using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services.Strategies;

public class NotificationContext
{
    private INotifyStrategy? _notifyStrategy;
    private readonly Message _message;

    /// <summary>
    /// Инициализирует контекст уведомлений с указанным сообщением.
    /// </summary>
    /// <param name="message">Сообщение, которое будет использовано для уведомления.</param>
    public NotificationContext(Message message)
    {
        _message = message ?? throw new ArgumentNullException(nameof(message), "Message cannot be null.");
    }

    /// <summary>
    /// Устанавливает стратегию уведомления, которая будет использоваться.
    /// </summary>
    /// <param name="strategy">Стратегия уведомления.</param>
    public void SetStrategy(INotifyStrategy strategy)
    {
        _notifyStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy), "Strategy cannot be null.");
    }

    /// <summary>
    /// Executes the notification strategy to send the message.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the strategy is not set.</exception>
    public async Task SendMessageAsync()
    {
        if (_notifyStrategy == null)
        {
            throw new InvalidOperationException("Notification strategy must be set before sending a message.");
        }

        await _notifyStrategy.SendNotificationAsync(_message);
    }
}