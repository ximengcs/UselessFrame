
using System;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public struct WriteMessageResult
    {
        public NetOperateState State;
        public string StateMessage;
        public SocketException Exception;

        public static WriteMessageResult Create(NetOperateState code, string msg = null)
        {
            WriteMessageResult result = new WriteMessageResult();
            result.State = code;
            result.StateMessage = msg;
            result.Exception = null;
            return result;
        }

        public static WriteMessageResult Create(SocketException e, string stateMsg = null)
        {
            WriteMessageResult result = new WriteMessageResult();
            result.State = NetOperateState.SocketError;
            result.StateMessage = stateMsg;
            result.Exception = e;
            return result;
        }
    }
}
