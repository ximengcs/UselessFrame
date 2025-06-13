
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class ConnectState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.Connect;

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
                TryConnect().Forget();
            }

            private async UniTask TryConnect()
            {
                AsyncBegin();

                RequestConnectResult result = await AsyncStateUtility.RequestConnectAsync(_connection._client, _connection._remoteIP);
                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            ChangeState<CheckConnectState>().Forget();
                            break;
                        }

                    default:
                        {
                            ChangeState<DisposeState>().Forget();
                            break;
                        }
                }

                AsyncEnd();
            }
        }
    }
}
