namespace TelegramSmsBridge.DAL.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? TelegramUserName { get; set; }

    public string? RefreshToken { get; set; }

    public DateOnly? RefreshTokenLastUpdated { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
