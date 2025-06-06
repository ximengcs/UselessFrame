
namespace UselessFrame.Net
{
    public enum NetMessageState
    {
        OK,
        NormalClose,
        Disconnect,
        ParamError,
        SocketError,
        Cancel,
        DataError,
        Unknown
    }
}
