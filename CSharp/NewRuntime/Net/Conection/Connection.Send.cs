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
                buffer.Dispose();
            }
            else
            {
                _state.Value = ConnectionState.FatalErrorClose;
            }
        }
    }
}
