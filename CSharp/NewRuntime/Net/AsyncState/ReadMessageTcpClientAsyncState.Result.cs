using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UselessFrame.Net;

namespace UselessFrame.Net
{
    public struct ReadMessageResult
    {
        public readonly NetOperateState State;
        public readonly string StateMessage;
        public readonly IMessage Message;
        public readonly SocketException Exception;

        internal ReadMessageResult(byte[] msgData, int msgSize, ByteBufferPool pool, NetOperateState state, string stateMsg = null)
        {
            State = state;
            StateMessage = stateMsg;
            Message = msgData.AsMemory(Crc16CcittKermit.CRCLength, msgSize - Crc16CcittKermit.CRCLength).ToMessage();
            pool.Release(msgData);
            Exception = null;
        }

        internal ReadMessageResult(NetOperateState state, string stateMsg)
        {
            State = state;
            StateMessage = stateMsg;
            Message = null;
            Exception = null;
        }

        internal ReadMessageResult(SocketException socketEx)
        {
            Exception = socketEx;
            State = NetOperateState.SocketError;
            StateMessage = socketEx.Message;
            Message = null;
        }
    }
}
