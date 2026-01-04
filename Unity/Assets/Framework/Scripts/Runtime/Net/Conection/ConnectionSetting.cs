
namespace UselessFrame.Net
{
    public class ConnectionSetting
    {
        public bool ShowReceiveKeepaliveLog;
        public bool ShowReceiveMessageInfo;
        public bool ShowSendMessageInfo;

        public ConnectionSetting()
        {
            ShowReceiveKeepaliveLog = false;
            ShowReceiveMessageInfo = false;
            ShowSendMessageInfo = false;
        }
    }
}
