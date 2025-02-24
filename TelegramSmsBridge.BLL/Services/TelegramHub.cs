using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services;

[Authorize]
public class TelegramHub : Hub
{
    // Отправляет сообщение определённому зарегестрированному пользователю
    public async Task SendMessageAsync(string user, SmsMessage message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", message);
    }
}