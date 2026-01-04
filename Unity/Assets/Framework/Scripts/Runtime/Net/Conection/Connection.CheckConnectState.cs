using System;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class CheckConnectState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.CheckConnect;

            private int _tryTimes;
            private bool _remoteTest;
            private bool _localTest;
            private UniTaskCompletionSource _remoteTestTaskSource;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                _tryTimes = 3;
                _remoteTest = false;
                _localTest = false;
                _remoteTestTaskSource = new UniTaskCompletionSource();
                RetryHandler();
            }

            private async UniTask SuccessHandler()
            {
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}check success");
                await _remoteTestTaskSource.Task;
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}wait remote check complete");
                ChangeState<TokenCheck>().Forget();
            }

            private void FailureHandler()
            {
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}check failure");
                ChangeState<DisposeState>().Forget();
            }

            private void RetryHandler()
            {
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}try check connect, times {_tryTimes}");
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
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}try check step1");
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
                    X.Log.Exception(e);
                    FailureHandler();
                }
                catch (ObjectDisposedException e)
                {
                    X.Log.Exception(e);
                    FailureHandler();
                }
                catch (SocketException e)
                {
                    X.Log.Error($"{DebugPrefix}try check step1 socket error, {e.ErrorCode}");
                    X.Log.Exception(e);
                    RetryHandler();
                }
                catch (Exception e)
                {
                    X.Log.Exception(e);
                    FailureHandler();
                }
            }

            private async UniTask CheckStep2()
            {
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}try check step2");
                AsyncBegin();

                TestConnect testMessage = new TestConnect()
                {
                    Time = DateTime.Now.Ticks
                };
                _connection._stream.StartRead();
                ReadMessageResult result = await _connection._stream.SendWait(testMessage, true);
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}try check step2 complete, {result.State}");

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            SuccessHandler().Forget();
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
                            X.Log.Error($"{DebugPrefix}try check step2 socket error, {result.Exception.ErrorCode}");
                            X.Log.Exception(result.Exception);
                            switch (result.Exception.SocketErrorCode)
                            {
                                case SocketError.Success:
                                case SocketError.WouldBlock:
                                    {
                                        SuccessHandler().Forget();
                                    }
                                    break;

                                default:
                                    {
                                        RetryHandler();
                                    }
                                    break;
                            }
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

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                            X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}receive message {result.MessageType.Name}");
                            if (result.MessageType == typeof(TestConnect))
                            {
                                TestConnect msg = (TestConnect)result.Message;
                                bool success = await result.Response(new TestConnectResponse());
                                if (!success)
                                {
                                    ChangeState<DisposeState>().Forget();
                                    CancelAllAsyncWait();
                                    return false;
                                }
                                else
                                {
                                    _remoteTest = true;
                                    _remoteTestTaskSource.TrySetResult();
                                }
                                if (_localTest)
                                    return false;
                                else
                                    return true;
                            }
                            else if (result.MessageType == typeof(TestConnectResponse))
                            {
                                responseHandle.SetResponse(messageResult);
                                _localTest = true;
                                if (_remoteTest)
                                    return false;
                                else
                                    return true;
                            }
                            else
                            {
                                responseHandle.SetCancel();
                            }

                            return true;
                        }

                    case NetOperateState.SocketError:
                        {
                            X.Log.Error($"{DebugPrefix}connect socket error, {messageResult.Exception.ErrorCode}");
                            X.Log.Exception(messageResult.Exception);
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
