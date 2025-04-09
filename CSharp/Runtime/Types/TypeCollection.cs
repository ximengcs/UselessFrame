
using System;
using System.Collections;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Types
{
    internal class TypeCollection : ITypeCollection
    {
        private Type _mainType;
        private List<Type> _subClasses;

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
        }

        public void Add(Type subClass)
        {
            _subClasses.Add(subClass);
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _subClasses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
