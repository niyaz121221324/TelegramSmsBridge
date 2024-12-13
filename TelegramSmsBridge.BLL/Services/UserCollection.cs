using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services;

public class UserCollection
{
    private readonly HashSet<AppUser> _users;
    private static readonly Lazy<UserCollection> _instance = new(() => new UserCollection());

    private UserCollection()
    {
        _users = new HashSet<AppUser>(new TelegramUserNameComparer());
    }

    public static UserCollection Instance => _instance.Value;

    public void AddUser(AppUser user) => _users.Add(user);

    public void RemoveUser(AppUser user) => _users.Remove(user);

    public AppUser? FirstOrDefaultUser(Func<AppUser, bool> predicate) => _users.FirstOrDefault(predicate);
}