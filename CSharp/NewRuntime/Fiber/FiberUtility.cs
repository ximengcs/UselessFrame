
using System;
using System.Threading;
using System.Diagnostics;

namespace UselessFrame.NewRuntime.Fiber
{
    public class FiberUtility
    {
        public static void RunLoopSleep1(Action<float> handler, CancellationToken token)
        {
            RunLoop(0, handler, token);
        }

        public static void RunLoopSleep0(Action<float> handler, CancellationToken token)
        {
            RunLoop(-1, handler, token);
        }

        public static void RunLoopFull(Action<float> handler, CancellationToken token)
        {
            RunLoop(-2, handler, token);
        }

        public static void RunLoop30Hz(Action<float> handler, CancellationToken token)
        {
            RunLoop(30, handler, token);
        }

        public static void RunLoop60Hz(Action<float> handler, CancellationToken token)
        {
            RunLoop(60, handler, token);
        }

        private static void RunLoop(int frameRate, Action<float> handler, CancellationToken token)
        {
            double timestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
            long perDeltaTimeStamp = 0;
            if (frameRate > 0)
            {
                long ticksPerFrame = TimeSpan.TicksPerSecond / frameRate;
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

            while (!token.IsCancellationRequested)
            {
                long currentTimestamp = Stopwatch.GetTimestamp();
                long deltaTicks = (long)((currentTimestamp - prevTimestamp) * timestampToTicks);
                float deltaTime = (float)deltaTicks / TimeSpan.TicksPerSecond;
                handler(deltaTime);
                prevTimestamp = currentTimestamp;

                if (frameRate > 0)
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
                    if (frameRate == 0)
                    {
                        Thread.Sleep(1);
                    }
                    else if (frameRate == -1)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        ;
                    }
                }
            }
        }
    }
}
