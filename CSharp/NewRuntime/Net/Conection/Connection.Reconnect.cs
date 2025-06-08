
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Net;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Net.Conection;
using Vortice;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private async UniTaskVoid Connect()
        {
            _state.Value = ConnectionState.Connecting;
            RequestConnectResult result = await ConnectionUtility.RequestConnectAsync(_ip, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;

            if (_state.Value == ConnectionState.Connecting)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        X.SystemLog.Debug("Net", $"connect success target {_ip.Address}:{_ip.Port} {result.State}");
                        _client = result.Remote;
                        _state.Value = ConnectionState.Normal;
                        RequestToken().Forget();
                        break;

                    default:
                        X.SystemLog.Error("Net", $"connect error {result.State} {result.Message}");
                        _state.Value = ConnectionState.FatalConnect;
                        break;
                }
            }
        }

        public void Reconnect()
        {
            switch (_state.Value)
            {
                case ConnectionState.SocketError:
                    _state.Value = ConnectionState.Reconnect;
                    TryReconnect().Forget();
                    break;

                case ConnectionState.UnKnown:
                case ConnectionState.FatalErrorClose:
                    _state.Value = ConnectionState.Reconnect;
                    TryReconnectWithNew().Forget();
                    break;

                case ConnectionState.FatalConnect:
                    Connect().Forget();
                    break;
            }
        }

        private async UniTaskVoid TryReconnect()
        {
            RequestConnectResult result = await ConnectionUtility.ReConnectAsync(_client);
            if (_closeTokenSource.IsCancellationRequested)
                return;
            if (_state.Value == ConnectionState.Reconnect)
                HandleReconnectResult(result);
        }

        private async UniTaskVoid TryReconnectWithNew()
        {
            if (_ip == null)
            {
                X.SystemLog.Error("Net", $"reconnect with new failure, because ip is null");
                _state.Value = ConnectionState.ReconnectErrorClose;
                Dispose();
                return;
            }

            _client.Dispose();
            _client = null;
            RequestConnectResult result = await ConnectionUtility.RequestConnectAsync(_ip, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;
            if (_state.Value == ConnectionState.Reconnect)
                HandleReconnectResult(result);
        }

        private void HandleReconnectResult(RequestConnectResult result)
        {
            if (result.State == NetOperateState.OK)
            {
                _client = result.Remote;
                _ip = (IPEndPoint)_client.Client.RemoteEndPoint;
                X.SystemLog.Debug("Net", $"reconnect success target {_ip.Address}:{_ip.Port}");
                _state.Value = ConnectionState.Normal;
                RequestToken().Forget();
            }
            else
            {
                X.SystemLog.Error("Net", $"reconnect failure {result.State} {result.Message}");
                _state.Value = ConnectionState.ReconnectErrorClose;
                Dispose();
            }
        }

        private async UniTaskVoid RequestToken()
        {
            ReadMessageResult result = await MessageUtility.ReadMessageAsync(_client, _pool, _closeTokenSource.Token);
            if (_state.Value == ConnectionState.Normal)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        IMessage msg = result.Bytes.ToMessage();
                        ServerToken token = (ServerToken)msg;
                        _guid = token.GetId();
                        RequestMessage().Forget();
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
                        X.SystemLog.Error("Net", $"request token error {result.State} {result.StateMessage}");
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
