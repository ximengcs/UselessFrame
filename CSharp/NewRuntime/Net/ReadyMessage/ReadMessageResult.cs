using System;

namespace UselessFrame.Net
{
    public struct ReadMessageResult : IDisposable
    {
        private ByteBufferPool _pool;

        public readonly Exception Error;
        public readonly byte[] MessageData;
        public readonly int MessageSize;
        public readonly NetMessageState State;

        internal ReadMessageResult(byte[] msgData, int msgSize, ByteBufferPool pool, NetMessageState state, Exception err = null)
        {
            _pool = pool;
            MessageSize = msgSize;
            MessageData = msgData;
            State = state;
            Error = err;
        }

        public void Dispose()
        {
            _pool.Release(MessageData);
            _pool = null;
        }
    }
}
