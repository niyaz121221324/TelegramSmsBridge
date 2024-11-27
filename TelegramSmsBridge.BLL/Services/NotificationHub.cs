using Microsoft.AspNetCore.SignalR;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services;

public class NotificationHub : Hub
{
    private readonly static Dictionary<string, string> Users = new Dictionary<string, string>();

    public override Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            Users[Context.ConnectionId] = userId;
        }

        Console.WriteLine($"Пользователь {userId} подключился с ConnectionId: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Users.Remove(Context.ConnectionId);
        Console.WriteLine($"Пользователь отключился: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageAsync(string user, SmsMessage message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", user, message);
    }
}