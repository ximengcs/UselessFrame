
namespace TestIMGUI.Core
{
    public enum ConnectionState
    {
        None,
        Connecting,
        UnKnown,
        Normal,
        Reconnect,
        SocketError,
        NormalClose,
        FatalErrorClose,
        ReconnectErrorClose,
        FatalConnect
    }
}
