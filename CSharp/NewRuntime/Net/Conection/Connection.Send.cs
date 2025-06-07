using Cysharp.Threading.Tasks;
using System.Text;
using UselessFrame.Net;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        public async UniTask Send(string msg)
        {
            MessageWriteBuffer buffer = new MessageWriteBuffer(_pool, Encoding.UTF8.GetByteCount(msg));
            Encoding.UTF8.GetBytes(msg, buffer.Message);
            WriteMessageResult result = await MessageUtility.WriteMessageAsync(_client, buffer, _closeTokenSource.Token);
            if (_state.Value == ConnectionState.Normal)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        buffer.Dispose();
                        break;

                    case NetOperateState.NormalClose:
                        _state.Value = ConnectionState.NormalClose;
                        _closeTokenSource.Cancel();
                        Dispose();
                        break;

                    case NetOperateState.Disconnect:
                    case NetOperateState.DataError:
                    case NetOperateState.ParamError:
                    case NetOperateState.PermissionError:
                    case NetOperateState.Unknown:
                        _state.Value = ConnectionState.FatalErrorClose;
                        _closeTokenSource.Cancel();
                        Dispose();
                        break;

                    case NetOperateState.SocketError:
                        _state.Value = ConnectionState.SocketError;
                        break;
                }
            }
        }
    }
}
