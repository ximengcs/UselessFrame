﻿
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class RunState : NetFsmState<Connection>
        {
            private MessageBeat _beat;

            public override int State => (int)ConnectionState.Run;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                if (_beat == null)
                {
                    _beat = new MessageBeat(_connection._runFiber, _connection._stream, 60);
                    _beat.ErrorEvent += OnErrorHandler;
                }

                _connection._stream.StartRead();
                _connection._stream.SetWriteActive(true);
                _beat.Start();
            }

            public override void OnExit()
            {
                base.OnExit();
                if (_beat != null)
                {
                    _beat.Stop();
                }
            }

            public override void OnDispose()
            {
                base.OnDispose();
                if (_beat != null)
                {
                    _beat.Dispose();
                    _beat = null;
                }
            }

            public override async UniTask OnSendMessage(IMessage message)
            {
                WriteMessageResult result = await _connection.Stream.Send(message, false);
                switch (result.State)
                {
                    case NetOperateState.OK:
                        break;

                    default:
                        break;
                }
            }

            public override async UniTask<MessageResult> OnSendWaitMessage(IMessage message)
            {
                ReadMessageResult messageResult = await _connection.Stream.SendWait(message, false);
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                            return result;
                        }

                    default:
                        {
                            return default;
                        }
                }
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
                                if (_messageHandler.TryGetValue(result.MessageType, out var handler))
                                {
                                    return handler(result);
                                }
                                else
                                {
                                    _connection.TriggerNewMessage(result);
                                    return true;
                                }
                            }
                        }

                    case NetOperateState.SocketError:
                        {
                            X.Log.Error($"{DebugPrefix}receive message happend socket error, {messageResult.Exception.ErrorCode}");
                            X.Log.Exception(messageResult.Exception);
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    default:
                        {
                            X.Log.Debug($"{DebugPrefix}receive message error, {messageResult.State} {messageResult.StateMessage}");
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }
                }
            }

            private void OnErrorHandler(WriteMessageResult result)
            {
                switch (result.State)
                {
                    case NetOperateState.SocketError:
                        {
                            X.Log.Error($"{DebugPrefix}send keepalive happend socket error, {result.Exception.ErrorCode}");
                            X.Log.Exception(result.Exception);
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            break;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            break;
                        }

                    default:
                        {
                            X.Log.Debug($"{DebugPrefix}send keepalive message error, {result.State} {result.StateMessage}");

                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            break;
                        }
                }
            }
        }
    }
}
