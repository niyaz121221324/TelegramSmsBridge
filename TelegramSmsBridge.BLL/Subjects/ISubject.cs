using TelegramSmsBridge.BLL.Observers;

namespace TelegramSmsBridge.BLL.Subjects
{
    public interface ISubject
    {
        void Attach(IObserver observer);

        void Detach(IObserver observer);

        void Notify();
    }
}
