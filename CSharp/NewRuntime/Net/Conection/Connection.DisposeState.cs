
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class DisposeState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.Dispose;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                if (_connection._server == null)
                    _connection.Dispose();
                else
                    DelayDestroy().Forget();
            }

            private async UniTask DelayDestroy(float seonds = 10)
            {
                await UniTaskExt.Delay(seonds);
                _connection.Dispose();
            }
        }
    }
}
