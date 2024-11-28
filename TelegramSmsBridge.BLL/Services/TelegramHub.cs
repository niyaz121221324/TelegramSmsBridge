using Microsoft.AspNetCore.SignalR;
using Telegram.Bot;

namespace TelegramSmsBridge.BLL.Services;

public class TelegramHub : Hub
{
    private readonly ITelegramBotClient _botClient;

    private readonly static Dictionary<string, long> Users = new Dictionary<string, long>();

    public TelegramHub(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            Users[Context.ConnectionId] = 0;
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
}