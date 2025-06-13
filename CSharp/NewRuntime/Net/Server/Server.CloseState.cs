
namespace UselessFrame.Net
{
    internal partial class Server
    {
        public class CloseState : NetFsmState<Server>
        {
            public override int State => (int)ServerState.Close;
        }
    }
}
