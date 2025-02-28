using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TelegramSmsBridge.DAL.Contexts;
using TelegramSmsBridge.DAL.Entities;

namespace TelegramSmsBridge.DAL.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<User> _dbSet;

    public UserRepository(AppDbContext dbContext) 
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<User>();
    }

    public async Task<bool> AddUserAsync(User user)
    {
        var entry = await _dbSet.AddAsync(user);
        _dbContext.SaveChanges();
        return entry.State == EntityState.Added;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        User? userToDelete = await _dbSet.FindAsync(id);
        if (userToDelete != null)
        {
            _dbSet.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<User?> FindUserAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> UpdateUserAsync(User userToUpdate, 
        Expression<Func<SetPropertyCalls<User>, SetPropertyCalls<User>>> setPropertyCalls, 
        CancellationToken cancellationToken = default)
    {
        var updatedRow = await _dbSet.ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        return updatedRow > 0;
    }
}