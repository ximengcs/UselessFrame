
namespace TestIMGUI.Core
{
    public partial class Connection
    {
        public struct DebugInfo
        {
            public readonly int SendTimes;
            public readonly int ReceiveTimes;

            public DebugInfo(int sendTimes, int receiveTimes)
            {
                SendTimes = sendTimes;
                ReceiveTimes = receiveTimes;
            }
        }
    }
}
