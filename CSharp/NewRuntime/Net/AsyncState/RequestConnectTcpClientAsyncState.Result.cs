using System;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public struct RequestConnectResult
    {
        public TcpClient Remote;
        public NetOperateState State;
        public string Message;
        public SocketException Exception;

        public static RequestConnectResult Create(TcpClient remote, NetOperateState errorCode, string message = null)
        {
            RequestConnectResult result = new RequestConnectResult();
            result.Remote = remote;
            result.State = errorCode;
            result.Message = message;
            result.Exception = null;
            return result;
        }

        public static RequestConnectResult Create(NetOperateState code, string msg = null)
        {
            RequestConnectResult result = new RequestConnectResult();
            result.State = code;
            result.Message = msg;
            result.Remote = null;
            result.Exception = null;
            return result;
        }

        public static RequestConnectResult Create(SocketException se, string stateMsg = null)
        {
            RequestConnectResult result = new RequestConnectResult();
            result.State = NetOperateState.SocketError;
            result.Message = stateMsg;
            result.Remote = null;
            result.Exception = se;
            return result;
        }
    }
}
