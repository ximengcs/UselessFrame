﻿
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Text;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Pools;

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
                if (!force && !_writeActive)
                {
                    X.SystemLog.Debug($"send message refuse {force} {_writeActive}");
                    return default;
                }

                string typeName = message.Descriptor.FullName;
                int typeNameSize = Encoding.UTF8.GetByteCount(typeName);
                int msgSize = message.CalculateSize() + sizeof(int) + typeNameSize;

                MessageWriteBuffer buffer = new MessageWriteBuffer(_connection._pool, msgSize);
                BitConverter.TryWriteBytes(buffer.Message, typeNameSize);
                Encoding.UTF8.GetBytes(typeName, buffer.Message.Slice(sizeof(int), typeNameSize));
                message.WriteTo(buffer.Message.Slice(sizeof(int) + typeNameSize));
                WriteMessageResult result = await AsyncStateUtility.WriteMessageAsync(_connection._client, buffer, _connection._runFiber);
                return result;
            }
        }
    }

}
