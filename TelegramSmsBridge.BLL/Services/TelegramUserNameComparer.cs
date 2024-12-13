using System.Diagnostics.CodeAnalysis;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services;

public class TelegramUserNameComparer : IEqualityComparer<AppUser>
{
    public bool Equals(AppUser? x, AppUser? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if ((x == null && y != null) || (y == null && x != null))
        {
            return false;
        }

        return x?.TelegramUserName == y?.TelegramUserName;
    }

    public int GetHashCode([DisallowNull] AppUser obj)
    {
        if (obj == null)
        {
            return 0;
        }

        return obj.TelegramUserName.GetHashCode();
    }
}