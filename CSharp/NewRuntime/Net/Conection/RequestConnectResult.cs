using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.Net.Conection
{
    public struct RequestConnectResult
    {
        public readonly TcpClient Remote;
        public readonly NetOperateState State;
        public readonly string Message;

        public RequestConnectResult(TcpClient remote, NetOperateState errorCode, string message = null)
        {
            Remote = remote;
            State = errorCode;
            Message = message;
        }

        public RequestConnectResult(NetOperateState code, string msg = null)
        {
            State = code;
            Message = msg;
            Remote = null;
        }
    }
}
