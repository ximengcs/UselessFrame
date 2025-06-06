using System;

namespace UselessFrame.Net
{
    public struct ReadMessageResult
    {
        private ByteBufferPool _pool;

        public readonly Exception Error;
        public readonly byte[] MessageData;
        public readonly int MessageSize;

        internal ReadMessageResult(byte[] msgData, int msgSize, ByteBufferPool pool, Exception err = null)
        {
            _pool = pool;
            MessageSize = msgSize;
            MessageData = msgData;
            Error = err;
        }

        public void Dispose()
        {
            _pool.Release(MessageData);
        }
    }

}
