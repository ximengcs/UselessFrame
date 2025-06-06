using System;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Net
{
    public struct ReadMessageResult : IDisposable
    {
        private ByteBufferPool _pool;

        public readonly byte[] MessageData;
        public readonly int MessageSize;
        public readonly NetMessageState State;
        public readonly string StateMessage;

        internal ReadMessageResult(byte[] msgData, int msgSize, ByteBufferPool pool, NetMessageState state, string stateMsg = null)
        {
            _pool = pool;
            MessageSize = msgSize;
            MessageData = msgData;
            State = state;
            StateMessage = stateMsg;
        }

        internal ReadMessageResult(NetMessageState state, string stateMsg)
        {
            _pool = default;
            MessageSize = default;
            MessageData = default;
            State = state;
            StateMessage = stateMsg;
        }

        public void Dispose()
        {
            _pool.Release(MessageData);
            _pool = null;
        }
    }
}
