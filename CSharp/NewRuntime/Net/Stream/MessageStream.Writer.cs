
using System;
using System.Text;
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

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

            public async UniTask<ReadMessageResult> SendWait(IMessage message, bool force, IFiber fiber = null)
            {
                if (!force && !_writeActive)
                {
                    X.SystemLog.Debug($"send message refuse {force} {_writeActive}");
                    return default;
                }

                WaitResponseHandle waitHandle = new WaitResponseHandle(message);
                _waitResponseList.TryAdd(waitHandle.Id, waitHandle);
                WriteMessageResult result = await Send(message, force, fiber);
                ReadMessageResult response = await waitHandle.ResponseTask;
                return response;
            }

            public async UniTask<WriteMessageResult> Send(IMessage message, bool force, IFiber fiber = null)
            {
                if (!force && !_writeActive)
                {
                    X.SystemLog.Debug($"send message refuse {force} {_writeActive}");
                    return new WriteMessageResult(NetOperateState.Cancel, "[Net]this operate is inactive");
                }

                string typeName = message.Descriptor.FullName;
                int typeNameSize = Encoding.UTF8.GetByteCount(typeName);
                int msgSize = message.CalculateSize() + sizeof(int) + typeNameSize;

                if (!NetUtility.CheckMessageSize(msgSize))
                {
                    X.SystemLog.Debug($"send message is too larget, will refuse, msgSize is {msgSize}");
                    return new WriteMessageResult(NetOperateState.Cancel, $"[Net][DENGER]send a large message. will interrupt this operate, msgSize is {msgSize}");
                }

                MessageWriteBuffer buffer = new MessageWriteBuffer(_connection._pool, msgSize);
                BitConverter.TryWriteBytes(buffer.Message, typeNameSize);
                NetUtility.WriteMessageNameTo(typeName, buffer.Message.Slice(sizeof(int), typeNameSize));
                message.WriteTo(buffer.Message.Slice(sizeof(int) + typeNameSize));

                if (fiber == null)
                    fiber = _connection._runFiber;

                WriteMessageResult result = await AsyncStateUtility.WriteMessageAsync(_connection._client, buffer, fiber);
                buffer.Dispose();
                return result;
            }
        }
    }

}
