
using System;
using System.Collections;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Types
{
    internal class TypeCollection : ITypeCollection
    {
        private Type _mainType;
        private List<Type> _subClasses;
        private Dictionary<string, Type> _typesMap;

        public Type MainType => _mainType;

        public int Count => _subClasses.Count;

        public Type this[int index]
        {
            get
            {
                if (index >= 0 && index < _subClasses.Count)
                    return _subClasses[index];
                return default;
            }
        }

        public TypeCollection(Type pType)
        {
            _mainType = pType;
            _subClasses = new List<Type>();
            _typesMap = new Dictionary<string, Type>();
        }

        public Type Get(string typeFullName)
        {
            if (_typesMap.TryGetValue(typeFullName, out Type type))
                return type;
            return default;
        }

        public void Add(Type subClass)
        {
            _subClasses.Add(subClass);
            _typesMap.Add(subClass.FullName, subClass);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _subClasses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type[] ToArray()
        {
            return _subClasses.ToArray();
        }
    }
}
