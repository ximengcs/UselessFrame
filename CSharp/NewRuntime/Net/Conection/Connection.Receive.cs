using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UselessFrame.Net;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private async UniTaskVoid RequestMessage()
        {
            ReadMessageResult result = await MessageUtility.ReadMessageAsync(_client, _pool, _closeTokenSource.Token);
            if (_state == ConnectionState.Normal)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        SuccessHandler(result);
                        RequestMessage().Forget();
                        break;
                }
            }
        }

        private void SuccessHandler(ReadMessageResult result)
        {
            Memory<byte> datas = result.Bytes;
            string content = Encoding.UTF8.GetString(datas.Span);
            result.Dispose();
            Console.WriteLine($"read success {content}");
        }
    }
}
