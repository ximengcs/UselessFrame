
namespace UselessFrame.Net
{
    public struct WriteMessageResult
    {
        public readonly NetMessageState ErrorCode;

        public WriteMessageResult(NetMessageState code)
        {
            ErrorCode = code;
        }
    }
}
