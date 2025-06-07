
using Cysharp.Threading.Tasks;
using System;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Net.Conection;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        public void Reconnect()
        {
            switch (_state.Value)
            {
                case ConnectionState.FatalErrorClose:
                    _state.Value = ConnectionState.Reconnect;
                    TryReconnect().Forget();
                    break;
            }
        }

        private async UniTaskVoid TryReconnect()
        {
            RequestConnectResult result = await ConnectionUtility.ReConnectAsync(_client);
            if (result.State == NetOperateState.OK)
            {
                Console.WriteLine($"reconnect success");
                _state.Value = ConnectionState.Normal;
            }
            else
            {
                Console.WriteLine($"reconnect failure {result.State} {result.Message}");
                _state.Value = ConnectionState.ReconnectErrorClose;
                Dispose();
            }
        }
    }
}
