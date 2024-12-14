namespace TelegramSmsBridge.BLL.Services.Authentification;

public interface IJWTProvider
{
    string GenerateAccessToken(string telegramUserName);

    string GenerateRefreshToken();
}