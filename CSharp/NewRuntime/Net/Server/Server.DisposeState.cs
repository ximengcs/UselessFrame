
namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class DisposeState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Dispose;
        }
    }
}
