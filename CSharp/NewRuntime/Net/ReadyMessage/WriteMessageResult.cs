
namespace UselessFrame.Net
{
    public struct WriteMessageResult
    {
        public readonly NetErrorCode ErrorCode;

        public WriteMessageResult(NetErrorCode code)
        {
            ErrorCode = code;
        }
    }
}
