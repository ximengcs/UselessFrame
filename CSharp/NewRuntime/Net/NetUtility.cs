
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Net
{
    public static partial class NetUtility
    {
        private static Dictionary<string, MessageTypeInfo> _types = new Dictionary<string, MessageTypeInfo>();

        internal static IMessage ToMessage(this Memory<byte> datas)
        {
            int typeNameSize = BitConverter.ToInt32(datas.Span);
            string typeName = Encoding.UTF8.GetString(datas.Span.Slice(sizeof(int), typeNameSize));
            MessageTypeInfo info = NetUtility.GetMessageTypeInfo(typeName);
            IMessage message = info.Parser.ParseFrom(datas.Span.Slice(sizeof(int) + typeNameSize));
            return message;
        }

        internal static MessageTypeInfo GetMessageTypeInfo(string messageTypeFullName)
        {
            if (_types.TryGetValue(messageTypeFullName, out MessageTypeInfo messageTypeInfo))
                return messageTypeInfo;

            lock (_types)
            {
                ITypeCollection typeMap = X.Type.GetCollection(typeof(IMessage));
                Type type = typeMap.Get(messageTypeFullName);
                MessageParser parser = (MessageParser)type.GetProperty("Parser", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                messageTypeInfo = new MessageTypeInfo(type, parser);
                _types.TryAdd(messageTypeFullName, messageTypeInfo);
            }

            return messageTypeInfo;
        }

        public static ServerToken CreateToken(Guid guid)
        {
            ServerToken token = new ServerToken()
            {
                Id = ByteString.CopyFrom(guid.ToByteArray())
            };
            return token;
        }

        public static Guid GetId(this ServerToken token)
        {
            return new Guid(token.Id.Memory.Span);
        }
    }
}
