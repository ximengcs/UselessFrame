
using System;

namespace UselessFrame.Net
{
    public struct MessageWriteBuffer
    {
        private byte[] _buffer;
        private int _msgSize;
        private int _packageSize;

        public Span<byte> LengthHead => _buffer.AsSpan(0, sizeof(int));

        public Span<byte> CrcHead => new Span<byte>(_buffer, sizeof(int), Crc16CcittKermit.CRCLength);

        public Span<byte> Message => new Span<byte>(_buffer, sizeof(int) + Crc16CcittKermit.CRCLength, _msgSize);

        public byte[] Package => _buffer;

        public int PackageSize => _packageSize;

        public MessageWriteBuffer(byte[] buffer, int msgSize)
        {
            _buffer = buffer;
            _msgSize = msgSize;
            _packageSize = sizeof(int) + Crc16CcittKermit.CRCLength + _msgSize;
            BitConverter.TryWriteBytes(LengthHead, _packageSize);
        }
    }
}
