
namespace UselessFrame.Net
{
    public struct WriteMessageResult
    {
        public readonly NetOperateState State;
        public readonly string Message;

        public WriteMessageResult(NetOperateState code, string msg = null)
        {
            State = code;
            Message = msg;
        }
    }
}
