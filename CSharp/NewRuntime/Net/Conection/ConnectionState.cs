
namespace TestIMGUI.Core
{
    public enum ConnectionState
    {
        Known,
        Normal,
        Reconnect,
        SocketError,
        NormalClose,
        FatalErrorClose,
        ReconnectErrorClose
    }
}
