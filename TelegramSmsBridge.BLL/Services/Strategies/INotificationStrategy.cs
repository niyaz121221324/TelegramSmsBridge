using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services.Strategies;

public interface INotificationStrategy
{
    Task SendNotificationAsync(Message message);
}