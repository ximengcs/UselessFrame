
namespace UselessFrame.Net
{
    public struct WriteMessageResult
    {
        public readonly NetMessageState ErrorCode;
        public readonly string Message;

        public WriteMessageResult(NetMessageState code, string msg = null)
        {
            ErrorCode = code;
            Message = msg;
        }
    }
}
