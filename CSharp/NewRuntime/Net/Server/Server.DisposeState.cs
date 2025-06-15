
namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class DisposeState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Dispose;

            public override void OnEnter(NetFsmState<Server> preState)
            {
                base.OnEnter(preState);
                _connection._listener = null;
                _connection._connections = null;
                _connection._onConnectionListChange = null;
            }
        }
    }
}
