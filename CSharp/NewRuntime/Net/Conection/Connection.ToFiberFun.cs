
using Cysharp.Threading.Tasks;
using Google.Protobuf;
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

            public static async void SendMessageToRunFiber(object state)
            {
                var tuple = (Tuple<Connection, IMessage, bool, AutoResetUniTaskCompletionSource>)state;
                Connection connection = tuple.Item1;
                IMessage message = tuple.Item2;
                bool autoRelease = tuple.Item3;
                AutoResetUniTaskCompletionSource completeSource = tuple.Item4;
                await connection._fsm.Current.OnSendMessage(message, connection._dataFiber);
                if (autoRelease)
                    NetPoolUtility.ReleaseMessage(message);
                completeSource.TrySetResult();
            }

            public static async void SendWaitMessageToRunFiber(object state)
            {
                var tuple = (Tuple<Connection, IMessage, bool, AutoResetUniTaskCompletionSource<MessageResult>>)state;
                Connection connection = tuple.Item1;
                IMessage message = tuple.Item2;
                bool autoRelease = tuple.Item3;
                AutoResetUniTaskCompletionSource<MessageResult> completeSource = tuple.Item4;
                MessageResult result = await connection._fsm.Current.OnSendWaitMessage(message, connection._dataFiber);
                if (autoRelease)
                    NetPoolUtility.ReleaseMessage(message);
                completeSource.TrySetResult(result);
            }

            public static void TriggerMessageToDataFiber(object state)
            {
                var tuple = (Tuple<Action<MessageResult>, MessageResult>)state;
                tuple.Item1.Invoke(tuple.Item2);
            }

            public static void RunCommand(object state)
            {
                Tuple<Connection, MessageResult> result = (Tuple<Connection, MessageResult>)state;
                Connection connection = result.Item1;
                MessageResult reqResult = result.Item2;
                CommandMessage cmdMsg = (CommandMessage)reqResult.Message;
                string cmd = cmdMsg.CommandStr;
                cmd += $" --server_id \"{connection._server.Id}\"";
                cmd += $" --client_id \"{connection.Id}\"";
                CommandExecuteResult execResult = X.Command.Execute(cmd);
                reqResult.Response(new CommandResponseMessage()
                {
                    CommandCode = (int)execResult.Code,
                    CommandResult = execResult.Message
                }).Forget();
            }
        }
    }
}
