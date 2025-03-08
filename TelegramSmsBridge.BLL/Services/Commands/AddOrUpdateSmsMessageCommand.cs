using MongoDB.Driver;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Commands;

public class AddOrUpdateSmsMessageCommand : BaseAddOrUpdateCommand<SmsMessage>
{
    private readonly IMongoDbRepository<SmsMessage> _repository;
    private readonly ICacheService<SmsMessage> _cacheService;

    public AddOrUpdateSmsMessageCommand(IMongoDbRepository<SmsMessage> repository, ICacheService<SmsMessage> cacheService,
        SmsMessage entity) : base(entity)
    {
        _repository = repository;
        _cacheService = cacheService;
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
        var isCached = _cacheService.Get(Entity.ChatId.ToString()) != null;
        return await Task.FromResult(isCached);
    }

    protected override async Task UpdateCache(SmsMessage entity)
    {
        await Task.Run(() => _cacheService.Update(entity.ChatId.ToString(), entity)); 
    }

    protected override async Task UpdateInDb(SmsMessage entity)
    {
        var filter = Builders<SmsMessage>.Filter.Eq(e => e.ChatId, entity.ChatId);
        await _repository.UpdateAsync(filter, entity);
    }
}