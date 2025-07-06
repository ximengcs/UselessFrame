
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
            private bool _listening;

            public override int State => (int)ServerState.Listen;

            public override void OnEnter(NetFsmState<Server> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                _listening = false;
                TryListen().Forget();
            }

            private async UniTask TryListen()
            {
                if (_listening)
                    return;
                _listening = true;

                while (_listening)
                {
                    X.SystemLog.Debug($"{DebugPrefix}ready accept");
                    AcceptConnectResult result = await AsyncStateUtility.AcceptConnectAsync(_connection._listener);
                    X.SystemLog.Debug($"{DebugPrefix}find new client, result state : {result.State} ");
                    switch (result.State)
                    {
                        case NetOperateState.OK:
                            {
                                Connection connection = new Connection(_connection, Guid.NewGuid(), result.Client, _connection._fiber);
                                _connection.AddConnection(connection);
                                X.SystemLog.Debug($"{DebugPrefix}add new client, id : {connection.Id}, ip : {connection.RemoteIP}");
                                _listening = true;
                            }
                            break;

                        case NetOperateState.SocketError:
                            {
                                X.SystemLog.Error($"{DebugPrefix}try listen client happend socket error, {result.Exception.ErrorCode}");
                                X.SystemLog.Exception(result.Exception);
                                ChangeState<DisposeState>().Forget();
                                _listening = false;
                            }
                            break;

                        case NetOperateState.FatalError:
                            ChangeState<DisposeState>().Forget();
                            _listening = false;
                            break;

                        default:
                            ChangeState<DisposeState>().Forget();
                            _listening = false;
                            break;
                    }
                }
            }
        }
    }
}
