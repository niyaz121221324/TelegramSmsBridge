using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services;

public sealed class UserUpdateCollection
{
    private readonly HashSet<Update> _reciviedUpdates;
    private static readonly Lazy<UserUpdateCollection> _instance = new(() => new UserUpdateCollection());

    private UserUpdateCollection()
    {
        _reciviedUpdates = new HashSet<Update>(new UpdateComparerByUserName());
    }

    public static UserUpdateCollection Instance => _instance.Value;

    public bool AddUpdate(Update update) => _reciviedUpdates.Add(update);

    public bool RemoveUpdate(Update update) => _reciviedUpdates.Remove(update);

    public Update? FirstOrDefaultUpdate(Func<Update, bool> predicate) => _reciviedUpdates.FirstOrDefault(predicate);   
}