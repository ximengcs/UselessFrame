using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class RequestConnectResult
    {
        public readonly TcpClient Remote;
        public readonly NetOperateState State;
        public readonly string Message;
        public readonly SocketException Exception;

        public RequestConnectResult(TcpClient remote, NetOperateState errorCode, string message = null)
        {
            Remote = remote;
            State = errorCode;
            Message = message;
            Exception = null;
        }

        public RequestConnectResult(NetOperateState code, string msg = null)
        {
            State = code;
            Message = msg;
            Remote = null;
            Exception = null;
        }

        public RequestConnectResult(SocketException se, string stateMsg = null)
        {
            State = NetOperateState.SocketError;
            Message = stateMsg;
            Remote = null;
            Exception = se;
        }
    }
}
