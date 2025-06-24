
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public class FiberUtility
    {
        public static void RunLoop1(Action<float> handler, CancellationToken token)
        {
            RunLoop(0, handler, token);
        }

        public static void RunLoop0(Action<float> handler, CancellationToken token)
        {
            RunLoop(-1, handler, token);
        }

        public static void RunLoop30(Action<float> handler, CancellationToken token)
        {
            RunLoop(30, handler, token);
        }

        public static void RunLoop60(Action<float> handler, CancellationToken token)
        {
            RunLoop(60, handler, token);
        }

        public static void RunLoop(int frameRate, Action<float> handler, CancellationToken token)
        {
            double timestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
            long perDeltaTimeStamp = 0;
            if (frameRate > 0)
            {
                long ticksPerFrame = TimeSpan.TicksPerSecond / frameRate;
                perDeltaTimeStamp = (long)(ticksPerFrame / timestampToTicks);
            }
            long prevTimestamp = Stopwatch.GetTimestamp();
            while (!token.IsCancellationRequested)
            {
                long currentTimestamp = Stopwatch.GetTimestamp();
                long deltaTicks = (long)((currentTimestamp - prevTimestamp) * timestampToTicks);
                float deltaTime = (float)deltaTicks / TimeSpan.TicksPerSecond;
                prevTimestamp = currentTimestamp;

                handler(deltaTime);

                if (frameRate > 0)
                {
                    long targetTicks = currentTimestamp + perDeltaTimeStamp;
                    while (Stopwatch.GetTimestamp() < targetTicks)
                    {
                        long remainingTicks = (long)((targetTicks - Stopwatch.GetTimestamp()) * timestampToTicks);
                        if (remainingTicks > 4_000_000)
                            Thread.Sleep(1);
                        else if (remainingTicks > 30_000)
                            Thread.Sleep(0);
                    }
                }
                else
                {
                    if (frameRate == 0)
                        Thread.Sleep(1);
                    else
                        Thread.Sleep(0);
                }
            }
        }
    }
}
