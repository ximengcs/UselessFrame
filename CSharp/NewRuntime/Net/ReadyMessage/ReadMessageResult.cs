using System;
using UselessFrame.Runtime.Pools;
using Vulkan;

namespace UselessFrame.Net
{
    public struct ReadMessageResult : IDisposable
    {
        private ByteBufferPool _pool;
        private byte[] _buffer;
        private int _messageSize;

        public readonly NetOperateState State;
        public readonly string StateMessage;

        public Memory<byte> Bytes => _buffer.AsMemory(Crc16CcittKermit.CRCLength, _messageSize - Crc16CcittKermit.CRCLength);

        internal ReadMessageResult(byte[] msgData, int msgSize, ByteBufferPool pool, NetOperateState state, string stateMsg = null)
        {
            _pool = pool;
            State = state;
            _buffer = msgData;
            _messageSize = msgSize;
            StateMessage = stateMsg;
        }

        internal ReadMessageResult(NetOperateState state, string stateMsg)
        {
            _pool = default;
            _buffer = null;
            _messageSize = default;
            State = state;
            StateMessage = stateMsg;
        }

        public void Dispose()
        {
            _pool.Release(_buffer);
            _pool = null;
        }
    }
}
