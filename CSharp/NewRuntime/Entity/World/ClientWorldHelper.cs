
using UselessFrame.Net;
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Worlds
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
            return new ClientEntityHelper(IServer.Connect(8888, X.MainFiber));
        }
    }
}
