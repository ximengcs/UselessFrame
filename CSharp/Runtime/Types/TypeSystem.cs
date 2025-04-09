using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UselessFrame.Runtime.Types
{
    internal class TypeSystem : ITypeSystem
    {
        private IReadOnlyList<Type> _types;
        private IReadOnlyList<Assembly> _assemblys;
        private IReadOnlyDictionary<Type, Attribute[]> _typesAllAttrs;
        private IReadOnlyDictionary<Type, List<Type>> _typesWithAttrs;
        private ConcurrentDictionary<Type, TypeCollection> _classRegister;
        private ConcurrentDictionary<Type, ConstructorInfo[]> _constructors;

        public TypeSystem(ITypeFilter typeFilter)
        {
            InnerInit(typeFilter);
        }

        public object CreateInstance(Type type, params object[] args)
        {
            return InnerCreateInstance(type, args);
        }

        private void InnerInit(ITypeFilter typeFilter)
        {
            var typesAllAttrs = new Dictionary<Type, Attribute[]>();
            var typesWithAttrs = new Dictionary<Type, List<Type>>();
            _typesAllAttrs = typesAllAttrs;
            _typesWithAttrs = typesWithAttrs;
            _constructors = new ConcurrentDictionary<Type, ConstructorInfo[]>();
            _assemblys = AppDomain.CurrentDomain.GetAssemblies();

            string[] assemblyList = typeFilter != null ? typeFilter.AssemblyList : null;
            List<Type> tmpList = new List<Type>(1024);
            foreach (Assembly assembly in _assemblys)
            {
                bool find = false;
                AssemblyName aName = assembly.GetName();
                string assemblyName = aName.Name;
                if (assemblyList != null)
                {
                    foreach (string name in assemblyList)
                    {
                        if (assemblyName == name)
                        {
                            find = true;
                            break;
                        }
                    }
                }
                else
                {
                    find = true;
                }

                if (!find)
                    continue;

                foreach (TypeInfo typeInfo in assembly.DefinedTypes)
                {
                    Type type = typeInfo.AsType();
                    if (typeFilter != null && !typeFilter.CheckType(type))
                        continue;
                    Attribute[] attrs = Attribute.GetCustomAttributes(type);
                    typesAllAttrs.Add(type, attrs);
                    foreach (Attribute attr in attrs)
                    {
                        Type attrType = attr.GetType();
                        if (!typesWithAttrs.TryGetValue(attrType, out List<Type> list))
                        {
                            list = new List<Type>(32);
                            typesWithAttrs.Add(attrType, list);
                        }
                        list.Add(type);
                    }
                    tmpList.Add(type);
                }
            }

            _types = tmpList;
            if (_classRegister != null)
                _classRegister.Clear();
            else
                _classRegister = new ConcurrentDictionary<Type, TypeCollection>();
        }

        private object InnerCreateInstance(Type type, params object[] args)
        {
            object instance = default;
            ConstructorInfo[] ctors;
            if (!_constructors.TryGetValue(type, out ctors))
            {
                ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _constructors.TryAdd(type, ctors);
            }

            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] paramInfos = ctor.GetParameters();
                if (args == null || args.Length == paramInfos.Length)
                {
                    int i = 0;
                    if (args != null)
                    {
                        while (i < paramInfos.Length)
                        {
                            Type argType = args[i].GetType();
                            Type paramType = paramInfos[i].ParameterType;
                            if (argType != paramType && !paramType.IsAssignableFrom(argType))
                            {
                                break;
                            }
                            i++;
                        }
                    }

                    if (i == paramInfos.Length)
                    {
                        instance = ctor.Invoke(args);
                        break;
                    }
                }
            }

            return instance;
        }

        public ITypeCollection GetOrNewWithAttr(Type pType)
        {
            TypeCollection collect;
            if (_classRegister.TryGetValue(pType, out collect))
                return collect;

            collect = new TypeCollection(pType);
            _classRegister.TryAdd(pType, collect);
            foreach (var item in _typesWithAttrs)
            {
                if (item.Key.IsSubclassOf(pType) || item.Key == pType)
                {
                    foreach (Type subType in item.Value)
                    {
                        Attribute attr = GetAttribute(subType, pType);
                        if (attr != null)
                        {
                            collect.Add(subType);
                        }
                    }
                }
            }

            return collect;
        }

        public Attribute GetAttribute(Type classType, Type pType)
        {
            if (_typesAllAttrs.TryGetValue(classType, out Attribute[] values))
            {
                foreach (Attribute attr in values)
                {
                    Type attrType = attr.GetType();
                    if (attrType.IsSubclassOf(pType) || attrType == pType)
                        return attr;
                }
            }
            return default;
        }

        public IReadOnlyList<Attribute> GetAttributes(Type classType)
        {
            if (_typesAllAttrs.TryGetValue(classType, out Attribute[] values))
            {
                return values;
            }

            return default;
        }

        public ITypeCollection GetOrNew(Type baseType)
        {
            TypeCollection collect;
            if (_classRegister.TryGetValue(baseType, out collect))
                return collect;

            collect = new TypeCollection(baseType);
            _classRegister.TryAdd(baseType, collect);
            foreach (Type type in _types)
            {
                if (baseType != type && baseType.IsAssignableFrom(type))
                {
                    collect.Add(type);
                }
            }

            return collect;
        }

    }
}
