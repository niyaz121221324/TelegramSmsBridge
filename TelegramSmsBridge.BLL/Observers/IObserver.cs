using TelegramSmsBridge.BLL.Subjects;

namespace TelegramSmsBridge.BLL.Observers
{
    public interface IObserver
    {
        void Update(ISubject subject);
    }
}
