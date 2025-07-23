
using System;
using System.Threading;
using System.Diagnostics;

namespace UselessFrame.NewRuntime.Fiber
{
    internal class PirceseGameLoop : ILooper
    {
        private int _frameRate;
        private Action<float> _updater;
        private LoopState _state;
        private CancellationToken _exitToken;

        public LoopState State => _state;

        public PirceseGameLoop(int frameRate, Action<float> handler, CancellationToken token)
        {
            _state = LoopState.Stop;
            _frameRate = frameRate;
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
            double timestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
            long perDeltaTimeStamp = 0;
            if (_frameRate > 0)
            {
                long ticksPerFrame = TimeSpan.TicksPerSecond / _frameRate;
                perDeltaTimeStamp = (long)(ticksPerFrame / timestampToTicks);
            }
            long prevTimestamp = Stopwatch.GetTimestamp();

            long sleep1Ticks = 0;
            long sleep2Ticks = 0;
            long sleep1TicksCount = 0;
            long sleep2TicksCount = 0;
            long sleep1MaxGap = 0;
            long sleep2MaxGap = 0;
            long sleep1Dir = 1;
            long sleep2Dir = 1;

            while (!_exitToken.IsCancellationRequested)
            {
                long currentTimestamp = Stopwatch.GetTimestamp();
                long deltaTicks = (long)((currentTimestamp - prevTimestamp) * timestampToTicks);
                float deltaTime = (float)deltaTicks / TimeSpan.TicksPerSecond;
                _updater(deltaTime);
                prevTimestamp = currentTimestamp;

                if (_frameRate > 0)
                {
                    long targetTicks = currentTimestamp + perDeltaTimeStamp;
                    long timeStamp = Stopwatch.GetTimestamp();
                    while (timeStamp < targetTicks)
                    {
                        long remainingTicks = (long)((targetTicks - timeStamp) * timestampToTicks);
                        if (remainingTicks > (sleep1Ticks - sleep1MaxGap * sleep1Dir))
                        {
                            Thread.Sleep(1);
                            long newRemainingTicks = Stopwatch.GetTimestamp() - timeStamp;
                            long gap = Math.Abs(newRemainingTicks - sleep1Ticks);
                            long newGap = (sleep1MaxGap * sleep1TicksCount + gap) / (sleep1TicksCount + 1);
                            if (sleep1Dir == 1)
                            {
                                if (Math.Abs(newGap - sleep1MaxGap) / (double)sleep1MaxGap <= 0.001)
                                    sleep1Dir = -1;
                            }
                            sleep1MaxGap = newGap;

                            sleep1Ticks = (sleep1Ticks * sleep1TicksCount + newRemainingTicks) / (sleep1TicksCount + 1);
                            sleep1TicksCount++;
                        }
                        else if (remainingTicks > (sleep2Ticks - sleep2MaxGap * sleep2Dir))
                        {
                            Thread.Sleep(0);
                            long newRemainingTicks = Stopwatch.GetTimestamp() - timeStamp;
                            long gap = Math.Abs(newRemainingTicks - sleep2Ticks);

                            long newGap = (sleep2MaxGap * sleep2TicksCount + gap) / (sleep2TicksCount + 1);
                            if (sleep2Dir == 1)
                            {
                                if (Math.Abs(newGap - sleep2MaxGap) / (double)sleep2MaxGap <= 0.001)
                                    sleep2Dir = -1;
                            }
                            sleep2MaxGap = newGap;

                            sleep2Ticks = (sleep2Ticks * sleep2TicksCount + newRemainingTicks) / (sleep2TicksCount + 1);
                            sleep2TicksCount++;
                        }
                        else
                        {
                            ;
                        }
                        timeStamp = Stopwatch.GetTimestamp();
                    }
                }
                else
                {
                    if (_frameRate == 0)
                    {
                        Thread.Sleep(1);
                    }
                    else if (_frameRate == -1)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        ;
                    }
                }

                if (ExecuteState())
                    break;
            }
        }
    }
}
