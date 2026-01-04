
using UselessFrame.Runtime.Collections;

namespace UselessFrame.Runtime
{
    internal struct ModuleHandle
    {
        public IModuleHandler Handler;
        public XList<IModule> Modules;

        public ModuleHandle(IModuleHandler handler, XList<IModule> modules)
        {
            Handler = handler;
            Modules = modules;
        }
    }
}
