
namespace UselessFrame.NewRuntime.Events
{
    public interface IEventManager
    {
        IEventDispatcher Create();

        void Remove(IEventDispatcher evtDispatcher);
    }
}
