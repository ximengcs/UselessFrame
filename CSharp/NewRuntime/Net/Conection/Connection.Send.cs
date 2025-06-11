using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Text;
using UselessFrame.Net;
using UselessFrame.NewRuntime;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private ConcurrentDictionary<Guid, WaitResponseHandle> _waitResponseList = new ConcurrentDictionary<Guid, WaitResponseHandle>();

        public async UniTask<T> SendWait<T>(IMessage message) where T : class, IMessage
        {
            if (message == null)
            {
                X.SystemLog.Debug($"{Id} SendWait message is null");
                return default;
            }

            WaitResponseHandle waitHandle = new WaitResponseHandle(message);
            _waitResponseList.TryAdd(waitHandle.Id, waitHandle);
            await Send(message);
            if (_closeTokenSource.IsCancellationRequested)
            {
                waitHandle.Dispose();
                return default;
            }

            IMessage response = await waitHandle.ResponseTask;
            if (response == null)
                return default;
            return response as T;
        }

        public async UniTask Send(IMessage message)
        {
            if (message == null)
            {
                X.SystemLog.Debug($"{Id} Send message is null");
                return;
            }

            string typeName = message.Descriptor.FullName;
            int typeNameSize = Encoding.UTF8.GetByteCount(typeName);
            int msgSize = message.CalculateSize() + sizeof(int) + typeNameSize;

            MessageWriteBuffer buffer = new MessageWriteBuffer(_pool, msgSize);
            BitConverter.TryWriteBytes(buffer.Message, typeNameSize);
            Encoding.UTF8.GetBytes(typeName, buffer.Message.Slice(sizeof(int), typeNameSize));
            message.WriteTo(buffer.Message.Slice(sizeof(int) + typeNameSize));
            await Send(buffer);
        }

        private async UniTask Send(MessageWriteBuffer buffer)
        {
            _sendTimes++;
            WriteMessageResult result = await MessageUtility.WriteMessageAsync(_client, buffer, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;

            switch (_state.Value)
            {
                case ConnectionState.TokenPending:
                case ConnectionState.Normal:
                    switch (result.State)
                    {
                        case NetOperateState.OK:
                            buffer.Dispose();
                            break;

                        case NetOperateState.Disconnect:
                        case NetOperateState.DataError:
                        case NetOperateState.ParamError:
                        case NetOperateState.PermissionError:
                        case NetOperateState.RemoteClose:
                        case NetOperateState.Unknown:
                            X.SystemLog.Debug("Net", $" {Id} send message fatal error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.FatalErrorClose;
                            InnerClose();
                            break;

                        case NetOperateState.SocketError:
                            X.SystemLog.Debug("Net", $" {Id} send message socket error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.SocketError;
                            InnerClose();
                            break;

                        default:
                            X.SystemLog.Debug("Net", $" {Id} send message unkown error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.FatalErrorClose;
                            InnerClose();
                            break;
                    }
                    break;
            }
        }

        private void SendTokenVerify(ByteString token)
        {
            ServerTokenVerify verify = new ServerTokenVerify()
            {
                ResponseToken = token
            };
            X.SystemLog.Debug("Net", $" send token verify {new Guid(token.Span)}, {_ip.Address}:{_ip.Port}");
            Send(verify).Forget();
        }
    }
}
