namespace TelegramSmsBridge.BLL.Services.Commands;

public abstract class BaseAddOrUpdateCommand<T> : ICommand where T : class
{
    protected readonly T Entity;

    public BaseAddOrUpdateCommand(T entity)
    {
        Entity = entity;
    }

    public async Task ExecuteAsync()
    {
        await AddOrUpdate();
    }

    protected virtual async Task AddOrUpdate()
    {
        if (await IsCached())
        {
            await UpdateCache(Entity);
        }

        if (await ExistsInDb())
        {
            await UpdateInDb(Entity);
            return;
        }

        await AddToDb(Entity);
    }

    protected abstract Task<bool> IsCached();

    protected abstract Task UpdateCache(T entity);

    protected abstract Task<bool> ExistsInDb();

    protected abstract Task UpdateInDb(T entity);

    protected abstract Task AddToDb(T entity);
}