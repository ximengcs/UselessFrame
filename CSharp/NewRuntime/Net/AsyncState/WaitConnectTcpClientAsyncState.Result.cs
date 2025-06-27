using System;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public struct AcceptConnectResult
    {
        private bool _disposed;
        public TcpClient Client;
        public NetOperateState State;
        public string Message;
        public SocketException Exception;

        public static AcceptConnectResult Create(TcpClient client, NetOperateState errorCode, string message = null)
        {
            AcceptConnectResult result = new AcceptConnectResult();
            result._disposed = false;
            result.Client = client;
            result.State = errorCode;
            result.Message = message;
            result.Exception = null;
            return result;
        }

        public static AcceptConnectResult Create(NetOperateState code, string msg = null)
        {
            AcceptConnectResult result = new AcceptConnectResult();
            result._disposed = false;
            result.State = code;
            result.Message = msg;
            result.Client = null;
            result.Exception = null;
            return result;
        }

        public static AcceptConnectResult Create(SocketException e, string stateMsg = null)
        {
            AcceptConnectResult result = new AcceptConnectResult();
            result._disposed = false;
            result.State = NetOperateState.SocketError;
            result.Exception = e;
            result.Client = null;
            result.Message = stateMsg;
            return result;
        }
    }
}
