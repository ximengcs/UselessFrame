
using System;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class WriteMessageResult : IDisposable
    {
        public NetOperateState State;
        public string StateMessage;
        public SocketException Exception;

        public static WriteMessageResult Create(NetOperateState code, string msg = null)
        {
            WriteMessageResult result = NetPoolUtility._writeMessageResultPool.Require();
            result.State = code;
            result.StateMessage = msg;
            result.Exception = null;
            return result;
        }

        public static WriteMessageResult Create(SocketException e, string stateMsg = null)
        {
            WriteMessageResult result = NetPoolUtility._writeMessageResultPool.Require();
            result.State = NetOperateState.SocketError;
            result.StateMessage = stateMsg;
            result.Exception = e;
            return result;
        }

        public void Dispose()
        {
            Reset();
            NetPoolUtility._writeMessageResultPool.Release(this);
        }

        public void Reset()
        {
            StateMessage = null;
            Exception = null;
        }
    }
}
