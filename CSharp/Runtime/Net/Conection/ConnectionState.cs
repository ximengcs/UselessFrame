
namespace UselessFrame.Net
{
    public enum ConnectionState
    {
        None,
        Connect,
        CheckConnect,
        CloseRequest,
        CloseResponse,
        Dispose,
        Run,
        TokenResponse,
        TokenVerify
    }
}
