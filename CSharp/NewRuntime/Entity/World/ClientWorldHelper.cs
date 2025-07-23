
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.ECS
{
    public class ClientWorldHelper : IWorldHelper, ICanNet
    {
        private IConnection _connection;

        IConnection ICanNet.Connection => _connection;

        public void OnInit()
        {
            //_connection = IServer.Connect(9999, X.MainFiber);
        }

        public void OnDispose()
        {

        }

        public IEntityHelper CreateHelper()
        {
            return new ClientEntityHelper(X.Net.Connect(8888, X.FiberManager.MainFiber));
        }
    }
}
