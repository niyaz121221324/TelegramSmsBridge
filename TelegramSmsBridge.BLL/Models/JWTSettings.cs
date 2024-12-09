namespace TelegramSmsBridge.BLL.Models;

public class JWTSettings
{
    public string? Key { get; set; }

    public int ExpiryTime { get; set; }

    public string? Issuer { get; set; }
    
    public string? Audience { get; set; }
}