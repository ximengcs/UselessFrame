
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class StartState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Start;

            public override void OnEnter(NetFsmState<Server> preState)
            {
                base.OnEnter(preState);
                TryStart();
            }

            private void TryStart()
            {
                try
                {
                    _connection._listener.Start();
                    X.SystemLog.Debug("Net", $"server start");
                    ChangeState<ListenState>().Forget();
                }
                catch (SocketException e)
                {
                    X.SystemLog.Debug("Net", $"server start error");
                    ChangeState<DisposeState>().Forget();
                }
            }
        }
    }
}
