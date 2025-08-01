
using IdGen;
using System;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime
{
    public class Container
    {
        private long _id;
        private IdGenerator _idGenerator;
        private Container _parent;
        private Container _root;
        private IDataProvider _data;
        private Dictionary<Type, Dictionary<long, Container>> _childrenWithType;
        private List<Container> _children;

        public long Id => _id;

        public IDataProvider Data => _root._data;

        public Container Root => _root;

        public Container Parent => _parent;

        private Container()
        {
            _children = new List<Container>();
            _childrenWithType = new Dictionary<Type, Dictionary<long, Container>>();
        }

        public static Container Create(IDataProvider dataProvider)
        {
            if (dataProvider == null)
                dataProvider = new DataProvider();

            Container container = new Container();
            container.InitRoot();
            container._data = dataProvider;
            return container;
        }

        private void InitRoot()
        {
            ITimeSource timeSource = new TimeTicksSource();
            _idGenerator = new IdGenerator(0, new IdGeneratorOptions(timeSource: timeSource));
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

        public Container AddChild()
        {
            Container container = new Container();
            InnerInitChild(container);
            return container;
        }

        public T AddChild<T>() where T : Container
        {
            T container = (T)X.Type.CreateInstance(typeof(T));
            InnerInitChild(container);
            return container;
        }

        public void RemoveChild(Container child)
        {
            _children.Remove(child);
            if (_childrenWithType.TryGetValue(child.GetType(), out Dictionary<long, Container> map))
            {
                map.Remove(child.Id);
            }
            InnerRecursiveDestory(child);
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
            child._id = _root._idGenerator.CreateId();
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
