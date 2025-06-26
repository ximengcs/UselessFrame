
using System;

namespace UselessFrame.NewRuntime.Net
{
    public static partial class NetDebugInfo
    {
        public class RecordObject
        {
            private WeakReference _ref;
            private string _headInfo;
            private string _msg;

            public bool IsAlive => _ref.IsAlive;

            public string HeadInfo => _headInfo;

            public string Msg => _msg;

            public RecordObject(object obj, string msg)
            {
                _ref = new WeakReference(obj, false);
                _headInfo = $"{obj.GetType().Name}|{obj.GetHashCode()}";
                _msg = msg;
            }

            public override string ToString()
            {
                return $"[{_headInfo}][{_msg}]";
            }
        }
    }
}
