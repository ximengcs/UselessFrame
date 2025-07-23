
using System.Net;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.NewRuntime.Net
{
    public interface INetManager
    {
        IServer Create(int port, IFiber fiber);

        IConnection Connect(IPEndPoint ip, IFiber fiber);

        IConnection Connect(int port, IFiber fiber);
    }
}
