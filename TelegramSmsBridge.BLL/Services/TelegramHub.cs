using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services;

[Authorize]
public class TelegramHub : Hub
{
    // Отправляет сообщение пользователю, используя его ChatId.
    public async Task SendMessageAsync(string user, SmsMessage message)
    {
        if (UserCollection.Instance.FirstOrDefaultUser(u => u.TelegramUserName == user) == null)
        {
            throw new InvalidOperationException($"Unauthorized user {user}");
        }

        await Clients.User(user).SendAsync("ReceiveMessage", message);
    }
}