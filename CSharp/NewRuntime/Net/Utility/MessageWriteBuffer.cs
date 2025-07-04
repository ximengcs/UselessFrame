
using System;

namespace UselessFrame.Net
{
    public partial struct MessageWriteBuffer : IDisposable
    {
        private byte[] _buffer;
        private int _msgSize;
        private int _packageSize;
        private ByteBufferPool _pool;

        public Span<byte> LengthHead => _buffer.AsSpan(0, NetUtility.SizeLength);

        public Span<byte> CrcHead => _buffer.AsSpan(NetUtility.SizeLength, Crc16CcittKermit.CRCLength);

        public Span<byte> Message => _buffer.AsSpan(NetUtility.SizeLength + Crc16CcittKermit.CRCLength, _msgSize);

        public byte[] Package => _buffer;

        public int PackageSize => _packageSize;

        public MessageWriteBuffer(ByteBufferPool pool, int msgSize)
        {
            _pool = pool;
            _packageSize = NetUtility.SizeLength + Crc16CcittKermit.CRCLength + msgSize;
            _buffer = pool.Require(_packageSize);
            _msgSize = msgSize;
            BitConverter.TryWriteBytes(LengthHead, Crc16CcittKermit.CRCLength + msgSize);
        }

        public void Dispose()
        {
            if (_pool != null && _buffer != null)
            {
                _pool.Release(_buffer);
                _buffer = null;
                _pool = null;
            }
        }
    }
}
