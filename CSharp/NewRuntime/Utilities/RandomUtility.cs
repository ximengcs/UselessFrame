
using System;

namespace UselessFrame.NewRuntime.Utilities
{
    public static class RandomUtility
    {
        private static Random _random;

        internal static void Initialize(int seed)
        {
            _random = new Random(seed);
        }

        public static int NextInt()
        {
            return _random.Next();
        }
    }
}
