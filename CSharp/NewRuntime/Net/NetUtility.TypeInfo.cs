using System;
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Google.Protobuf.Reflection;

namespace UselessFrame.Net
{
    public static partial class NetUtility
    {
        internal struct MessageTypeInfo
        {
            private bool _requestTokenFiledCheck;
            private bool _responseTokenFiledCheck;
            private IFieldAccessor _requestTokenFiled;
            private IFieldAccessor _responseTokenFiled;

            public Type Type;
            public MessageParser Parser;
            public MessageDescriptor Descriptor;

            public bool HasRequestToken
            {
                get
                {
                    EnsureRequestToken();
                    return _requestTokenFiled != null;
                }
            }

            public bool HasResponseToken
            {
                get
                {
                    EnsureResponseToken();
                    return _responseTokenFiled != null;
                }
            }

            public MessageTypeInfo(Type type, MessageParser parser, MessageDescriptor descriptor)
            {
                Type = type;
                Parser = parser;
                Descriptor = descriptor;
                _requestTokenFiled = null;
                _responseTokenFiled = null;
                _requestTokenFiledCheck = false;
                _responseTokenFiledCheck = false;
            }

            public Guid GetResponseToken(IMessage message)
            {
                EnsureResponseToken();
                if (_responseTokenFiled == null)
                    return Guid.Empty;

                ByteString value = (ByteString)_responseTokenFiled.GetValue(message);
                return new Guid(value.Span);
            }

            public void SetResponseToken(IMessage message, Guid token)
            {
                EnsureResponseToken();
                if (_responseTokenFiled == null)
                    return;

                ByteString bytes = ByteString.CopyFrom(token.ToByteArray());
                _responseTokenFiled.SetValue(message, bytes);
            }

            private void EnsureResponseToken()
            {
                if (_responseTokenFiledCheck)
                    return;
                _responseTokenFiledCheck = true;

                if (_responseTokenFiled == null)
                {
                    FieldDescriptor field = Descriptor.FindFieldByName("ResponseToken");
                    if (field != null)
                    {
                        _responseTokenFiled = field.Accessor;
                    }
                    else
                    {
                        X.SystemLog.Debug($"response do not has token");
                        return;
                    }
                }
            }

            public Guid GetRequestToken(IMessage message)
            {
                EnsureRequestToken();
                if (_requestTokenFiled == null)
                    return Guid.Empty;

                ByteString value = (ByteString)_requestTokenFiled.GetValue(message);
                return new Guid(value.Span);
            }

            public void SetRequestToken(IMessage message, Guid token)
            {
                EnsureRequestToken();
                if (_requestTokenFiled == null)
                    return;

                ByteString bytes = ByteString.CopyFrom(token.ToByteArray());
                _requestTokenFiled.SetValue(message, bytes);
            }

            private void EnsureRequestToken()
            {
                if (_requestTokenFiledCheck)
                    return;
                _requestTokenFiledCheck = true;

                if (_requestTokenFiled == null)
                {
                    FieldDescriptor field = Descriptor.FindFieldByName("RequestToken");
                    if (field != null)
                    {
                        _requestTokenFiled = field.Accessor;
                    }
                    else
                    {
                        X.SystemLog.Debug($"response do not has token");
                        return;
                    }
                }
            }
        }
    }
}
