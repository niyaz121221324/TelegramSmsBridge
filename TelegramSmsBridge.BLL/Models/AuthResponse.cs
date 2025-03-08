namespace TelegramSmsBridge.BLL.Models;

public class AuthResponse
{
    public string AccessToken { get; }
    
    public string RefreshToken { get; }

    public AuthResponse() : this(string.Empty, string.Empty) 
    {
    }

    public AuthResponse(string accessToken, string refreshToken)
    {
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
    }
}