﻿using System;
using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Runtime.Collections
{
    internal partial class ModuleCollection
    {
        private ModuleDriver _driver;
        private Dictionary<Type, Dictionary<int, ModuleBase>> _modules;
        private List<ModuleBase> _moduleList;
        private PurePool<List<ModuleBase>> _listPool;

        public ModuleCollection(ModuleDriver driver)
        {
            _driver = driver;
            _moduleList = new List<ModuleBase>();
            _modules = new Dictionary<Type, Dictionary<int, ModuleBase>>();
            _listPool = new PurePool<List<ModuleBase>>();
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
            return new Enumerator(_listPool, _moduleList);
        }

        public IEnumerator<ModuleBase> GetBackEnumerator()
        {
            return new BackEnumerator(_listPool, _moduleList);
        }
    }
}
