
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.ECS
{
    public class ServerWorldHelper : IWorldHelper
    {
        private IServer _server;

        public void OnInit()
        {
            //_server = IServer.Create(9999, X.MainFiber);
            //_server.Start();
        }

        public IEntityHelper CreateHelper()
        {
            ServerEntityHelper helper = new ServerEntityHelper();
            helper.Start();
            return helper;
        }

        public void OnDispose()
        {
        }
    }
}
