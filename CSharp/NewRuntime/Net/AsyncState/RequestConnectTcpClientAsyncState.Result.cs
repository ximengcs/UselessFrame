using System;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class RequestConnectResult : IDisposable
    {
        public TcpClient Remote;
        public NetOperateState State;
        public string Message;
        public SocketException Exception;

        public static RequestConnectResult Create(TcpClient remote, NetOperateState errorCode, string message = null)
        {
            RequestConnectResult result = NetPoolUtility._requestConnectResultPool.Require();
            result.Remote = remote;
            result.State = errorCode;
            result.Message = message;
            result.Exception = null;
            return result;
        }

        public static RequestConnectResult Create(NetOperateState code, string msg = null)
        {
            RequestConnectResult result = NetPoolUtility._requestConnectResultPool.Require();
            result.State = code;
            result.Message = msg;
            result.Remote = null;
            result.Exception = null;
            return result;
        }

        public static RequestConnectResult Create(SocketException se, string stateMsg = null)
        {
            RequestConnectResult result = NetPoolUtility._requestConnectResultPool.Require();
            result.State = NetOperateState.SocketError;
            result.Message = stateMsg;
            result.Remote = null;
            result.Exception = se;
            return result;
        }

        public void Dispose()
        {
            Reset();
            NetPoolUtility._requestConnectResultPool.Release(this);
        }

        public void Reset()
        {
            Remote = null;
            Message = null;
            Exception = null;
        }
    }
}
