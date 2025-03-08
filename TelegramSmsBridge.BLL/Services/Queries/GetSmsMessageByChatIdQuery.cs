using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetSmsMessageByChatIdQuery : BaseQuery<SmsMessage>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMongoDbRepository<SmsMessage> _repository;
    private readonly long _chatId;

    public GetSmsMessageByChatIdQuery(IMemoryCache memoryCache, IMongoDbRepository<SmsMessage> repository, long chatId)
    {
        _memoryCache = memoryCache;
        _repository = repository;
        _chatId = chatId;
    }

    protected override async Task<SmsMessage?> GetFromCache()
    {
        return await Task.FromResult((SmsMessage?)_memoryCache.Get(_chatId));
    }

    protected override async Task<SmsMessage?> GetFromDb()
    {
        var filter = Builders<SmsMessage>.Filter.Eq(m => m.ChatId, _chatId);
        return await _repository.FirstOrDefaultAsync(filter);
    }

    protected override async Task WriteToCache(SmsMessage data)
    {
        await Task.Run(() => _memoryCache.Set(data.ChatId ,data));
    }
}