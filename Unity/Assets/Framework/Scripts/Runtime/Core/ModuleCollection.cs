using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Pools;
using ModuleList = UselessFrame.Runtime.Collections.XList<UselessFrame.Runtime.ModuleBase>;

namespace UselessFrame.Runtime.Collections
{
    internal partial class ModuleCollection : IMultiEnumerable<ModuleBase>
    {
        private Dictionary<Type, Dictionary<int, ModuleBase>> _modules;
        private ModuleList _moduleList;
        private IPool<ModuleList> _listPool;

        public ModuleCollection()
        {
            _moduleList = new ModuleList();
            _modules = new Dictionary<Type, Dictionary<int, ModuleBase>>();
            _listPool = X.Pool.GetOrNew<ModuleList>();
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
            Attribute attr = type.GetCustomAttribute(typeof(ModuleAttribute));
            Type targetType = type;
            if (attr != null)
            {
                ModuleAttribute mAttr = (ModuleAttribute)attr;
                targetType = mAttr.Type;
            }

            if (!_modules.TryGetValue(targetType, out var moduleList))
            {
                moduleList = new Dictionary<int, ModuleBase>();
                _modules[targetType] = moduleList;
            }

            if (!moduleList.ContainsKey(id))
            {
                ModuleBase module = (ModuleBase)X.Type.CreateInstance(type);
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
