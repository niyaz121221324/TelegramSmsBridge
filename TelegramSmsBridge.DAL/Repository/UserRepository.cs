using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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
        await _dbContext.SaveChangesAsync();
        return entry.State == EntityState.Added;
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
}