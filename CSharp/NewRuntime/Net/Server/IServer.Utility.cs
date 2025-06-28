using System.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    public partial interface IServer
    {
        public static IServer Create(int port, IFiber fiber)
        {
            IServer server = new Server(port, fiber);
            X.RegisterServer(server);
            return server;
        }

        public static IConnection Connect(IPEndPoint ip, IFiber fiber)
        {
            IConnection connection = new Connection(ip, fiber);
            X.RegisterConnection(connection);
            return connection;
        }

        public static IConnection Connect(int port, IFiber fiber)
        {
            IPEndPoint ip = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            IConnection connection = new Connection(ip, fiber);
            X.RegisterConnection(connection);
            return connection;
        }
    }
}
