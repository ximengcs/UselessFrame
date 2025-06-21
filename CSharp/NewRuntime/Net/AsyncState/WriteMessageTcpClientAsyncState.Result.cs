
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class WriteMessageResult
    {
        public readonly NetOperateState State;
        public readonly string StateMessage;
        public readonly SocketException Exception;

        public WriteMessageResult(NetOperateState code, string msg = null)
        {
            State = code;
            StateMessage = msg;
            Exception = null;
        }

        public WriteMessageResult(SocketException e, string stateMsg = null)
        {
            State = NetOperateState.SocketError;
            StateMessage = stateMsg;
            Exception = e;
        }
    }
}
