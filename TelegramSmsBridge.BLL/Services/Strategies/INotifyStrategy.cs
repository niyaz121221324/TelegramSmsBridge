using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services.Strategies;

public interface INotifyStrategy
{
    Task SendNotification(Message message);
}