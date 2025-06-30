
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Text;
using System.Threading;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using static Google.Protobuf.Reflection.FieldOptions.Types;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class MessageStream
        {
            private bool _writeActive;

            public void SetWriteActive(bool active)
            {
                _writeActive = active;
            }

            public async UniTask<ReadMessageResult> SendWait(IMessage message, bool force)
            {
                if (!force && !_writeActive)
                {
                    X.SystemLog.Debug($"send message refuse {force} {_writeActive}");
                    return default;
                }

                WaitResponseHandle waitHandle = new WaitResponseHandle(message);
                _waitResponseList.TryAdd(waitHandle.Id, waitHandle);
                WriteMessageResult result = await Send(message, force);
                ReadMessageResult response = await waitHandle.ResponseTask;
                return response;
            }

            public async UniTask<WriteMessageResult> Send(IMessage message, bool force)
            {
                if (_setting.ShowSendMessageInfo)
                {
                    X.SystemLog.Debug($"{_connection.GetDebugPrefix(_connection._fsm.Current)}send message start : {message.GetType().Name} {message.GetHashCode()}");
                }
                if (!force && !_writeActive)
                {
                    X.SystemLog.Debug($"send message refuse {force} {_writeActive}");
                    return WriteMessageResult.Create(NetOperateState.Cancel, "[Net]this operate is inactive");
                }

                string typeName = message.Descriptor.FullName;
                int typeNameSize = Encoding.UTF8.GetByteCount(typeName);
                int msgSize = message.CalculateSize() + sizeof(int) + typeNameSize;

                if (!NetUtility.CheckMessageSize(msgSize))
                {
                    X.SystemLog.Debug($"send message is too larget, will refuse, msgSize is {msgSize}");
                    return WriteMessageResult.Create(NetOperateState.Cancel, $"[Net][DENGER]send a large message. will interrupt this operate, msgSize is {msgSize}");
                }

                MessageWriteBuffer buffer = new MessageWriteBuffer(_connection._pool, msgSize);
                BitConverter.TryWriteBytes(buffer.Message, typeNameSize);
                NetUtility.WriteMessageNameTo(typeName, buffer.Message.Slice(sizeof(int), typeNameSize));
                message.WriteTo(buffer.Message.Slice(sizeof(int) + typeNameSize));

                WriteMessageResult result = await AsyncStateUtility.WriteMessageAsync(_connection._client, buffer);
                buffer.Dispose();
                if (_setting.ShowSendMessageInfo)
                {
                    X.SystemLog.Debug($"{_connection.GetDebugPrefix(_connection._fsm.Current)}send message end : {message.GetType().Name} {message.GetHashCode()}");
                }
                return result;
            }
        }
    }

}
