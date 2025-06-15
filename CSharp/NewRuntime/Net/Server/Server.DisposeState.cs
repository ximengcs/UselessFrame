
namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class DisposeState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Dispose;

            public override void OnEnter(NetFsmState<Server> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                _connection._listener = null;
                _connection._connections = null;
                _connection._onConnectionListChange = null;
            }
        }
    }
}
