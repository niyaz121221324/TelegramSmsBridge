using MongoDB.Driver;
using TelegramSmsBridge.DAL.Entities;

namespace TelegramSmsBridge.DAL.Repository;

public class SmsMessageRepository : BaseMongoDbRepository<SmsMessage>
{
    public SmsMessageRepository(IMongoDatabase database) : base(database, "sms_messages_collection")
    {
    }
}