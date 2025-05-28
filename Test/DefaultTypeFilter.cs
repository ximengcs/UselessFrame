
using UselessFrame.Runtime.Types;

namespace UselessFrameTest
{
    public class DefaultTypeFilter : ITypeFilter
    {
        public string[] AssemblyList => new string[]
        {
            "UselessFrame",
            "UselessFrameTest"
        };

        public bool CheckType(Type type)
        {
            return true;
        }
    }
}
