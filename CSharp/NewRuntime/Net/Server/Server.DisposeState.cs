
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
            }
        }
    }
}
