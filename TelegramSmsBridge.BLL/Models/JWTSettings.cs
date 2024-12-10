namespace TelegramSmsBridge.BLL.Models;

public class JWTSettings
{
    public int ExpiryTime { get; set; }
    
    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;
    
    public string Audience { get; set; } = string.Empty;
}