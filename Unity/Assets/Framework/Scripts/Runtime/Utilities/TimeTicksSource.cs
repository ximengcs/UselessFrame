using IdGen;
using System;

namespace UselessFrame.NewRuntime.Utilities
{
    public class TimeTicksSource : ITimeSource
    {
        public TimeTicksSource() : this(DateTime.UtcNow.Ticks) { }

        public TimeTicksSource(long timeTicks)
        {
            Epoch = IdGeneratorOptions.DefaultEpoch;
            TickDuration = new TimeSpan(timeTicks);
        }

        public DateTimeOffset Epoch { get; private set; }

        public TimeSpan TickDuration { get; private set; }

        public long GetTicks()
        {
            return TickDuration.Ticks;
        }
    }
}
