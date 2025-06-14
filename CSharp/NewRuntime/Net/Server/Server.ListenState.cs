
using System;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class ListenState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Listen;

            public override void OnEnter(NetFsmState<Server> preState)
            {
                base.OnEnter(preState);
                TryListen().Forget();
            }

            private async UniTask TryListen()
            {
                X.SystemLog.Debug("Net", $"ready accept {_connection._host}");
                AcceptConnectResult result = await AsyncStateUtility.AcceptConnectAsync(_connection._listener);
                X.SystemLog.Debug("Net", $"find new client, result : {result.State}, server : {_connection._host} {result.State} ");
                switch (result.State)
                {
                    case NetOperateState.OK:
                        _connection.AddConnection(new Connection(Guid.NewGuid(), result.Client, _connection._fiber));
                        TryListen().Forget();
                        break;

                    case NetOperateState.SocketError:
                        ChangeState<DisposeState>().Forget();
                        break;

                    case NetOperateState.FatalError:
                        ChangeState<DisposeState>().Forget();
                        break;

                    default:
                        ChangeState<DisposeState>().Forget();
                        break;
                }
            }
        }
    }
}
