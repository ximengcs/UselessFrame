using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class AcceptConnectResult
    {
        public readonly TcpClient Client;
        public readonly NetOperateState State;
        public readonly string Message;
        public readonly SocketException Exception;

        public AcceptConnectResult(TcpClient client, NetOperateState errorCode, string message = null)
        {
            Client = client;
            State = errorCode;
            Message = message;
            Exception = null;
        }

        public AcceptConnectResult(NetOperateState code, string msg = null)
        {
            State = code;
            Message = msg;
            Client = null;
            Exception = null;
        }

        public AcceptConnectResult(SocketException e, string stateMsg = null)
        {
            State = NetOperateState.SocketError;
            Exception = e;
            Client = null;
            Message = stateMsg;
        }
    }
}
