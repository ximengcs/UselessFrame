using System;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class CheckConnectState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.CheckConnect;

            private int _tryTimes;

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
                _tryTimes = 3;
                RetryHandler();
            }

            private void SuccessHandler()
            {
                ChangeState<RunState>().Forget();
            }

            private void FailureHandler()
            {
                ChangeState<DisposeState>().Forget();
            }

            private void RetryHandler()
            {
                if (_tryTimes > 0)
                {
                    CheckStep1();
                    _tryTimes--;
                }
                else
                {
                    FailureHandler();
                }
            }

            private void CheckStep1()
            {
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
                AsyncBegin();

                // 发送0字节数据测试连接
                byte[] dummy = new byte[0];
                TestConnect testMessage = new TestConnect();
                ReadMessageResult result = await _connection._stream.SendWait(testMessage, true);

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            SuccessHandler();
                        }
                        break;

                    case NetOperateState.Cancel:
                        break;

                    case NetOperateState.SocketError:
                        {
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
                switch (e.SocketErrorCode)
                {
                    case SocketError.Success:
                    case SocketError.WouldBlock:
                        {
                            SuccessHandler();
                        }
                        break;

                    default:
                        {
                            RetryHandler();
                        }
                        break;
                }
            }
        }
    }
}
