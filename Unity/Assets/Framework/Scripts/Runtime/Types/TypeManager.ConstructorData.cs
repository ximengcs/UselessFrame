using System.Reflection;

namespace UselessFrame.NewRuntime
{
    internal partial class TypeManager
    {
        public struct ConstructorData
        {
            public ConstructorInfo Ctor;
            public ParameterInfo[] Parameters;

            public ConstructorData(ConstructorInfo ctor)
            {
                Ctor = ctor;
                Parameters = null;
            }

            public void EnstoreParameter()
            {
                Parameters = Ctor.GetParameters();
            }
        }
    }
}
