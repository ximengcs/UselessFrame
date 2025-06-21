
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

            public override void OnEnter(NetFsmState<Server> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                TryListen().Forget();
            }

            private async UniTask TryListen()
            {
                X.SystemLog.Debug($"{DebugPrefix}ready accept");
                AcceptConnectResult result = await AsyncStateUtility.AcceptConnectAsync(_connection._listener, _connection._fiber);
                X.SystemLog.Debug($"{DebugPrefix}find new client, result state : {result.State} ");
                switch (result.State)
                {
                    case NetOperateState.OK:
                        Connection connection = new Connection(_connection, Guid.NewGuid(), result.Client, _connection._fiber);
                        _connection.AddConnection(connection);
                        X.SystemLog.Debug($"{DebugPrefix}add new client, id : {connection.Id}, ip : {connection.RemoteIP}");
                        TryListen().Forget();
                        break;

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}try listen client happend socket error, {result.Exception.ErrorCode}");
                            X.SystemLog.Exception(result.Exception);
                            ChangeState<DisposeState>().Forget();
                        }
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
