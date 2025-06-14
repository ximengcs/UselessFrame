
namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class DisposeState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.Dispose;

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
                _connection._client.Close();
                _connection._pool.Dispose();
                _connection._onReceiveMessage = null;
                _connection._runFiber.Dispose();
            }
        }
    }
}
