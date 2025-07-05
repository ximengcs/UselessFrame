using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class RunState
        {
            public override void OnInit()
            {
                base.OnInit();
                _messageHandler = new Dictionary<Type, Func<MessageResult, bool>>()
                {
                    { typeof(CloseResponseState), CloseRequestHandler },
                    { typeof(KeepAlive), KeepAliveHandler },
                    { typeof(CommandMessage), CommandMessageHandler },
                    { typeof(TestLatencyMessage), TestLatencyMessageHandler },
                };
            }

            private bool CloseRequestHandler(MessageResult result)
            {
                ChangeState<CloseResponseState>(result).Forget();
                CancelAllAsyncWait();
                return false;
            }

            private bool KeepAliveHandler(MessageResult result)
            {
                if (_connection.GetRuntimeData<ConnectionSetting>().ShowReceiveKeepaliveLog)
                    X.SystemLog.Debug($"{DebugPrefix}receive keepalive.");
                return true;
            }

            private bool CommandMessageHandler(MessageResult result)
            {
                CommandMessage cmd = (CommandMessage)result.Message;
                X.SystemLog.Debug($"{DebugPrefix}execute command -> {cmd.CommandStr}.");
                _connection._dataFiber.Post(ToFiberFun.RunCommand, Tuple.Create(_connection, result));
                return true;
            }

            private bool TestLatencyMessageHandler(MessageResult result)
            {
                X.SystemLog.Debug($"{DebugPrefix}test latency");
                TestLatencyMessage test = (TestLatencyMessage)result.Message;
                TestLatencyResponseMessage rspTest = NetPoolUtility.CreateMessage<TestLatencyResponseMessage>();
                rspTest.Time = Stopwatch.GetTimestamp();
                result.Response(rspTest).Forget();
                return true;
            }
        }
    }
}
