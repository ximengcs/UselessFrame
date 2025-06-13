using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UselessFrame.Net;

namespace UselessFrame.Net
{
    public struct AcceptConnectResult
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
