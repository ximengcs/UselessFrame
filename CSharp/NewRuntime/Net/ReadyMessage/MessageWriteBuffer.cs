
using System;

namespace UselessFrame.Net
{
    public partial struct MessageWriteBuffer : IDisposable
    {
        private byte[] _buffer;
        private int _msgSize;
        private int _packageSize;
        private ByteBufferPool _pool;

        public Span<byte> LengthHead => _buffer.AsSpan(0, sizeof(int));

        public Span<byte> CrcHead => new Span<byte>(_buffer, sizeof(int), Crc16CcittKermit.CRCLength);

        public Span<byte> Message => new Span<byte>(_buffer, sizeof(int) + Crc16CcittKermit.CRCLength, _msgSize);

        public byte[] Package => _buffer;

        public int PackageSize => _packageSize;

        public MessageWriteBuffer(ByteBufferPool pool, int msgSize)
        {
            _pool = pool;
            _packageSize = sizeof(int) + Crc16CcittKermit.CRCLength + msgSize;
            _buffer = pool.Require(_packageSize);
            _msgSize = msgSize;
            BitConverter.TryWriteBytes(LengthHead, _packageSize);
        }

        public void Dispose()
        {
            if (_pool != null)
            {
                _pool.Release(_buffer);
                _buffer = null;
                _pool = null;
            }
        }
    }
}
