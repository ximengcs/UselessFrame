using System;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal class VerifyTokenState : ConnectionState
        {
            private Guid _token;
            private int _tryTimes;

            public override void OnInit()
            {
                base.OnInit();
                _token = Guid.Empty;
            }

            public override void OnEnter(ConnectionState preState)
            {
                base.OnEnter(preState);
                _tryTimes = 3;
                if (_token != Guid.Empty)
                {
                    ChangeState<RunState>().Forget();
                }
                else
                {
                    Verify().Forget();
                }
            }

            private async UniTask Verify()
            {
                AsyncBegin();

                ServerToken token = NetUtility.CreateToken(_connection._id);
                _token = new Guid(token.Id.Span);
                X.SystemLog.Debug("Net", $"create new client {_connection._id} {_token}");
                _connection._stream.StartRead();
                ReadMessageResult result = await _connection._stream.SendWait(token, true);

                if (_waitCancel)
                {
                    AsyncEnd();
                    return;
                }

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            ServerTokenVerify tokenVerify = result.Message as ServerTokenVerify;
                            if (tokenVerify != null)
                            {
                                X.SystemLog.Debug("Net", $"verify success {_connection._id} {new Guid(token.Id.Span)}");
                                ChangeState<RunState>().Forget();
                            }
                            else
                            {
                                X.SystemLog.Debug("Net", $"tokenVerify error -> {_connection._client}");
                                if (_tryTimes > 0)
                                {
                                    Verify().Forget();
                                }
                                else
                                {
                                    ChangeState<CloseState>().Forget();
                                }
                            }
                        }
                        break;

                    case NetOperateState.FatalError:
                        ChangeState<CloseState>().Forget();
                        break;

                    case NetOperateState.SocketError:

                        break;
                }

                AsyncEnd();
            }
        }
    }
}
