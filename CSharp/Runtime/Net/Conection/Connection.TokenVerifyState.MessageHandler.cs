using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class TokenVerifyState
        {
            private Dictionary<Type, Func<MessageResult, bool>> _messageHandler;

            public override void OnInit()
            {
                base.OnInit();
                _messageHandler = new Dictionary<Type, Func<MessageResult, bool>>()
                {
                    { typeof(CloseResponseState), CloseRequestHandler },
                    { typeof(TestServerTimeMessage), TestServerTimeMessageHandler },
                    { typeof(ServerTokenRequest), ServerTokenRequestHandler },
                };
            }

            private bool ServerTokenRequestHandler(MessageResult result)
            {
                Verify().Forget();
                return true;
            }

            private bool CloseRequestHandler(MessageResult result)
            {
                ChangeState<CloseResponseState>(result).Forget();
                CancelAllAsyncWait();
                return false;
            }

            private bool TestServerTimeMessageHandler(MessageResult result)
            {
                TestServerTimeMessage test = (TestServerTimeMessage)result.Message;
                TestServerTimeResponseMessage rspTest = NetPoolUtility.CreateMessage<TestServerTimeResponseMessage>();
                rspTest.Time = DateTime.UtcNow.Ticks;
                result.Response(rspTest).Forget();
                return true;
            }
        }
    }
}
