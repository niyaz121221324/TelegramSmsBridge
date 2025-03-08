namespace TelegramSmsBridge.BLL.Services;

public interface ICacheService<T> where T : class
{
    void Add(string key, T value);

    T? Get(string key);

    void Remove(string key);

    void Update(string key, T newValue);
}