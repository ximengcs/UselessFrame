using System;
using System.Reflection;
using System.Collections.Generic;
using UselessFrame.Runtime.Types;
using System.Collections.Concurrent;
using System.Linq;

namespace UselessFrame.NewRuntime
{
    internal partial class TypeManager : ITypeManager
    {
        #region Fileds
        private IReadOnlyList<Type> _types;
        private IReadOnlyDictionary<string, Type> _typesDictionary;
        private IReadOnlyList<Assembly> _assemblys;
        private IReadOnlyDictionary<Type, Attribute[]> _typesAllAttrs;
        private IReadOnlyDictionary<Type, List<Type>> _typesWithAttrs;
        private IDictionary<Type, ITypeCollection> _classRegister;
        private IDictionary<Type, ConstructorData[]> _constructors;
        private List<Type> _tmpList;
        #endregion

        #region Property
        public Type this[string fullName]
        {
            get
            {
                if (_typesDictionary.TryGetValue(fullName, out var type))
                    return type;
                return null;
            }
        }

        public IReadOnlyList<Type> Types => _types;
        #endregion

        #region Initialize
        public TypeManager(ITypeFilter typeFilter)
        {
            InnerInit(typeFilter);
        }

        private void InnerInit(ITypeFilter typeFilter)
        {
            var typesAllAttrs = new Dictionary<Type, Attribute[]>();
            var typesWithAttrs = new Dictionary<Type, List<Type>>();
            var typesDictionary = new Dictionary<string, Type>();
            _typesDictionary = typesDictionary;
            _typesAllAttrs = typesAllAttrs;
            _typesWithAttrs = typesWithAttrs;
            _assemblys = AppDomain.CurrentDomain.GetAssemblies();

            string[] excludeList = new string[] { "System", "Microsoft" };

            string[] assemblyList = typeFilter != null ? typeFilter.AssemblyList : null;
            List<Type> tmpList = new List<Type>(1024);
            foreach (Assembly assembly in _assemblys)
            {
                bool find = false;
                AssemblyName aName = assembly.GetName();
                string assemblyName = aName.Name;
                bool skip = false;
                foreach (string excludeName in excludeList)
                {
                    if (assemblyName.StartsWith(excludeName))
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    continue;

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
                    if (!typesDictionary.ContainsKey(type.FullName))
                    {
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
                        typesDictionary.Add(type.FullName, type);
                        tmpList.Add(type);
                    }
                    else
                    {
                        Console.WriteLine($"already addtype {type.FullName} {type.GetHashCode()} {typesDictionary[type.FullName].GetHashCode()}");
                    }
                }
            }

            _types = tmpList;
            _classRegister = new ConcurrentDictionary<Type, ITypeCollection>();
            _constructors = new ConcurrentDictionary<Type, ConstructorData[]>();
        }
        #endregion

        #region Interface
        public bool TryGetType(string fullName, out Type type)
        {
            return _typesDictionary.TryGetValue(fullName, out type);
        }

        public object CreateInstance(Type type, params object[] args)
        {
            return InnerCreateInstance(type, args);
        }

        public ITypeCollection GetCollection(Type type)
        {
            if (type.IsSubclassOf(typeof(Attribute)))
                return GetOrNewWithAttr(type);
            else
                return GetOrNew(type);
        }

        public ITypeCollection GetOrNewWithAttr(Type pType)
        {
            if (_classRegister.TryGetValue(pType, out ITypeCollection result))
                return result;

            TypeCollection collect = new TypeCollection(pType);
            _classRegister.Add(pType, collect);
            if (_typesWithAttrs.TryGetValue(pType, out List<Type> list))
            {
                foreach (Type subType in list)
                    collect.Add(subType);
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
            if (_classRegister.TryGetValue(baseType, out ITypeCollection result))
                return result;

            TypeCollection collect = new TypeCollection(baseType);
            _classRegister.Add(baseType, collect);
            foreach (Type type in _types)
            {
                if (baseType != type && baseType.IsAssignableFrom(type))
                {
                    collect.Add(type);
                }
            }

            return collect;
        }
        #endregion

        #region Implements
        private object InnerCreateInstance(Type type, params object[] args)
        {
            object instance = default;
            ConstructorData[] ctors;
            if (!_constructors.TryGetValue(type, out ctors))
            {
                ConstructorInfo[] ctorInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                ctors = new ConstructorData[ctorInfos.Length];
                for (int i = 0; i <  ctorInfos.Length; i++)
                    ctors[i] = new ConstructorData(ctorInfos[i]);
                _constructors.Add(type, ctors);
            }

            if (_tmpList == null)
                _tmpList = new List<Type>(args.Length);
            else
                _tmpList.Clear();
            if (args != null)
            {
                foreach (object arg in args)
                {
                    if (arg != null)
                        _tmpList.Add(arg.GetType());
                    else
                        _tmpList.Add(null);
                }
            }

            for (int j = 0; j < ctors.Length; j++)
            {
                ConstructorData ctorData = ctors[j];
                if (ctorData.Parameters == null)
                {
                    ctorData.EnstoreParameter();
                    ctors[j] = ctorData;
                }
                ParameterInfo[] paramInfos = ctorData.Parameters;
                if (args == null || args.Length == paramInfos.Length)
                {
                    int i = 0;
                    if (args != null)
                    {
                        while (i < paramInfos.Length)
                        {
                            Type paramType = _tmpList[i];
                            if (paramType != null)
                            {
                                Type paramInfoType = paramInfos[i].ParameterType;
                                if (paramType != paramInfoType && !paramInfoType.IsAssignableFrom(paramType))
                                {
                                    break;
                                }
                                i++;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }

                    if (i == paramInfos.Length)
                    {
                        instance = ctorData.Ctor.Invoke(args);
                        break;
                    }
                }
            }

            return instance;
        }
        #endregion
    }
}
