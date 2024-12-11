namespace TelegramSmsBridge.BLL.Services.Authentification;

public interface IJWTProvider
{
    string GenerateAccesstoken(string telegramUserName);

    string GenerateRefreshToken();
}