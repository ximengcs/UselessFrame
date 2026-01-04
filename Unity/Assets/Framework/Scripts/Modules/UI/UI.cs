
using UnityEngine;
using UnityXFrame.Core.UIElements;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Collections;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.UIElements
{
    public class UI : IUI, IPoolUI, IUIGroupElement, IUIGameObjectBinder
    {
        private IContainer<IUI> _container;
        private UIHandle _handle;
        private UIGroup _uiGroup;
        private int _layer;
        private RectTransform _mainRect;

        protected GameObject _gameObject;
        protected RectTransform _transform;

        #region IUI Interface
        public IUIHandle Handle => _handle;

        public RectTransform RootRect => _transform;

        public virtual RectTransform MainRect => _mainRect;

        public int Layer
        {
            get => _layer;
            set => _uiGroup.SetUILayer(this, value);
        }

        public IUIGroup Group => _uiGroup;

        public IUIHandle Open()
        {
            _uiGroup.OpenUI(this);
            return _handle;
        }

        public IUIHandle Close(bool destroy = true)
        {
            _uiGroup.CloseUI(this);
            return _handle;
        }
        #endregion

        #region Container Interface
        public IUI Owner => _container.Owner;

        public long Id => _container.Id;

        public IDataProvider Data => _container.Data;

        public IContainer<IUI> Root => _container.Root;

        public IContainer<IUI> Parent => _container.Parent;

        public void Trigger<T>() where T : IContainerEventHandler
        {
            _container.Trigger<T>();
        }

        public IContainer<IUI> AddCom()
        {
            return _container.AddCom();
        }

        public T AddCom<T>() where T : IContainer<IUI>
        {
            return _container.AddCom<T>();
        }

        public void RemoveCom(IContainer<IUI> child)
        {
            _container.RemoveCom(child);
        }

        public T GetCom<T>(long id = default) where T : IContainer<IUI>
        {
            return _container.GetCom<T>(id);
        }
        #endregion

        #region IUIGameObjectBinder
        void IUIGameObjectBinder.BindGameObject(GameObject gameObject)
        {
            IUINode node = this;
            _gameObject = gameObject;
            _gameObject.name = node.Name;
            _transform = gameObject.GetComponent<RectTransform>();
            _mainRect = _transform.Find("Rect")?.GetComponent<RectTransform>() ?? null;
        }
        #endregion

        #region IUIGroupElement Function
        UIHandle IUIGroupElement.Handle => _handle;

        string IUINode.Name => $"{GetType().Name}{GetHashCode()}";

        void IUIGroupElement.OnSetLayer(int layer)
        {
            _layer = layer;
        }

        void IUIGroupElement.OnBindHandle(long id, UIHandle handle)
        {
            _container = Container<IUI>.Create(this, id);
            _handle = handle;
        }

        void IUIGroupElement.OnInit(object userData)
        {
            Debug.Log($"owner {_container.Owner == null}");
            _container.AddCom<UIComFinder>();
            OnInit(userData);
        }

        void IUIGroupElement.OnOpen()
        {
            OnOpen();
        }

        void IUIGroupElement.OnClose()
        {
            OnClose();
        }

        void IUIGroupElement.OnUpdate()
        {
            OnUpdate();
        }

        void IUIGroupElement.OnGroupChange()
        {
            OnGroupChange();
        }

        void IUIGroupElement.OnSetGroup(IUIGroup group)
        {
            _uiGroup = (UIGroup)group;
            if (_uiGroup != null)
            {
                _transform.SetParent(_uiGroup.Root);
                _transform.anchoredPosition3D = Vector3.zero;
                _transform.localScale = Vector2.one;
                _transform.localRotation = Quaternion.identity;
            }
        }
        #endregion

        #region IPoolUI
        int IPoolObject.PoolKey { get; }

        IPool IPoolObject.InPool { get; set; }

        void IPoolObject.OnCreate(object param)
        {
        }

        void IPoolObject.OnRequest()
        {
        }

        void IPoolObject.OnRelease()
        {
        }

        void IPoolObject.OnDelete()
        {
        }
        #endregion

        #region Child Function
        protected virtual void OnInit(object userData) { }

        protected virtual void OnOpen() { }

        protected virtual void OnClose() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnGroupChange() { }
        #endregion
    }
}
