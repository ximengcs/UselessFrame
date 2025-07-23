using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UselessFrame.NewRuntime.Net
{
    public static partial class NetDebugInfo
    {
        private static ConcurrentBag<RecordObject> _objects = new ConcurrentBag<RecordObject>();

        public static void Record(object obj, string msg)
        {
            if (obj == null)
                return;
            _objects.Add(new RecordObject(obj, msg));
        }

        public static IReadOnlyList<RecordObject> GetObject()
        {
            return new List<RecordObject>(_objects);
        }
    }
}
