using System;
using System.Net;
using TestIMGUI.Core;
using Google.Protobuf;
using System.Threading;
using System.Net.Sockets;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    public class Server
    {
        private IPEndPoint _host;
        private TcpListener _tcpListener;
        private CancellationTokenSource _closeTokenSource;
        private Dictionary<Guid, Connection> _connections;
        private List<Connection> _connectionsList;
        private EventToFiberEnumSubject<Server, ServerState> _state;
        private IFiber _dataFiber;

        public Action<MessageResult> OnReceiveMessage;

        public IPEndPoint Host => _host;

        public ISubject<Server, ServerState> State => _state;

        public IEnumerable<Connection> Connections
        {
            get
            {
                _connectionsList.Clear();
                _connectionsList.AddRange(_connections.Values);
                return _connectionsList;
            }
        }

        public Server(int port, IFiber dataFiber)
        {
            _dataFiber = dataFiber;
            _connectionsList = new List<Connection>();
            _connections = new Dictionary<Guid, Connection>();
            _closeTokenSource = new CancellationTokenSource();
            _host = new IPEndPoint(ConnectionUtility.GetLocalIPAddress(), port);
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
            }

            Run().Forget();
        }

        public void Remove(Connection connection)
        {
            if (_connections.ContainsKey(connection.Id))
                _connections.Remove(connection.Id);
        }

        public void Close()
        {
            X.SystemLog.Debug("Net", $"close server {_host.Address}:{_host.Port}");
            _state.Value = ServerState.NormalClose;
            Dispose();
        }

        private void Dispose()
        {
            foreach (Connection connection in _connections.Values)
                connection.Close().Forget();
            _closeTokenSource.Cancel();
            _tcpListener.Stop();
            _tcpListener = null;
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
                        _state.Value = ServerState.SocketError;
                        break;

                    case NetOperateState.DataError:
                    case NetOperateState.PermissionError:
                    case NetOperateState.Unknown:
                        X.SystemLog.Debug("Net", $"accept unkown error {result.State} {result.Message}");
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
                _connections.Add(connect.Id, connect);
                connect.Start();
            }
            else
            {
                X.SystemLog.Debug("Net", $"tokenVerify error -> {_host.Address}:{_host.Port}");
            }
        }

        private void PostMessage(MessageResult message)
        {
            OnReceiveMessage.Invoke(message);
        }

        private void ConnectStateHandler(Connection connect, ConnectionState state)
        {
            X.SystemLog.Debug("Net", $"target state change, state {state}, {connect.LocalIP.Address}:{connect.LocalIP.Port} -> {connect.RemoteIP.Address}:{connect.RemoteIP.Port} {connect.Id}");
        }
    }
}
