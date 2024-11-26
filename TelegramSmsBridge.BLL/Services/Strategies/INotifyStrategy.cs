using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services.Strategies;

public interface INotifyStrategy
{
    Task SendNotificationAsync(Message message);
}