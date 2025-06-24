using System;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class AcceptConnectResult : IDisposable
    {
        private bool _disposed;
        public TcpClient Client;
        public NetOperateState State;
        public string Message;
        public SocketException Exception;

        public static AcceptConnectResult Create(TcpClient client, NetOperateState errorCode, string message = null)
        {
            AcceptConnectResult result = NetPoolUtility._waitConnectResultPool.Require();
            result._disposed = false;
            result.Client = client;
            result.State = errorCode;
            result.Message = message;
            result.Exception = null;
            return result;
        }

        public static AcceptConnectResult Create(NetOperateState code, string msg = null)
        {
            AcceptConnectResult result = NetPoolUtility._waitConnectResultPool.Require();
            result._disposed = false;
            result.State = code;
            result.Message = msg;
            result.Client = null;
            result.Exception = null;
            return result;
        }

        public static AcceptConnectResult Create(SocketException e, string stateMsg = null)
        {
            AcceptConnectResult result = NetPoolUtility._waitConnectResultPool.Require();
            result._disposed = false;
            result.State = NetOperateState.SocketError;
            result.Exception = e;
            result.Client = null;
            result.Message = stateMsg;
            return result;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Reset();
            NetPoolUtility._waitConnectResultPool.Release(this);
        }

        public void Reset()
        {
            Client = null;
            Message = null;
            Exception = null;
        }
    }
}
