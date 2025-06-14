
using System.Net;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public interface IFiber
    {
        int ThreadId { get; }

        void Post(SendOrPostCallback d, object state);

        void Dispose();
    }
}
