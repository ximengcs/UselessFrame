
using System.Net;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    public partial interface IServer
    {
        public static IServer Create(int port, IFiber fiber)
        {
            return new Server(port, fiber);
        }

        public static IConnection Connect(IPEndPoint ip, IFiber fiber)
        {
            return new Connection(ip, fiber);
        }
    }
}
