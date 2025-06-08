using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Text;
using UselessFrame.Net;
using UselessFrame.NewRuntime;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        public async UniTask Send(IMessage message)
        {
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
            WriteMessageResult result = await MessageUtility.WriteMessageAsync(_client, buffer, _closeTokenSource.Token);
            if (_state.Value == ConnectionState.Normal)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        buffer.Dispose();
                        break;

                    case NetOperateState.NormalClose:
                        _state.Value = ConnectionState.NormalClose;
                        _closeTokenSource.Cancel();
                        Dispose();
                        break;

                    case NetOperateState.Disconnect:
                    case NetOperateState.DataError:
                    case NetOperateState.ParamError:
                    case NetOperateState.PermissionError:
                    case NetOperateState.Unknown:
                        _state.Value = ConnectionState.FatalErrorClose;
                        X.SystemLog.Error("Net", $"send message error {result.State} {result.StateMessage}");
                        _closeTokenSource.Cancel();
                        Dispose();
                        break;

                    case NetOperateState.SocketError:
                        _state.Value = ConnectionState.SocketError;
                        break;
                }
            }
        }
    }
}
