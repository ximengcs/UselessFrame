﻿using System;
using Google.Protobuf;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class ReadMessageResult
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
            Exception = null;
        }

        internal ReadMessageResult(NetOperateState state, string stateMsg)
        {
            State = state;
            StateMessage = stateMsg;
            Message = null;
            Exception = null;
        }

        internal ReadMessageResult(SocketException socketEx, string stateMsg)
        {
            Exception = socketEx;
            State = NetOperateState.SocketError;
            StateMessage = stateMsg;
            Message = null;
        }
    }
}
