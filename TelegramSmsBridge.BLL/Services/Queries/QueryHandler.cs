namespace TelegramSmsBridge.BLL.Services.Queries;

public class QueryHandler
{
    public async Task<T?> HandleQueryAsync<T>(IQuery<T> query) where T : class
    {
        return await query.GetData();
    }
}