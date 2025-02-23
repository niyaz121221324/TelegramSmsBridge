namespace TelegramSmsBridge.BLL.Models;

public class AppUser
{
    public string TelegramUserName { get; set; }
    public string RefreshToken { get; set; }
    public DateOnly RefreshTokenLastUpdated { get; set; }

    public AppUser() : this(string.Empty, string.Empty) 
    {
    }

    public AppUser(string telegramUserName, string refreshToken)
    {
        TelegramUserName = telegramUserName ?? throw new ArgumentNullException(nameof(telegramUserName));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        RefreshTokenLastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}