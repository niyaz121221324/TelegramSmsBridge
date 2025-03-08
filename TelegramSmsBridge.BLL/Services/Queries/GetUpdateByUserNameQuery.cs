using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using Telegram.Bot.Types;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetUpdateByUserNameQuery : BaseQuery<Update>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMongoDbRepository<Update> _repository;
    private readonly string _userName;

    public GetUpdateByUserNameQuery(IMemoryCache memoryCache, IMongoDbRepository<Update> repository, string userName)
    {
        _memoryCache = memoryCache;
        _repository = repository;
        _userName = userName;
    }

    protected override async Task<Update?> GetFromCache()
    {
        return await Task.FromResult((Update?)_memoryCache.Get(_userName));
    }

    protected override async Task<Update?> GetFromDb()
    {
        var filter = Builders<Update>.Filter.Eq(u => u.Message.From.Username, _userName);
        return await _repository.FirstOrDefaultAsync(filter);
    }

    protected override async Task WriteToCache(Update data)
    {
        await Task.Run(() => _memoryCache.Set(data.Message.From.Username, data));
    }
}