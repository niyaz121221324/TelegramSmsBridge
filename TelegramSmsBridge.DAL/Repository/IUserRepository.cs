using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using TelegramSmsBridge.DAL.Entities;

namespace TelegramSmsBridge.DAL.Repository;

public interface IUserRepository
{
    Task<User?> FindUserAsync(int id);

    Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate);

    Task<bool> DeleteUserAsync(int id);

    Task<bool> AddUserAsync(User user);

    Task<bool> UpdateUserAsync(User userToUpdate, Expression<Func<SetPropertyCalls<User>, SetPropertyCalls<User>>> setPropertyCalls, 
        CancellationToken cancellationToken = default);        
}