
namespace TestIMGUI.Core
{
    public enum ConnectionState
    {
        None,
        Connecting,
        TokenPending,
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
