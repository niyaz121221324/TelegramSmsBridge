using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Commands;

public class AddOrUpdateSmsMessageCommand : BaseAddOrUpdateCommand<SmsMessage>
{
    private readonly IMongoDbRepository<SmsMessage> _repository;
    private readonly IMemoryCache _memoryCache;

    public AddOrUpdateSmsMessageCommand(IMongoDbRepository<SmsMessage> repository, IMemoryCache memoryCache, SmsMessage entity) 
        : base(entity)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    protected override async Task AddToDb(SmsMessage entity)
    {
        await _repository.AddAsync(entity);
    }

    protected override async Task<bool> ExistsInDb()
    {
        var filter = Builders<SmsMessage>.Filter.Eq(e => e.ChatId, Entity.ChatId);
        return await _repository.AnyAsync(filter);
    }

    protected override async Task<bool> IsCached()
    {
        var isCached = _memoryCache.TryGetValue(Entity.ChatId.ToString(), out _);
        return await Task.FromResult(isCached);
    }

    protected override async Task UpdateCache(SmsMessage entity)
    {
        await Task.FromResult(_memoryCache.Set(entity.ChatId, entity)); 
    }

    protected override async Task UpdateInDb(SmsMessage entity)
    {
        var filter = Builders<SmsMessage>.Filter.Eq(e => e.ChatId, entity.ChatId);
        await _repository.UpdateAsync(filter, entity);
    }
}