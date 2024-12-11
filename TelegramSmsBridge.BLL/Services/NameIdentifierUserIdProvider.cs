using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace TelegramSmsBridge.BLL.Services;

public class NameIdentifierUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
        }

        var user = connection.User;
        if (user?.Identity is not { IsAuthenticated: true })
        {
            return null;
        }

        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}