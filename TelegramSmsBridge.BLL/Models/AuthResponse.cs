namespace TelegramSmsBridge.BLL.Models;

public class AuthResponse
{
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set;}

    public AuthResponse() : this(string.Empty, string.Empty) 
    {
    }

    public AuthResponse(string accessToken, string refreshToken)
    {
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
    }
}