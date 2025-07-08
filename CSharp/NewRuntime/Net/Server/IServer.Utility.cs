using IdGen;
using System.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Utilities;

namespace UselessFrame.Net
{
    public partial interface IServer
    {
        private static IdGenerator _idGenerator;

        public static IServer Create(int port, IFiber fiber)
        {
            if (_idGenerator == null)
            {
                IdGeneratorOptions opt = new IdGeneratorOptions(timeSource: new TimeTicksSource());
                _idGenerator = new IdGenerator(0, opt);
            }
            IServer server = new Server(_idGenerator.CreateId(), port, fiber);
            X.RegisterServer(server);
            return server;
        }

        public static IConnection Connect(IPEndPoint ip, IFiber fiber)
        {
            IConnection connection = new Connection(ip, fiber);
            return connection;
        }

        public static IConnection Connect(int port, IFiber fiber)
        {
            IPEndPoint ip = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            IConnection connection = new Connection(ip, fiber);
            return connection;
        }
    }
}
