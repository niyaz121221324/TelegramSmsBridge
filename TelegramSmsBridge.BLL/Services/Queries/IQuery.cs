namespace TelegramSmsBridge.BLL.Services.Queries;

public interface IQuery<T> where T : class
{
    Task<T?> GetData();
}