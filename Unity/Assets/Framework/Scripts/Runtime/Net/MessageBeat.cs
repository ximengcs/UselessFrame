using System;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;
using static UselessFrame.Net.Connection;

namespace UselessFrame.Net
{
    internal class MessageBeat : IFiberLoopItem
    {
        private MessageStream _stream;
        private IFiber _fiber;
        private bool _disposed;
        private bool _running;
        private bool _pending;
        private float _timeGap;
        private float _time;
        private KeepAlive _keepaliveMsg;

        private Action<WriteMessageResult> _onError;

        public event Action<WriteMessageResult> ErrorEvent
        {
            add => _onError += value;
            remove => _onError -= value;
        }

        public MessageBeat(IFiber fiber, MessageStream stream, float timeGap)
        {
            _fiber = fiber;
            _stream = stream;
            _timeGap = timeGap;
            _time = 0;
            _keepaliveMsg = new KeepAlive();
            _fiber.Add(this);
        }

        public void Start()
        {
            if (_running) return;
            _running = true;
            Reset();
        }

        public void Stop()
        {
            if (!_running) return;
            _running = false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }

        public bool MoveNext()
        {
            if (!_pending && _running)
            {
                _time -= _fiber.DeltaTime;
                if (_time <= 0)
                {
                    SendMessage().Forget();
                    Reset();
                }
            }

            return !_disposed;
        }

        private void Reset()
        {
            _time = _timeGap;
        }

        private async UniTask SendMessage()
        {
            _pending = true;
            WriteMessageResult result = await _stream.Send(_keepaliveMsg, true);
            if (result.State != NetOperateState.OK)
            {
                _onError?.Invoke(result);
                Stop();
            }
            _pending = false;
        }
    }
}
