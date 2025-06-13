using System;
using System.Collections.Generic;
using System.Text;
using UselessFrame.Net;

namespace UselessFrame.Net
{
    public struct WriteMessageResult
    {
        public readonly NetOperateState State;
        public readonly string StateMessage;

        public WriteMessageResult(NetOperateState code, string msg = null)
        {
            State = code;
            StateMessage = msg;
        }
    }
}
