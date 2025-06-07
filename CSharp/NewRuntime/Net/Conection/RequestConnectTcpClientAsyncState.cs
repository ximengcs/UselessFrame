
using System;
using System.Net;
using System.Security;
using System.Threading;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Net.Conection;

namespace UselessFrame.Net
{
    internal struct RequestConnectTcpClientAsyncState
    {
        private TcpClient _client;
        private IPEndPoint _ipEndPoint;
        private CancellationToken _cancelToken;
        private AutoResetUniTaskCompletionSource<RequestConnectResult> _completeTaskSource;

        public UniTask<RequestConnectResult> CompleteTask => _completeTaskSource.Task;

        public RequestConnectTcpClientAsyncState(IPEndPoint ipEndPoint, CancellationToken cancelToken)
        {
            _ipEndPoint = ipEndPoint;
            _cancelToken = cancelToken;
            _completeTaskSource = AutoResetUniTaskCompletionSource<RequestConnectResult>.Create();
            _client = new TcpClient();
            Begin();
        }

        private void Complete(RequestConnectResult result)
        {
            _completeTaskSource.TrySetResult(result);
            if (result.State != NetOperateState.OK)
                _client.Dispose();
            _client = null;
        }

        private void Begin()
        {
            try
            {
                _client.BeginConnect(_ipEndPoint.Address, _ipEndPoint.Port, OnConnect, null);
            }
            catch (ArgumentNullException e)
            {
                Complete(new RequestConnectResult(NetOperateState.ParamError, $"[Net]request connect begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(new RequestConnectResult(NetOperateState.ParamError, $"[Net]request connect begin param error, exception:{e}"));
            }
            catch (SecurityException e)
            {
                Complete(new RequestConnectResult(NetOperateState.PermissionError, $"[Net]request connect permission error, exception:{e}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new RequestConnectResult(NetOperateState.Disconnect, $"[Net]request connect begin stream closing, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new RequestConnectResult(NetOperateState.SocketError, $"[Net]request connect begin socket error exception:{e}"));
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            if (_cancelToken.IsCancellationRequested)
            {
                Complete(new RequestConnectResult(NetOperateState.Cancel, "[Net]connect cancel."));
                return;
            }

            try
            {
                _client.EndConnect(ar);
                Complete(new RequestConnectResult(_client, NetOperateState.OK));
            }
            catch (ArgumentNullException e)
            {
                Complete(new RequestConnectResult(NetOperateState.ParamError, $"[Net]request connect begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(new RequestConnectResult(NetOperateState.ParamError, $"[Net]request connect begin param error, exception:{e}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new RequestConnectResult(NetOperateState.Disconnect, $"[Net]request connect begin stream closing, exception:{e}"));
            }
            catch (InvalidOperationException e)
            {
                Complete(new RequestConnectResult(NetOperateState.Unknown, $"[Net]request connect cant end before async operate, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new RequestConnectResult(NetOperateState.SocketError, $"[Net]request connect begin socket error exception:{e}"));
            }
        }
    }
}
