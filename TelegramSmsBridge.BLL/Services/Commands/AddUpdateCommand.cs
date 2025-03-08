using MongoDB.Driver;
using Telegram.Bot.Types;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.BLL.Services.Commands;

public class AddUpdateCommand : ICommand
{
    private readonly IMongoDbRepository<Update> _repository;
    private readonly Update _update;

    public AddUpdateCommand(IMongoDbRepository<Update> repository, Update update) 
    {
        _repository = repository;
        _update = update;
    }

    public async Task ExecuteAsync()
    {
        var filter = Builders<Update>.Filter.Or(
            Builders<Update>.Filter.Eq(u => u.Message.Chat.Id, _update.Message?.Chat?.Id),
            Builders<Update>.Filter.Eq(u => u.Message.Chat.Username, _update.Message?.Chat.Username)
        );

        var exists = await _repository.AnyAsync(filter);

        if (exists)
        {
            return;
        }

        await _repository.AddAsync(_update);
    }
}