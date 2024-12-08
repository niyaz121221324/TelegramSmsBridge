using Microsoft.AspNetCore.SignalR;

namespace TelegramSmsBridge.BLL.Services;

public class ConnectionIdUserProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.ConnectionId; 
    }
}