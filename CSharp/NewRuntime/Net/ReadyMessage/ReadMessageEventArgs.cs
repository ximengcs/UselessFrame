
using System;

namespace XFrameShare.Network
{
    public class ReadMessageEventArgs : EventArgs
    {
        public readonly Exception Error;
        public readonly byte[] MessageData;

        internal ReadMessageEventArgs(byte[] msgData)
        {
            MessageData = msgData;
            Error = null;
        }

        internal ReadMessageEventArgs(Exception err)
        {
            Error = err;
            MessageData = null;
        }
    }

}
