using MongoDB.Driver;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetSmsMessageByChatIdQuery : BaseQuery<SmsMessage>
{
    private readonly ICacheService<SmsMessage> _chaceService;
    private readonly IMongoDbRepository<SmsMessage> _repository;
    private readonly long _chatId;

    public GetSmsMessageByChatIdQuery(ICacheService<SmsMessage> chaceService, IMongoDbRepository<SmsMessage> repository, long chatId)
    {
        _chaceService = chaceService;
        _repository = repository;
        _chatId = chatId;
    }

    protected override async Task<SmsMessage?> GetFromCache()
    {
        return await Task.FromResult(_chaceService.Get(_chatId.ToString()));
    }

    protected override async Task<SmsMessage?> GetFromDb()
    {
        var filter = Builders<SmsMessage>.Filter.Eq(m => m.ChatId, _chatId);
        return await _repository.FirstOrDefaultAsync(filter);
    }

    protected override async Task WriteToCache(SmsMessage data)
    {
        await Task.Run(() => _chaceService.Add(data.ChatId.ToString(), data));
    }
}