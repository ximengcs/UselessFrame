using System;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Runtime
{
    public class ModuleDriver : IModuleDriver
    {
        private bool _start;
        private ModuleCollection _modules;

        public ModuleDriver()
        {
            _modules = new ModuleCollection();
        }

        public IModule GetModule(Type type, int id)
        {
            return _modules.Get(type, id);
        }

        public async UniTask<IModule> AddModule(Type type, object param)
        {
            int id = default;
            Attribute attr = type.GetCustomAttribute(typeof(ModuleAttribute));
            if (attr != null)
            {
                ModuleAttribute mAttr = (ModuleAttribute)attr;
                id = mAttr.Id;
            }

            ModuleBase module = _modules.Add(type, id);
            if (module != null)
            {
                module.OnInit(this, id, param);
                if (_start)
                    await module.OnModuleStart();
            }

            return module;
        }

        public async UniTask Remove(Type type, int id)
        {
            ModuleBase module = _modules.Remove(type, id);
            if (module)
            {
                await module.OnModuleDestroy();
            }
        }

        public async UniTask Start()
        {
            foreach (ModuleBase module in _modules)
            {
                await module.OnModuleStart();
            }
        }

        public async UniTask Destroy()
        {
            foreach (ModuleBase module in _modules)
            {
                await module.OnModuleDestroy();
            }
        }
    }
}
