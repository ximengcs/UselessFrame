using System;
using Google.Protobuf;
using System.Net.Sockets;

namespace UselessFrame.Net
{
    public class ReadMessageResult : INetPoolObject, IDisposable
    {
        public NetOperateState State;
        public string StateMessage;
        public IMessage Message;
        public SocketException Exception;

        internal static ReadMessageResult Create(byte[] msgData, int msgSize, ByteBufferPool pool, NetOperateState state, string stateMsg = null)
        {
            ReadMessageResult result = NetPoolUtility._readMessageResultPool.Require();
            result.State = state;
            result.StateMessage = stateMsg;
            result.Message = msgData.AsMemory(Crc16CcittKermit.CRCLength, msgSize - Crc16CcittKermit.CRCLength).ToMessage();
            result.Exception = null;
            return result;
        }

        internal static ReadMessageResult Create(NetOperateState state, string stateMsg)
        {
            ReadMessageResult result = NetPoolUtility._readMessageResultPool.Require();
            result.State = state;
            result.StateMessage = stateMsg;
            result.Message = null;
            result.Exception = null;
            return result;
        }

        internal static ReadMessageResult Create(SocketException socketEx, string stateMsg)
        {
            ReadMessageResult result = NetPoolUtility._readMessageResultPool.Require();
            result.Exception = socketEx;
            result.State = NetOperateState.SocketError;
            result.StateMessage = stateMsg;
            result.Message = null;
            return result;
        }

        public void Dispose()
        {
            NetPoolUtility._readMessageResultPool.Release(this);
        }

        public void Reset()
        {
            StateMessage = null;
            Message = null;
            Exception = null;
        }
    }
}
