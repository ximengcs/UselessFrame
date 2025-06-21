using Cysharp.Threading.Tasks;
using System;
using System.Data.Common;
using System.Net.Sockets;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class CheckConnectState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.CheckConnect;

            private int _tryTimes;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                _tryTimes = 3;
                RetryHandler();
            }

            private void SuccessHandler(MessageResult tokenMessage)
            {
                X.SystemLog.Debug($"{DebugPrefix}check success");
                ChangeState<TokenCheck>(tokenMessage).Forget();
            }

            private void FailureHandler()
            {
                X.SystemLog.Debug($"{DebugPrefix}check failure");
                ChangeState<DisposeState>().Forget();
            }

            private void RetryHandler()
            {
                X.SystemLog.Debug($"{DebugPrefix}try check connect, times {_tryTimes}");
                if (_tryTimes > 0)
                {
                    _tryTimes--;
                    CheckStep1();
                }
                else
                {
                    FailureHandler();
                }
            }

            private void CheckStep1()
            {
                X.SystemLog.Debug($"{DebugPrefix}try check step1");
                try
                {
                    Socket socket = _connection._client.Client;
                    // 检查读状态，0超时表示立即返回
                    // 如果连接关闭或重置，Poll会返回true且Available为0
                    bool failure = socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0;
                    if (failure)
                        RetryHandler();
                    else
                        CheckStep2().Forget();
                }
                catch (NotSupportedException e)
                {
                    FailureHandler();
                }
                catch (ObjectDisposedException e)
                {
                    FailureHandler();
                }
                catch (SocketException e)
                {
                    CheckSocketError(e);
                }
                catch (Exception e)
                {
                    FailureHandler();
                }
            }

            private async UniTask CheckStep2()
            {
                X.SystemLog.Debug($"{DebugPrefix}try check step2");
                AsyncBegin();

                // 发送0字节数据测试连接
                byte[] dummy = new byte[0];
                TestConnect testMessage = new TestConnect();
                _connection._stream.StartRead();
                ReadMessageResult result = await _connection._stream.SendWait(testMessage, true);
                X.SystemLog.Debug($"{DebugPrefix}try check step2 complete, {result.State}");

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            SuccessHandler(null);
                        }
                        break;

                    case NetOperateState.Cancel:
                        break;

                    case NetOperateState.Timeout:
                        {
                            RetryHandler();
                        }
                        break;

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}try check step2 socket error, {result.Exception.ErrorCode}");
                            X.SystemLog.Exception(result.Exception);
                            CheckSocketError(result.Exception);
                        }
                        break;

                    default:
                        {
                            FailureHandler();
                        }
                        break;
                }

                AsyncEnd();
            }

            private void CheckSocketError(SocketException e)
            {
                X.SystemLog.Debug($"{DebugPrefix}trigger socket error, {e.SocketErrorCode}");
                switch (e.SocketErrorCode)
                {
                    case SocketError.Success:
                    case SocketError.WouldBlock:
                        {
                            SuccessHandler(null);
                        }
                        break;

                    default:
                        {
                            RetryHandler();
                        }
                        break;
                }
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = new MessageResult(messageResult.Message, _connection);
                            X.SystemLog.Debug($"{DebugPrefix}receive message {result.MessageType.Name}");
                            if (result.MessageType == typeof(TestConnect))
                            {
                                bool success = await result.Response(new TestConnectResponse());
                                if (!success)
                                    return false;
                            }
                            else if (result.MessageType == typeof(TestConnectResponse))
                            {
                                responseHandle.SetResponse(messageResult);
                            }
                            else if (result.MessageType == typeof(ServerToken))
                            {
                                SuccessHandler(result);
                                CancelAllAsyncWait();
                                return false;
                            }
                            else
                            {
                                if (responseHandle.HasResponse)
                                    responseHandle.SetResponse(messageResult);
                            }
                            return true;
                        }

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}connect socket error, {messageResult.Exception.ErrorCode}");
                            X.SystemLog.Exception(messageResult.Exception);
                            ChangeState<DisposeState>().Forget();
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
