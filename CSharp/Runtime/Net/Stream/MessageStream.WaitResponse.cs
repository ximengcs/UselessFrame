﻿
using System;
using Google.Protobuf;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;
using UselessFrame.NewRuntime;
using System.Threading;
using UselessFrame.NewRuntime.Utilities;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class MessageStream
        {
            public struct WaitResponseHandle
            {
                private CancellationTokenSource _cancellationTokenSource;
                private AutoResetUniTaskCompletionSource<ReadMessageResult> _responseTaskSource;

                public readonly int Id;
                public readonly bool HasResponse;

                public UniTask<ReadMessageResult> ResponseTask => _responseTaskSource.Task;

                public WaitResponseHandle(IMessage requestMessage)
                {
                    Id = X.Random.NextInt();
                    HasResponse = true;
                    MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(requestMessage);
                    typeInfo.SetRequestToken(requestMessage, Id);
                    _responseTaskSource = AutoResetUniTaskCompletionSource<ReadMessageResult>.Create();
                    _cancellationTokenSource = new CancellationTokenSource();
                    WaitTimeout().Forget();
                }

                private async UniTask WaitTimeout()
                {
                    await UniTaskExt.Delay(60, _cancellationTokenSource.Token);
                    if (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _responseTaskSource.TrySetResult(ReadMessageResult.Create(NetOperateState.Timeout, "timeout"));
                        _cancellationTokenSource.Cancel();
                    }
                }

                public static WaitResponseHandle CreateEmpty()
                {
                    WaitResponseHandle handle = new WaitResponseHandle();
                    return handle;
                }

                public void SetResponse(ReadMessageResult messageResult)
                {
                    if (!HasResponse)
                        return;
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;
                    _responseTaskSource.TrySetResult(messageResult);
                    _cancellationTokenSource.Cancel();
                }

                public void SetCancel()
                {
                    if (!HasResponse)
                        return;
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;
                    _responseTaskSource.TrySetResult(ReadMessageResult.Create(NetOperateState.Cancel, "cancel"));
                    _cancellationTokenSource.Cancel();
                }
            }
        }
    }

}
