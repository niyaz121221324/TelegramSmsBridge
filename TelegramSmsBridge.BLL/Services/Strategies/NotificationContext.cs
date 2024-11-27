using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services.Strategies;

public class NotificationContext
{
    private INotificationStrategy? _notifyStrategy;
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
    public void SetStrategy(INotificationStrategy strategy)
    {
        _notifyStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy), "Strategy cannot be null.");
    }

    /// <summary>
    /// Выполняет стратегию уведомления для отправки сообщения.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    /// <exception cref="InvalidOperationException">Генерируется, если стратегия не установлена.</exception>
    public async Task SendMessageAsync()
    {
        if (_notifyStrategy == null)
        {
            throw new InvalidOperationException("Notification strategy must be set before sending a message.");
        }

        await _notifyStrategy.SendNotificationAsync(_message);
    }
}