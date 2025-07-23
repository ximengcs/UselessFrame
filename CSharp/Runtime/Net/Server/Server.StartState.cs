
using Cysharp.Threading.Tasks;
using System;
using System.Net.Sockets;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class StartState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Start;

            public override void OnEnter(NetFsmState<Server> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                TryStart();
            }

            private void TryStart()
            {
                X.Log.Debug($"{DebugPrefix}try start");
                try
                {
                    _connection._listener.Start();
                    ChangeState<ListenState>().Forget();
                }
                catch (SocketException e)
                {
                    X.Log.Error($"{DebugPrefix}try start socket error {e.SocketErrorCode}");
                    X.Log.Exception(e);
                    ChangeState<DisposeState>().Forget();
                }
            }
        }
    }
}
