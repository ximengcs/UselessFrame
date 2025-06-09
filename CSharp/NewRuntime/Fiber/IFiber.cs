
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public interface IFiber
    {
        void Post(SendOrPostCallback d, object state);
    }
}
