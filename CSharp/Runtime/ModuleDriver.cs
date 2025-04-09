using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime.Types;
using System.Collections.Generic;

namespace UselessFrame.Runtime
{
    internal class ModuleDriver : IModuleDriver
    {
        private bool _start;
        private ModuleCollection _modules;
        private ITypeSystem _typeSys;

        public ITypeSystem TypeSystem => _typeSys;

        public ModuleDriver(ITypeSystem typeSys)
        {
            _typeSys = typeSys;
            _modules = new ModuleCollection(this);
        }

        public void Trigger<T>()
        {

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
                module.OnModuleInit(this, id, param);
                if (_start)
                    await module.OnModuleStart();
            }

            return module;
        }

        public async UniTask RemoveModule(Type type, int id)
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
            _start = true;
        }

        public async UniTask Destroy()
        {
            IEnumerator<ModuleBase> it = _modules.GetBackEnumerator();
            while (it.MoveNext())
            {
                ModuleBase module = it.Current;
                await module.OnModuleDestroy();
            }
        }
    }
}
