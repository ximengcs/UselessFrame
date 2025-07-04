
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Types;

namespace UselessFrame.Net
{
    public static partial class NetUtility
    {
        internal const int SizeLength = sizeof(int);

        private static ByteStringTree _messageBytesTree = new ByteStringTree(512);
        private static Dictionary<string, MessageTypeInfo> _types = new Dictionary<string, MessageTypeInfo>(512);

        internal static bool CheckMessageSize(int size)
        {
            return size <= 1024 * 1024 * 10;
        }

        internal static void WriteMessageNameTo(string name, Span<byte> buffer)
        {
            lock (_messageBytesTree)
            {
                if (!_messageBytesTree.Contains(name))
                    _messageBytesTree.Add(name);
                _messageBytesTree.WriteTo(name, buffer);
            }
        }

        internal static string GetMessageName(ReadOnlySpan<byte> data)
        {
            lock (_messageBytesTree)
            {
                string name = _messageBytesTree.GetString(data);
                if (string.IsNullOrEmpty(name))
                {
                    name = Encoding.UTF8.GetString(data);
                    _messageBytesTree.Add(name);
                }
                return name;
            }
        }

        internal static IMessage ToMessage(this Memory<byte> datas)
        {
            int typeNameSize = BitConverter.ToInt32(datas.Span);
            string typeName = GetMessageName(datas.Span.Slice(sizeof(int), typeNameSize));
            MessageTypeInfo info = NetUtility.GetMessageTypeInfo(typeName);
            IMessage message = info.Parser.ParseFrom(datas.Span.Slice(sizeof(int) + typeNameSize));
            return message;
        }

        internal static MessageTypeInfo GetMessageTypeInfo(IMessage message)
        {
            return GetMessageTypeInfo(message.GetType().FullName);
        }

        internal static MessageTypeInfo GetMessageTypeInfo(string messageTypeFullName)
        {
            lock (_types)
            {
                if (_types.TryGetValue(messageTypeFullName, out MessageTypeInfo messageTypeInfo))
                    return messageTypeInfo;

                ITypeCollection typeMap = X.Type.GetCollection(typeof(IMessage));
                Type type = typeMap.Get(messageTypeFullName);
                MessageParser parser = (MessageParser)type.GetProperty("Parser", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                MessageDescriptor descriptor = (MessageDescriptor)type.GetProperty("Descriptor", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                messageTypeInfo = new MessageTypeInfo(type, parser, descriptor);
                _types.TryAdd(messageTypeFullName, messageTypeInfo);
                return messageTypeInfo;
            }
        }

        internal static ServerToken CreateToken(Guid guid)
        {
            ServerToken token = new ServerToken()
            {
                Id = ByteString.CopyFrom(guid.ToByteArray())
            };
            return token;
        }

        internal static Guid GetId(this ServerToken token)
        {
            return new Guid(token.Id.Memory.Span);
        }

        public static IPEndPoint GetLocalIPEndPoint(int port, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            IPAddress ip = GetLocalIPAddress(addressFamily);
            return new IPEndPoint(ip, port);
        }

        public static IPAddress GetLocalIPAddress(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = null;
            foreach (IPAddress address in ipHost.AddressList)
            {
                if (address.AddressFamily == addressFamily)
                {
                    ipAddress = address;
                    break;
                }
            }
            return ipAddress;
        }

    }
}
