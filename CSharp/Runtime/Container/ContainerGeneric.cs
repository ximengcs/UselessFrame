
using IdGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime
{
    public class Container<OwnerT> : IContainer<OwnerT>
    {
        private OwnerT _owner;
        private long _id;
        private Container<OwnerT> _parent;
        private Container<OwnerT> _root;
        private IDataProvider _data;
        private Dictionary<Type, Dictionary<long, Container<OwnerT>>> _childrenWithType;
        private List<Container<OwnerT>> _children;

        public OwnerT Owner => _owner;

        public long Id => _id;

        public IDataProvider Data => _root._data;

        public IContainer<OwnerT> Root => _root;

        public IContainer<OwnerT> Parent => _parent;

        protected Container()
        {
            _children = new List<Container<OwnerT>>();
            _childrenWithType = new Dictionary<Type, Dictionary<long, Container<OwnerT>>>();
        }

        private static IdGenerator _IdGen;

        public static IContainer<OwnerT> Create(OwnerT owner, IDataProvider dataProvider = null)
        {
            if (_IdGen == null)
            {
                ITimeSource timeSource = new TimeTicksSource();
                _IdGen = new IdGenerator(0, new IdGeneratorOptions(timeSource: timeSource));
            }
            if (dataProvider == null)
                dataProvider = new DataProvider();

            Container<OwnerT> container = new Container<OwnerT>();
            container._owner = owner;
            container.InitRoot();
            container._data = dataProvider;
            container._id = _IdGen.CreateId();
            return container;
        }

        private void InitRoot()
        {
            _root = this;
            OnInit();
        }

        public void Trigger<T>() where T : IContainerEventHandler
        {
            if (this is T handler)
                handler.OnTrigger();

            foreach (Container<OwnerT> child in _children)
                child.Trigger<T>();
        }

        public T GetCom<T>(long id = default) where T : IContainer<OwnerT>
        {
            if (_childrenWithType.TryGetValue(typeof(T), out var children))
            {
                if (id == default && children.Count > 0)
                    return (T)(IContainer<OwnerT>)children.First().Value;

                if (children.TryGetValue(id, out var result))
                    return (T)(IContainer<OwnerT>)result;
            }
            return default(T);
        }

        public IContainer<OwnerT> AddCom()
        {
            Container<OwnerT> container = new Container<OwnerT>();
            InnerInitChild(container);
            return container;
        }

        public T AddCom<T>() where T : IContainer<OwnerT>
        {
            IContainer<OwnerT> container = (IContainer<OwnerT>)X.Type.CreateInstance(typeof(T));
            InnerInitChild((Container<OwnerT>)container);
            return (T)container;
        }

        public void RemoveCom(IContainer<OwnerT> child)
        {
            Container<OwnerT> orgChild = (Container<OwnerT>)child;
            _children.Remove(orgChild);
            if (_childrenWithType.TryGetValue(child.GetType(), out Dictionary<long, Container<OwnerT>> map))
            {
                map.Remove(child.Id);
            }
            InnerRecursiveDestory(orgChild);
        }

        private void InnerRecursiveDestory(Container<OwnerT> container)
        {
            container.OnDestroy();
            foreach (Container<OwnerT> child in _children)
            {
                InnerRecursiveDestory(child);
            }
        }

        private void InnerInitChild(Container<OwnerT> child)
        {
            Type childType = child.GetType();
            child._root = _root;
            child._id = _IdGen.CreateId();
            child._owner = _owner;
            child._parent = this;
            _children.Add(child);
            if (!_childrenWithType.TryGetValue(childType, out Dictionary<long, Container<OwnerT>> map))
            {
                map = new Dictionary<long, Container<OwnerT>>();
                _childrenWithType[childType] = map;
            }
            map.Add(child._id, child);
            child.OnInit();
        }

        protected virtual void OnInit() { }

        protected virtual void OnDestroy() { }
    }
}
