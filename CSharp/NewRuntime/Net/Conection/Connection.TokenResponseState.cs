
using System;
using System.Diagnostics;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Utilities;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class TokenResponseState : TokenCheck
        {
            public override int State => (int)ConnectionState.TokenResponse;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                Check().Forget();
            }

            private async UniTask Check()
            {
                AsyncBegin();

                _connection.Stream.StartRead();
                bool success = await CheckLatency();
                if (success)
                {
                    ServerTokenRequest tokenRequest = new ServerTokenRequest();
                    await _connection.Stream.Send(tokenRequest, true);
                }

                AsyncEnd();
            }

            private async UniTask<bool> CheckLatency()
            {
                int tryTimes = 10;
                Dictionary<long, int> timeList = new Dictionary<long, int>(tryTimes);
                long value = 0;
                int maxTimes = 0;

                for (int i = 0; i < tryTimes; i++)
                {
                    long time = DateTime.UtcNow.Ticks;
                    TestServerTimeMessage msg = NetPoolUtility.CreateMessage<TestServerTimeMessage>();
                    ReadMessageResult messageResult = await _connection.Stream.SendWait(msg, true);
                    switch (messageResult.State)
                    {
                        case NetOperateState.OK:
                            {
                                TestServerTimeResponseMessage rspMsg = (TestServerTimeResponseMessage)messageResult.Message;
                                long nowTime = DateTime.UtcNow.Ticks;
                                long millisecond = (long)(rspMsg.Time - (time + nowTime) / 2d);
                                if (!timeList.TryGetValue(millisecond, out int count))
                                    count = 0;
                                count++;
                                timeList[millisecond] = count;
                                if (count > maxTimes)
                                {
                                    maxTimes = count;
                                    value = millisecond;
                                }
                                break;
                            }

                        default:
                            {
                                ChangeState<DisposeState>().Forget();
                                CancelAllAsyncWait();
                                return false;
                            }
                    }
                }

                if (maxTimes > 1)
                {
                    _connection._serverTimeGap = value;
                }
                else
                {
                    List<long> keys = new List<long>(timeList.Keys);
                    keys.Sort(SortUtility.LongDownToUp);
                    _connection._serverTimeGap = keys[keys.Count / 2];
                }
                X.SystemLog.Debug($"{DebugPrefix}check server time gap is {_connection._serverTimeGap}");

                return true;
            }

            private async UniTask<bool> SuccessHandler(MessageResult result)
            {
                ServerToken token = (ServerToken)result.Message;
                _connection._id = token.Id;
                X.SystemLog.Debug($"{DebugPrefix}receive server token {_connection._id}");
                bool success = await result.Response(new ServerTokenVerify() { ResponseToken = token.RequestToken });
                if (success)
                {
                    ChangeState<RunState>().Forget();
                }
                return success;
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            if (responseHandle.HasResponse)
                            {
                                responseHandle.SetResponse(messageResult);
                                return true;
                            }
                            else
                            {
                                MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                                if (result.RequireResponse && result.MessageType == typeof(ServerToken))
                                {
                                    await SuccessHandler(result);
                                    return false;
                                }
                                return true;
                            }
                        }

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}token response happend socket error, {messageResult.Exception.ErrorCode}");
                            X.SystemLog.Exception(messageResult.Exception);
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<CloseResponseState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    default:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }
                }
            }
        }
    }
}
