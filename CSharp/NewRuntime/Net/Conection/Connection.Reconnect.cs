
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Net;
using System.Net.Sockets;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Net.Conection;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private async UniTaskVoid Connect()
        {
            X.SystemLog.Debug("Net", $"request connect -> {_ip}");
            _state.Value = ConnectionState.Connecting;
            RequestConnectResult result = await ConnectionUtility.RequestConnectAsync(_client, _ip, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;

            X.SystemLog.Debug("Net", $"request connect complete -> {_ip}, result {result.State}");
            if (_state.Value == ConnectionState.Connecting)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        _state.Value = ConnectionState.TokenPending;
                        RequestToken().Forget();
                        break;

                    default:
                        X.SystemLog.Debug("Net", $" {Id} connect error {result.State} {result.Message}");
                        _state.Value = ConnectionState.FatalConnect;
                        InnerClose();
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

                default:
                    X.SystemLog.Debug($"state is {_state.Value}, can not reconnect");
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
                X.SystemLog.Debug("Net", $" {Id} reconnect with new failure, because ip is null");
                _state.Value = ConnectionState.ReconnectErrorClose;
                InnerClose();
                return;
            }

            _client.Dispose();
            _client = new TcpClient(AddressFamily.InterNetwork);
            RequestConnectResult result = await ConnectionUtility.RequestConnectAsync(_client, _ip, _closeTokenSource.Token);
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
                X.SystemLog.Debug("Net", $" {Id} reconnect success target {_ip.Address}:{_ip.Port}");
                RequestToken().Forget();
            }
            else
            {
                X.SystemLog.Debug("Net", $" {Id} reconnect failure {result.State} {result.Message}");
                _state.Value = ConnectionState.ReconnectErrorClose;
                InnerClose();
            }
        }

        private async UniTaskVoid RequestToken()
        {
            X.SystemLog.Debug("Net", $" request token {_ip.Address}:{_ip.Port}");
            ReadMessageResult result = await MessageUtility.ReadMessageAsync(_client, _pool, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;

            X.SystemLog.Debug("Net", $" request token complete, {_ip.Address}:{_ip.Port}, state {result.State}");
            if (_state.Value == ConnectionState.TokenPending)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        IMessage msg = result.Bytes.ToMessage();
                        ServerToken token = (ServerToken)msg;
                        _guid = token.GetId();
                        X.SystemLog.Debug("Net", $"client token is {_guid}, verify token is {new Guid(token.RequestToken.Span)}, {_ip}");
                        SendTokenVerify(token.RequestToken);
                        RequestMessage().Forget();
                        Start();
                        break;

                    case NetOperateState.NormalClose:
                        _state.Value = ConnectionState.NormalClose;
                        InnerClose();
                        break;

                    case NetOperateState.InValidRequest:
                    case NetOperateState.Disconnect:
                    case NetOperateState.DataError:
                    case NetOperateState.ParamError:
                    case NetOperateState.PermissionError:
                    case NetOperateState.RemoteClose:
                    case NetOperateState.Unknown:
                        X.SystemLog.Debug("Net", $" {Id} request token error {result.State} {result.StateMessage}");
                        _state.Value = ConnectionState.FatalErrorClose;
                        InnerClose();
                        break;

                    case NetOperateState.SocketError:
                        X.SystemLog.Debug("Net", $" {Id} request token socket error {result.State} {result.StateMessage}");
                        _state.Value = ConnectionState.SocketError;
                        InnerClose();
                        break;

                    default:
                        X.SystemLog.Debug("Net", $" {Id} request token unkown error {result.State} {result.StateMessage}");
                        _state.Value = ConnectionState.FatalErrorClose;
                        InnerClose();
                        break;
                }
            }
        }
    }
}
