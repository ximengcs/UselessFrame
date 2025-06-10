using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TestIMGUI.Core;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    public class Server
    {
        private IPEndPoint _host;
        private TcpListener _tcpListener;
        private CancellationTokenSource _closeTokenSource;
        private ConcurrentDictionary<Guid, Connection> _connections;
        private EventToFiberEnumSubject<Server, ServerState> _state;
        private IFiber _dataFiber;

        private int _acceptErrorRetryTimes;

        public Action<MessageResult> OnReceiveMessage;

        public IPEndPoint Host => _host;

        public ISubject<Server, ServerState> State => _state;

        public Server(int port, IFiber dataFiber)
        {
            _acceptErrorRetryTimes = 0;

            _dataFiber = dataFiber;
            _connections = new ConcurrentDictionary<Guid, Connection>();
            _closeTokenSource = new CancellationTokenSource();
            _host = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            _tcpListener = new TcpListener(_host);
            try
            {
                _tcpListener.Start();
                X.SystemLog.Debug("Net", $"server start");
                _state = new EventToFiberEnumSubject<Server, ServerState>(this, ServerState.Normal, dataFiber);
            }
            catch (SocketException e)
            {
                X.SystemLog.Debug("Net", $"server start error happen, {e}");
                _state = new EventToFiberEnumSubject<Server, ServerState>(this, ServerState.SocketError, dataFiber);
                _tcpListener.Stop();
                return;
            }

            Run().Forget();
        }

        public void GetConnections(List<Connection> toList)
        {
            toList.AddRange(_connections.Values);
        }

        public Connection GetConnection(Guid guid)
        {
            return _connections.TryGetValue(guid, out Connection connection) ? connection : null;
        }

        public void Remove(Connection connection)
        {
            if (connection == null)
                return;

            if (_connections.TryRemove(connection.Id, out _))
            {
                X.SystemLog.Debug("Net", $"remove connection {connection.Id}, current count {_connections.Count}");
            }
        }

        public void Close()
        {
            X.SystemLog.Debug("Net", $"close server {_host.Address}:{_host.Port}");
            _state.Value = ServerState.NormalClose;
            Dispose();
        }

        private void Dispose()
        {
            _closeTokenSource.Cancel();
            List<Connection> connections = new List<Connection>(_connections.Values);
            foreach (Connection connection in connections)
                connection.InnerClose();
            _tcpListener.Stop();
            _connections.Clear();
            _tcpListener = null;
            _connections = null;
        }

        private async UniTaskVoid Run()
        {
            if (_state.Value != ServerState.Normal)
                return;

            X.SystemLog.Debug("Net", $"ready accept {_host.Address}:{_host.Port}");
            AcceptConnectResult result = await ConnectionUtility.AcceptConnectAsync(_tcpListener, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;

            X.SystemLog.Debug("Net", $"find new client, result : {result.State}, server : {_host.Address}:{_host.Port} {_state.Value} ");
            if (_state.Value == ServerState.Normal)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        CreateNewConnect(result).Forget();
                        Run().Forget();
                        break;

                    case NetOperateState.InValidRequest:
                        X.SystemLog.Debug("Net", $"request invalid {result.State} {result.Message}");
                        Run().Forget();
                        break;

                    case NetOperateState.SocketError:
                        X.SystemLog.Debug("Net", $"accept error {result.State} {result.Message}");
                        if (_acceptErrorRetryTimes > 0)
                        {
                            X.SystemLog.Debug("Net", $"server will retry accept ({_acceptErrorRetryTimes}), {result.State} {result.Message}");
                            _acceptErrorRetryTimes--;
                            Run().Forget();
                        }
                        else
                        {
                            X.SystemLog.Debug("Net", $"server retry times is ({_acceptErrorRetryTimes}), will close, {result.State} {result.Message}");
                            _state.Value = ServerState.SocketError;
                            Dispose();
                        }
                        break;

                    case NetOperateState.Disconnect:
                    case NetOperateState.Cancel:
                        X.SystemLog.Debug("Net", $"accept unkown error {result.State} {result.Message}, will close");
                        _state.Value = ServerState.FatalErrorClose;
                        Dispose();
                        break;
                }
            }
        }

        private async UniTaskVoid CreateNewConnect(AcceptConnectResult result)
        {
            Connection connect = new Connection(Guid.NewGuid(), result.Client, _dataFiber);
            ServerToken token = NetUtility.CreateToken(connect.Id);
            X.SystemLog.Debug("Net", $"create new client {connect.Id} {new Guid(token.Id.Span)}");
            ServerTokenVerify tokenVerify = await connect.SendWait<ServerTokenVerify>(token);
            if (tokenVerify != null)
            {
                X.SystemLog.Debug("Net", $"verify success {connect.Id} {new Guid(token.Id.Span)}");
                connect.OnReceiveMessage += PostMessage;
                connect.State.Subscribe(ConnectStateHandler, true);
                _connections.TryAdd(connect.Id, connect);
                connect.Start();
            }
            else
            {
                X.SystemLog.Debug("Net", $"tokenVerify error -> {_host.Address}:{_host.Port}");
                connect.InnerClose();
            }
        }

        private void PostMessage(MessageResult message)
        {
            OnReceiveMessage?.Invoke(message);
        }

        private void ConnectStateHandler(Connection connect, ConnectionState state)
        {
            X.SystemLog.Debug("Net", $"target state change, state {state}, {connect.LocalIP.Address}:{connect.LocalIP.Port} -> {connect.RemoteIP.Address}:{connect.RemoteIP.Port} {connect.Id}");

            switch (state)
            {
                case ConnectionState.FatalConnect:
                case ConnectionState.NormalClose:
                case ConnectionState.FatalErrorClose:
                case ConnectionState.SocketError:
                    Remove(connect);
                    break;
            }
        }
    }
}
