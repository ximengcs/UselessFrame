
namespace UselessFrame.NewRuntime.Events
{
    internal class DefaultEvent : XEvent
    {
        public static DefaultEvent Create(int eventId)
        {
            DefaultEvent evt = X.Pool.Require<DefaultEvent>();
            evt.Id = eventId;
            return evt;
        }
    }
}
