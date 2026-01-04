
using UselessFrame.NewRuntime;
using UselessFrame.Runtime;
using UselessFrame.Runtime.Types;
using UselessFrameUnity.RedPointSystem;

namespace UselessFrameUnity
{
    public static partial class Framework
    {
        private static IRedPointModule _redPoint;

        public static IRedPointModule RedPoint => _redPoint ??= (IRedPointModule)X.Module.Get(typeof(RedPointModule));
    }
}
