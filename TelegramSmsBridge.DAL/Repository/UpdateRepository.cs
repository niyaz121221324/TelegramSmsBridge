using MongoDB.Driver;
using Telegram.Bot.Types;

namespace TelegramSmsBridge.DAL.Repository;

public class UpdateRepository : BaseMongoDbRepository<Update>
{
    public UpdateRepository(IMongoDatabase database) : base(database, "updates_collection")
    {
    }
}