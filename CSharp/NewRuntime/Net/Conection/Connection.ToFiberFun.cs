
using Cysharp.Threading.Tasks;
using System;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Commands;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        public static class ToFiberFun
        {
            public static void RunCheckOnFiber(object state)
            {
                NetFsm<Connection> fsm = (NetFsm<Connection>)state;
                fsm.Start<CheckConnectState>();
            }

            public static void RunConnectOnFiber(object state)
            {
                NetFsm<Connection> fsm = (NetFsm<Connection>)state;
                fsm.Start<ConnectState>();
            }

            public static void RegisteConnection(object state)
            {
                IConnection connection = (IConnection)state;
                X.RegisterConnection(connection);
            }

            public static void UnRegisteConnection(object state)
            {
                var tuple = (Tuple<Server, Connection>)state;
                tuple.Item1.RemoveConnection(tuple.Item2);
                X.UnRegisterConnection(tuple.Item2);
            }

            public static void TriggerMessageToDataFiber(object state)
            {
                var tuple = (Tuple<Action<MessageResult>, MessageResult>)state;
                tuple.Item1.Invoke(tuple.Item2);
            }

            public static void RunCommand(object state)
            {
                Tuple<IFiber, MessageResult> result = (Tuple<IFiber, MessageResult>)state;
                MessageResult reqResult = result.Item2;
                CommandMessage cmdMsg = (CommandMessage)reqResult.Message;
                CommandExecuteResult execResult = X.Command.Execute(cmdMsg.CommandStr);
                reqResult.Response(new CommandResponseMessage()
                {
                    CommandCode = (int)execResult.Code,
                    CommandResult = execResult.Message
                }).Forget();
            }
        }
    }
}
