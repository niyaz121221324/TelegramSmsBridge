using System.Linq.Expressions;
using TelegramSmsBridge.DAL.Entities;

namespace TelegramSmsBridge.DAL.Repository;

public interface IUserRepository
{
    Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate);

    Task<bool> AddUserAsync(User user);      
}