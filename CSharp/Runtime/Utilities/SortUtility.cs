
namespace UselessFrame.NewRuntime.Utilities
{
    public static class SortUtility
    {
        public static int FloatDownToUp(float a, float b)
        {
            return (a > b) ? 1 : (a < b) ? -1 : 0;
        }

        public static int FloatUpToDown(float a, float b)
        {
            return (a > b) ? -1 : (a < b) ? 1 : 0;
        }

        public static int LongDownToUp(long a, long b)
        {
            return (a > b) ? 1 : (a < b) ? -1 : 0;
        }

        public static int LongUpToDown(long a, long b)
        {
            return (a > b) ? -1 : (a < b) ? 1 : 0;
        }
    }
}
