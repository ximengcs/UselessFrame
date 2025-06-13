
namespace UselessFrame.Net
{
    public partial struct MessageWriteBuffer
    {
        internal static MessageWriteBuffer CloseBuffer = new MessageWriteBuffer(new byte[4]);

        internal MessageWriteBuffer(byte[] buffer)
        {
            _buffer = buffer;
            _msgSize = 0;
            _packageSize = buffer.Length;
            _pool = null;
        }
    }
}
