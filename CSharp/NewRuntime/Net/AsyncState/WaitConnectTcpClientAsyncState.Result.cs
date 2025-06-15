using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class AcceptConnectResult
    {
        public readonly TcpClient Client;
        public readonly NetOperateState State;
        public readonly string Message;

        public AcceptConnectResult(TcpClient client, NetOperateState errorCode, string message = null)
        {
            Client = client;
            State = errorCode;
            Message = message;
        }

        public AcceptConnectResult(NetOperateState code, string msg = null)
        {
            State = code;
            Message = msg;
            Client = null;
        }
    }
}
