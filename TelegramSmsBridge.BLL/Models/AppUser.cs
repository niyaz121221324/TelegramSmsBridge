namespace TelegramSmsBridge.BLL.Models;

public class AppUser
{
    public string TelegramUserName { get; set; } = string.Empty;

    public string RefreshToken { get; set;} = string.Empty;

    public AppUser()
    {
    }

    public AppUser(string telegramUserName, string refreshToken)
    {
        TelegramUserName = telegramUserName;
        RefreshToken = refreshToken;
    }
}