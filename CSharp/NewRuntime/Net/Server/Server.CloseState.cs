
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class CloseState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Close;

            public override void OnEnter(NetFsmState<Server> preState)
            {
                base.OnEnter(preState);
                TryClose().Forget();
            }

            private async UniTask TryClose()
            {
                foreach (var entry in _connection._connections)
                {
                    Connection connection = entry.Value;
                    await connection.Fsm.ChangeState(typeof(Connection.DisposeState));
                }
                _connection._listener.Stop();
                await ChangeState<DisposeState>();
            }
        }
    }
}
