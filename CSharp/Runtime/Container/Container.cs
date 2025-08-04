
using IdGen;
using System;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime
{
    public class Container : IContainer
    {
        private long _id;
        private Container _parent;
        private Container _root;
        private IDataProvider _data;
        private Dictionary<Type, Dictionary<long, Container>> _childrenWithType;
        private List<Container> _children;

        public long Id => _id;

        public IDataProvider Data => _root._data;

        public IContainer Root => _root;

        public IContainer Parent => _parent;

        private Container()
        {
            _children = new List<Container>();
            _childrenWithType = new Dictionary<Type, Dictionary<long, Container>>();
        }

        private static IdGenerator _IdGen;

        public static Container Create(IDataProvider dataProvider = null)
        {
            if (_IdGen == null)
            {
                ITimeSource timeSource = new TimeTicksSource();
                _IdGen = new IdGenerator(0, new IdGeneratorOptions(timeSource: timeSource));
            }
            if (dataProvider == null)
                dataProvider = new DataProvider();

            Container container = new Container();
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

            foreach (Container child in _children)
                child.Trigger<T>();
        }

        public IContainer AddCom()
        {
            Container container = new Container();
            InnerInitChild(container);
            return container;
        }

        public T AddCom<T>() where T : IContainer
        {
            IContainer container = (IContainer)X.Type.CreateInstance(typeof(T));
            InnerInitChild((Container)container);
            return (T)container;
        }

        public void RemoveCom(IContainer child)
        {
            Container orgChild = (Container)child;
            _children.Remove(orgChild);
            if (_childrenWithType.TryGetValue(child.GetType(), out Dictionary<long, Container> map))
            {
                map.Remove(child.Id);
            }
            InnerRecursiveDestory(orgChild);
        }

        private void InnerRecursiveDestory(Container container)
        {
            container.OnDestroy();
            foreach (Container child in _children)
            {
                InnerRecursiveDestory(child);
            }
        }

        private void InnerInitChild(Container child)
        {
            child._root = _root;
            child._id = _IdGen.CreateId();
            _children.Add(child);
            if (!_childrenWithType.TryGetValue(typeof(Container), out Dictionary<long, Container> map))
            {
                map = new Dictionary<long, Container>();
                _childrenWithType[typeof(Container)] = map;
            }
            map.Add(child._id, child);
            child.OnInit();
        }

        protected virtual void OnInit() { }

        protected virtual void OnDestroy() { }
    }
}
