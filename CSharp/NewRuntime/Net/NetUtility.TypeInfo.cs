
using Google.Protobuf;
using System;

namespace UselessFrame.Net
{
    public static partial class NetUtility
    {
        internal struct MessageTypeInfo
        {
            public Type Type;
            public MessageParser Parser;

            public MessageTypeInfo(Type type, MessageParser parser)
            {
                Type = type;
                Parser = parser;
            }
        }
    }
}
