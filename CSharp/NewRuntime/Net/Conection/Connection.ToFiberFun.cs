
using System;
using UselessFrame.NewRuntime;

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

            public static void UnRegister(object state)
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
        }
    }
}
