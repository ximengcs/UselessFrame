﻿
using System;
using System.Net;
using System.Security;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal struct RequestConnectTcpClientAsyncState
    {
        private TcpClient _client;
        private IPEndPoint _ipEndPoint;
        private AutoResetUniTaskCompletionSource<RequestConnectResult> _completeTaskSource;

        public UniTask<RequestConnectResult> CompleteTask => _completeTaskSource.Task;

        public RequestConnectTcpClientAsyncState(TcpClient client, IPEndPoint ipEndPoint)
        {
            _ipEndPoint = ipEndPoint;
            _completeTaskSource = AutoResetUniTaskCompletionSource<RequestConnectResult>.Create();
            _client = client;
            Begin();
        }

        private void Complete(RequestConnectResult result)
        {
            if (result.State != NetOperateState.OK)
                _client.Dispose();
            _completeTaskSource.TrySetResult(result);
        }

        private void Begin()
        {
            try
            {
                _client.BeginConnect(_ipEndPoint.Address, _ipEndPoint.Port, OnConnect, null);
            }
            catch (ArgumentNullException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect begin param error, exception:{e}"));
            }
            catch (SecurityException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect permission error, exception:{e}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect begin stream closing, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(RequestConnectResult.Create(e, $"[Net]request connect begin socket error"));
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                _client.EndConnect(ar);
                Complete(RequestConnectResult.Create(_client, NetOperateState.OK));
            }
            catch (ArgumentNullException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect begin param error, exception:{e}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect begin stream closing, exception:{e}"));
            }
            catch (InvalidOperationException e)
            {
                Complete(RequestConnectResult.Create(NetOperateState.FatalError, $"[Net]request connect cant end before async operate, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(RequestConnectResult.Create(e, $"[Net]request connect begin socket error"));
            }
        }
    }
}
