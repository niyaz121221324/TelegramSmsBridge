using Telegram.Bot.Types;
using TelegramSmsBridge.BLL.Models;

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

    /// <summary>
    /// Получает или задает словарь, который хранит последние SMS-сообщения для каждого чата.
    /// Ключ представляет идентификатор чата, а значение — соответствующее <see cref="SmsMessage"/>.
    /// </summary>
    public Dictionary<long, SmsMessage> RecentMessagesByChat { get; set; } = new Dictionary<long, SmsMessage>();

    public bool AddUpdate(Update update) => _reciviedUpdates.Add(update);

    public bool RemoveUpdate(Update update) => _reciviedUpdates.Remove(update);

    public Update? FirstOrDefaultUpdate(Func<Update, bool> predicate) => _reciviedUpdates.FirstOrDefault(predicate);   
}