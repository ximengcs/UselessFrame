
namespace UselessFrame.Net
{
    public class WriteMessageResult
    {
        public readonly NetOperateState State;
        public readonly string StateMessage;

        public WriteMessageResult(NetOperateState code, string msg = null)
        {
            State = code;
            StateMessage = msg;
        }
    }
}
