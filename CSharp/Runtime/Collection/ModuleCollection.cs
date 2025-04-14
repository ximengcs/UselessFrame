using System;
using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;
using ModuleList = UselessFrame.Runtime.Collections.List<UselessFrame.Runtime.ModuleBase>;

namespace UselessFrame.Runtime.Collections
{
    internal partial class ModuleCollection : IMultiEnumerable<ModuleBase>
    {
        private ModuleDriver _driver;
        private Dictionary<Type, Dictionary<int, ModuleBase>> _modules;
        private ModuleList _moduleList;
        private IPool<ModuleList> _listPool;

        public ModuleCollection(ModuleDriver driver)
        {
            _driver = driver;
            _moduleList = new ModuleList();
            _modules = new Dictionary<Type, Dictionary<int, ModuleBase>>();
            _listPool = (IPool<ModuleList>)driver.Core.Pool.GetOrNew(typeof(ModuleList));
        }

        public ModuleBase Get(Type type, int id)
        {
            if (_modules.TryGetValue(type, out var list))
            {
                if (list.TryGetValue(id, out var module))
                {
                    return module;
                }
            }

            return null;
        }

        public ModuleBase Add(Type type, int id)
        {
            if (!_modules.TryGetValue(type, out var moduleList))
            {
                moduleList = new Dictionary<int, ModuleBase>();
                _modules[type] = moduleList;
            }

            if (!moduleList.ContainsKey(id))
            {
                ModuleBase module = (ModuleBase)_driver.Core.TypeSystem.CreateInstance(type);
                moduleList.Add(id, module);
                _moduleList.Add(module);
                return module;
            }
            else
            {
                return null;
            }
        }

        public ModuleBase Remove(Type type, int id)
        {
            if (!_modules.TryGetValue(type, out var moduleList))
            {
                if (moduleList.TryGetValue(id, out var module))
                {
                    moduleList.Remove(id);
                    _moduleList.Remove(module);
                    return module;
                }
            }

            return null;
        }

        public IEnumerator GetEnumerator()
        {
            return new ListEnumerator<ModuleBase>(_moduleList, true, _listPool);
        }

        public IEnumerator<ModuleBase> GetEnumerator(EnumeratorType type)
        {
            switch (type)
            {
                case EnumeratorType.Front:
                    return new ListEnumerator<ModuleBase>(_moduleList, true, _listPool);

                case EnumeratorType.Back:
                    return new ListBackEnumerator<ModuleBase>(_moduleList, true, _listPool);
            }
            throw new NotImplementedException();
        }

        IEnumerator<ModuleBase> IEnumerable<ModuleBase>.GetEnumerator()
        {
            return new ListEnumerator<ModuleBase>(_moduleList, true, _listPool);
        }

        IEnumerator IMultiEnumerable.GetEnumerator(EnumeratorType type)
        {
            return GetEnumerator(type);
        }
    }
}
