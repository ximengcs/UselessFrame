using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    public class NetPoolUtility
    {
        internal static NetObjectPool<ReadMessageTcpClientAsyncState> _readMessageAsyncPool;
        internal static NetObjectPool<WriteMessageTcpClientAsyncState> _writeMessageAsyncPool;
        internal static NetObjectPool<ReadMessageResult> _readMessageResultPool;
        internal static NetObjectPool<WriteMessageResult> _writeMessageResultPool;
        internal static NetObjectPool<MessageResult> _messageResultPool;
        private static Dictionary<Type, NetObjectPool> _messagePool;

        internal static void InitializePool()
        {
            _readMessageAsyncPool = new NetObjectPool<ReadMessageTcpClientAsyncState>();
            _writeMessageAsyncPool = new NetObjectPool<WriteMessageTcpClientAsyncState>();
            _readMessageResultPool = new NetObjectPool<ReadMessageResult>();
            _writeMessageResultPool = new NetObjectPool<WriteMessageResult>();
            _messageResultPool = new NetObjectPool<MessageResult>();
            _messagePool = new Dictionary<Type, NetObjectPool>(128);
        }

        public static T CreateMessage<T>() where T : class, IMessage, new()
        {
            lock (_messagePool)
            {
                if (!_messagePool.TryGetValue(typeof(T), out NetObjectPool pool))
                {
                    pool = new NetObjectPool();
                    _messagePool[typeof(T)] = pool;
                }

                object item = pool.Require();
                if (item == null)
                    return new T();
                return (T)item;
            }
        }

        internal static void ReleaseMessage(IMessage message)
        {
            RecursiveReleaseMessage(message);
        }

        private static void ReleaseMessageToPool(IMessage message)
        {
            lock (_messagePool)
            {
                Type messageType = message.GetType();
                if (!_messagePool.TryGetValue(messageType, out NetObjectPool pool))
                {
                    pool = new NetObjectPool();
                    _messagePool[messageType] = pool;
                }
                pool.Release(message);
            }
        }

        private static void RecursiveReleaseMessage(IMessage message)
        {
            if (message == null)
                return;

            ReleaseMessageToPool(message);
            foreach (FieldDescriptor field in message.Descriptor.Fields.InFieldNumberOrder())
            {
                IFieldAccessor accessor = field.Accessor;
                if (field.FieldType == FieldType.Message)
                {
                    RecursiveReleaseMessage((IMessage)accessor.GetValue(message));
                }
                accessor.Clear(message);
            }
        }
    }
}
