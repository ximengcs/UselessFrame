
using Cysharp.Threading.Tasks;
using System.Net;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class ConnectState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.Connect;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                TryConnect().Forget();
            }

            private async UniTask TryConnect()
            {
                AsyncBegin();

                X.Log.Debug($"{DebugPrefix}TryConnect");
                RequestConnectResult result = await AsyncStateUtility.RequestConnectAsync(_connection._client, _connection._remoteIP);
                X.Log.Debug($"{DebugPrefix}TryConnect complete, {result.State}");
                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            _connection._localIP = (IPEndPoint)_connection._client.Client.LocalEndPoint;
                            ChangeState<CheckConnectState>().Forget();
                            break;
                        }

                    default:
                        {
                            X.Log.Error($"{DebugPrefix}connect error, {result.State} {result.Exception}");
                            ChangeState<DisposeState>().Forget();
                            break;
                        }
                }

                AsyncEnd();
            }
        }
    }
}
