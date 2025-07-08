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
            private IFieldAccessor _requestTokenFiled;
            private IFieldAccessor _responseTokenFiled;

            public Type Type;
            public MessageParser Parser;
            public MessageDescriptor Descriptor;

            public bool HasRequestToken
            {
                get
                {
                    return _requestTokenFiled != null;
                }
            }

            public bool HasResponseToken
            {
                get
                {
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
                EnsureRequestToken();
                EnsureResponseToken();
            }

            public int GetResponseToken(IMessage message)
            {
                if (_responseTokenFiled == null)
                    return 0;

                return (int)_responseTokenFiled.GetValue(message);
            }

            public void SetResponseToken(IMessage message, int token)
            {
                if (_responseTokenFiled == null)
                    return;

                _responseTokenFiled.SetValue(message, token);
            }

            private void EnsureResponseToken()
            {
                if (_responseTokenFiled == null)
                {
                    FieldDescriptor field = Descriptor.FindFieldByName("ResponseToken");
                    if (field != null)
                    {
                        _responseTokenFiled = field.Accessor;
                    }
                    else
                    {
                        X.SystemLog.Debug($"{Descriptor.FullName} do not has response token");
                        return;
                    }
                }
            }

            public int GetRequestToken(IMessage message)
            {
                if (_requestTokenFiled == null)
                    return 0;

                return (int)_requestTokenFiled.GetValue(message);
            }

            public void SetRequestToken(IMessage message, int token)
            {
                if (_requestTokenFiled == null)
                    return;

                _requestTokenFiled.SetValue(message, token);
            }

            private void EnsureRequestToken()
            {
                if (_requestTokenFiled == null)
                {
                    FieldDescriptor field = Descriptor.FindFieldByName("RequestToken");
                    if (field != null)
                    {
                        _requestTokenFiled = field.Accessor;
                    }
                    else
                    {
                        X.SystemLog.Debug($"{Descriptor.FullName} do not has request token");
                        return;
                    }
                }
            }
        }
    }
}
