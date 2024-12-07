using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services;

public class TelegramHub : Hub
{
    private static readonly ConcurrentDictionary<long, string> Users = new ConcurrentDictionary<long, string>();

    // Регистрирует пользователя по его userName пользователя в telegram, сохраняя его connection ID.
    public async Task RegisterUserAsync(string userName)
    {
        var connectionId = Context.ConnectionId;

        if (string.IsNullOrEmpty(connectionId))
        {
            throw new InvalidOperationException("Connection ID cannot be null or empty.");
        }

        long chatId = await GetChatIdAsync(userName);

        if (chatId == 0)
        {
            throw new KeyNotFoundException($"No chat found for user: {userName}");
        }

        // Добавьте или обновите пользователя в словаре (потокобезопасно)
        Users[chatId] = connectionId;
        Console.WriteLine($"User {userName} with chatId {chatId} registered.");
    }

    // Отправляет сообщение пользователю, используя его ChatId.
    public async Task SendMessageAsync(long chatId, SmsMessage message)
    {
        if (!Users.TryGetValue(chatId, out var connectionId) || string.IsNullOrEmpty(connectionId))
        {
            Console.WriteLine($"No connection found for chatId: {chatId}");
            return; 
        }

        await Clients.User(connectionId).SendAsync("ReceiveMessage", message);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var chatId = Users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

        if (chatId != 0)
        {
            Users.TryRemove(chatId, out _); 
            Console.WriteLine($"User with connectionId {Context.ConnectionId} disconnected.");
        }
        else
        {
            Console.WriteLine($"Failed to find user for connectionId {Context.ConnectionId} on disconnect.");
        }

        return base.OnDisconnectedAsync(exception);
    }

    // Получает ChatId для данного имени пользователя, получая последние обновления.
    private Task<long> GetChatIdAsync(string userName)
    {
        // Попробуйте найти ChatId для указанного имени пользователя в последних обновлениях.
        var update = UserUpdateCollection.Instance
            .FirstOrDefaultUpdate(u => u?.Message?.From?.FirstName?.Equals(userName, StringComparison.OrdinalIgnoreCase) == true);

        return Task.FromResult(update?.Message?.Chat?.Id ?? 0);
    }
}