
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Worlds
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
