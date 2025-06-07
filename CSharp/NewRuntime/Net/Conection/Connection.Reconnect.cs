
using Cysharp.Threading.Tasks;
using SharpGen.Runtime;
using System;
using System.Net;
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
                case ConnectionState.SocketError:
                    _state.Value = ConnectionState.Reconnect;
                    TryReconnect().Forget();
                    break;

                case ConnectionState.Known:
                case ConnectionState.FatalErrorClose:
                    _state.Value = ConnectionState.Reconnect;
                    TryReconnectWithNew().Forget();
                    break;
            }
        }

        private async UniTaskVoid TryReconnect()
        {
            RequestConnectResult result = await ConnectionUtility.ReConnectAsync(_client);
            if (_closeTokenSource.IsCancellationRequested)
                return;
            HandleReconnectResult(result);
        }

        private async UniTaskVoid TryReconnectWithNew()
        {
            if (_client.Client.RemoteEndPoint == null)
            {
                Console.WriteLine($"reconnect with new failure");
                _state.Value = ConnectionState.ReconnectErrorClose;
                Dispose();
                return;
            }

            IPEndPoint remoteEndPoint = (IPEndPoint)_client.Client.RemoteEndPoint;
            Dispose();
            _pool = new ByteBufferPool();
            RequestConnectResult result = await ConnectionUtility.RequestConnectAsync(remoteEndPoint, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;
            HandleReconnectResult(result);
        }

        private void HandleReconnectResult(RequestConnectResult result)
        {
            if (result.State == NetOperateState.OK)
            {
                Console.WriteLine($"reconnect success");
                _client = result.Remote;
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
