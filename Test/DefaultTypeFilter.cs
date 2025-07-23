
using UselessFrame.Runtime.Types;

namespace UselessFrameTest
{
    public class DefaultTypeFilter : ITypeFilter
    {
        public bool CheckAssembly(string assemblyName)
        {
            throw new NotImplementedException();
        }

        public bool CheckType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
