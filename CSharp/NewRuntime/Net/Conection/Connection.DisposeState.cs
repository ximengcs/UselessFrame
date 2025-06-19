
using Cysharp.Threading.Tasks;

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
                DelayDestroy().Forget();
            }

            private async UniTask DelayDestroy(float seonds = 10)
            {
                _connection.Dispose();
            }
        }
    }
}
