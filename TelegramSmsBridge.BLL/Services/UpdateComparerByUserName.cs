using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Services;

public class UpdateComparerByUserName : IEqualityComparer<Update>
{
    public bool Equals(Update? x, Update? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if ((x == null && y != null) || (y == null && x != null))
        {
            return false;
        }

        return x?.Message?.Chat?.Username == y?.Message?.Chat?.Username; 
    }

    public int GetHashCode([DisallowNull] Update obj)
    {
        if (obj == null)
        {
            return 0;
        }

        return HashCode.Combine(obj?.Message?.Chat?.Username, obj?.Message?.Chat?.Id);
    }
}