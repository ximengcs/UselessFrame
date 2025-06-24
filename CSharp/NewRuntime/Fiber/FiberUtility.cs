
using System;
using System.Diagnostics;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public class FiberUtility
    {
        public static void RunLoop(int frameRate, Action<float> handler, CancellationToken token)
        {
            double timestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
            long ticksPerFrame = TimeSpan.TicksPerSecond / frameRate;
            long perDeltaTimeStamp = (long)(ticksPerFrame / timestampToTicks);

            long prevTimestamp = Stopwatch.GetTimestamp();
            while (!token.IsCancellationRequested)
            {
                long currentTimestamp = Stopwatch.GetTimestamp();
                long deltaTicks = (long)((currentTimestamp - prevTimestamp) * timestampToTicks);
                float deltaTime = (float)deltaTicks / TimeSpan.TicksPerSecond;
                prevTimestamp = currentTimestamp;

                handler(deltaTime);

                SpinWait spin = new SpinWait();
                long targetTicks = currentTimestamp + perDeltaTimeStamp;
                while (Stopwatch.GetTimestamp() < targetTicks)
                {
                    long remainingTicks = (long)((targetTicks - Stopwatch.GetTimestamp()) * timestampToTicks);
                    if (remainingTicks > 4_000_000)
                        Thread.Sleep(1);
                    else if (remainingTicks > 50_000)
                        Thread.Sleep(0);
                    else
                        spin.SpinOnce();
                }
            }
        }
    }
}
