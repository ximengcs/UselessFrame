
using System;
using System.Diagnostics;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    internal class GameLoop : ILooper
    {
        private Action<float> _updater;
        private LoopState _state;
        private CancellationToken _exitToken;

        public LoopState State => _state;

        public GameLoop(Action<float> handler, CancellationToken token)
        {
            _state = LoopState.Stop;
            _updater = handler;
            _exitToken = token;
        }

        public void Start()
        {
            switch (_state)
            {
                case LoopState.Stop:
                    Run();
                    break;

                case LoopState.Paused:
                    _state = LoopState.Running;
                    break;
            }
        }

        public void Stop()
        {
            switch (_state)
            {
                case LoopState.Running:
                case LoopState.Paused:
                    _state = LoopState.Stop;
                    break;
            }
        }

        public void Pause()
        {
            switch (_state)
            {
                case LoopState.Running:
                    _state = LoopState.Paused;
                    break;
            }
        }

        public void Continue()
        {
            switch (_state)
            {
                case LoopState.Paused:
                    _state = LoopState.Running;
                    break;
            }
        }

        private bool ExecuteState()
        {
            switch (_state)
            {
                case LoopState.Paused:
                    Thread.Sleep(1);
                    ExecuteState();
                    break;

                case LoopState.Running:
                    return false;

                case LoopState.Stop:
                    return true;
            }

            return false;
        }

        private void Run()
        {
            _state = LoopState.Running;
            float timestampToTicks = FiberUtility.TimestampToTicks;
            long prevTimestamp = Stopwatch.GetTimestamp();

            while (!_exitToken.IsCancellationRequested)
            {
                long currentTimestamp = Stopwatch.GetTimestamp();
                long deltaTicks = (long)((currentTimestamp - prevTimestamp) * timestampToTicks);
                float deltaTime = (float)deltaTicks / TimeSpan.TicksPerSecond;
                _updater(deltaTime);
                prevTimestamp = currentTimestamp;
                Thread.Sleep(1);
                if (ExecuteState())
                    break;
            }
        }
    }
}
