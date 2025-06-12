
namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal class RunState : ConnectionState
        {
            public override void OnInit()
            {

            }

            public override void OnEnter(ConnectionState preState)
            {
                _connection._sender.SetActive(true);
            }

            public override void OnExit()
            {

            }

            public override void OnDispose()
            {

            }

        }
    }
}
