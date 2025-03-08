using MongoDB.Driver;
using Telegram.Bot.Types;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Queries;

public class GetUpdateByUserNameQuery : BaseQuery<Update>
{
    private readonly ICacheService<Update> _cacheService;
    private readonly IMongoDbRepository<Update> _repository;
    private readonly string _userName;

    public GetUpdateByUserNameQuery(ICacheService<Update> cacheService, IMongoDbRepository<Update> repository, string userName)
    {
        _cacheService = cacheService;
        _repository = repository;
        _userName = userName;
    }

    protected override async Task<Update?> GetFromCache()
    {
        return await Task.FromResult(_cacheService.Get(_userName));
    }

    protected override async Task<Update?> GetFromDb()
    {
        var filter = Builders<Update>.Filter.Eq(u => u.Message.From.Username, _userName);
        return await _repository.FirstOrDefaultAsync(filter);
    }

    protected override async Task WriteToCache(Update data)
    {
        await Task.Run(() => _cacheService.Add(data.Message.From.Username, data));
    }
}