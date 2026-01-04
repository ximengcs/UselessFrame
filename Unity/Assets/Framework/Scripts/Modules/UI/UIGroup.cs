using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UselessFrame.NewRuntime;

namespace UselessFrame.UIElements
{
    internal class UIGroup : IUIGroup, IUINode
    {
        private int _layer;
        private bool _isOpen;
        private string _name;
        private UIModule _module;
        private GameObject _gameObject;
        private CanvasGroup _canvasGroup;
        private RectTransform _transform;
        private List<IUIGroupHelper> _helpers;
        private List<IUIGroupElement> _uiList;
        private Dictionary<string, IUIGroupElement> _elementsMap;

        private List<IUIGroupElement> _uiCacheList;
        private bool _listDirty;

        public IUIManager Manager => _module;

        public RectTransform Root => _transform;

        public string Name => _name;

        public int Count => _uiList.Count;

        public bool IsOpen => _isOpen;

        public int Layer
        {
            get => _layer;
            set
            {
                _module.SetUIGroupLayer(this, value);
            }
        }

        public UIGroup(UIModule uiModule, GameObject root, string name)
        {
            _name = name;
            _module = uiModule;
            _gameObject = root;
            _canvasGroup = _gameObject.GetComponent<CanvasGroup>();
            Canvas canvas = _gameObject.AddComponent<Canvas>();
            canvas.vertexColorAlwaysGammaSpace = true;
            _gameObject.AddComponent<GraphicRaycaster>();
            _transform = (RectTransform)root.transform;
            _helpers = new List<IUIGroupHelper>();
            _uiList = new List<IUIGroupElement>();
            _elementsMap = new Dictionary<string, IUIGroupElement>();
            _uiCacheList = new List<IUIGroupElement>();

            _transform.anchorMin = Vector2.zero;
            _transform.anchorMax = Vector2.one;
            _transform.offsetMin = Vector2.zero;
            _transform.offsetMax = Vector2.zero;

            _transform.localScale = Vector3.one;
            _transform.localRotation = Quaternion.identity;

            _listDirty = false;
            InnerOpen();
        }

        public T AddHelper<T>() where T : IUIGroupHelper
        {
            T helper = (T)X.Type.CreateInstance(typeof(T));
            _helpers.Add(helper);
            helper.OnBind(this);
            return helper;
        }

        public void RemoveHelper(IUIGroupHelper helper)
        {
            if (_helpers.Contains(helper))
            {
                _helpers.Remove(helper);
                helper.OnUnbind(this);
            }
        }

        public void SetUILayer(IUIGroupElement ui, int layer)
        {
            int curLayer = ui.Layer;
            if (curLayer == layer)
                return;

            UIModule.SetLayer(Root, this, layer, InnerUIIndexChange);
        }

        internal void InnerUIIndexChange(string name, int index)
        {
            if (_elementsMap.TryGetValue(name, out IUIGroupElement element))
            {
                element.OnSetLayer(index);
            }
        }

        internal void UpdateLayer(int layer)
        {
            _layer = layer;
        }

        public void Open()
        {
            if (_isOpen) return;
            InnerOpen();
        }

        private void InnerOpen()
        {
            _isOpen = true;

            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        public void Close()
        {
            if (!_isOpen) return;
            InnerClose();
        }

        private void InnerClose()
        {
            _isOpen = false;

            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void AddUI(IUIGroupElement ui)
        {
            _uiList.Add(ui);
            _elementsMap.Add(ui.Name, ui);
            UIGroup originGroup = ui.Group as UIGroup;
            if (originGroup != null)
            {
                originGroup._uiList.Remove(ui);
                originGroup._elementsMap.Remove(ui.Name);
            }
            ui.OnSetGroup(this);
            ui.OnSetLayer(_uiList.Count - 1);
            ui.OnGroupChange();
            _listDirty = true;
        }

        public void RemoveUI(IUIGroupElement ui)
        {
            if (_elementsMap.ContainsKey(ui.Name))
            {
                _uiList.Remove(ui);
                _elementsMap.Remove(ui.Name);
                UIGroup originGroup = ui.Group as UIGroup;
                if (originGroup != null)
                    originGroup._uiList.Remove(ui);
                ui.OnGroupChange();
                _listDirty = true;
            }
        }

        public void OpenUI(IUIGroupElement ui)
        {
            UIHandle uiHandle = ui.Handle;
            switch (uiHandle.State.Value)
            {
                case UIState.Ready:
                case UIState.Loading:
                case UIState.Loaded:
                case UIState.OpenBegin:
                case UIState.Open:
                case UIState.OpenEnd:
                    return;
            }

            bool handle = false;
            ui.Handle.State.Value = UIState.OpenBegin;
            if (_helpers != null && _helpers.Count > 0)
            {
                foreach (IUIGroupHelper helper in _helpers)
                {
                    if (helper.MatchType(ui.GetType()))
                    {
                        helper.OnUIOpen(ui, uiHandle.OpenFinish, uiHandle.OpenToken);
                        handle = true;
                        break;
                    }
                }
            }

            if (!handle)
            {
                uiHandle.OpenFinish();
            }
        }

        public void CloseUI(IUIGroupElement ui)
        {
            UIHandle uiHandle = ui.Handle;
            switch (uiHandle.State.Value)
            {
                case UIState.Ready:
                case UIState.Loading:
                case UIState.Loaded:
                case UIState.CloseBegin:
                case UIState.Close:
                case UIState.CloseEnd:
                    return;
            }
            Debug.Log($"[State] {uiHandle.State.Value}");
            bool handle = false;
            uiHandle.State.Value = UIState.CloseBegin;
            if (_helpers != null && _helpers.Count > 0)
            {
                foreach (IUIGroupHelper helper in _helpers)
                {
                    if (helper.MatchType(ui.GetType()))
                    {
                        helper.OnUIClose(ui, uiHandle.CloseFinish, uiHandle.CloseToken);
                        handle = true;
                        break;
                    }
                }
            }

            if (!handle)
            {
                uiHandle.CloseFinish();
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_listDirty)
            {
                _uiCacheList.Clear();
                _uiCacheList.AddRange(_uiList);
            }

            foreach (IUIGroupElement ui in _uiCacheList)
            {
                if (_helpers != null && _helpers.Count >= 0)
                {
                    foreach (IUIGroupHelper helper in _helpers)
                    {
                        if (helper.MatchType(ui.GetType()))
                        {
                            helper.OnUIUpdate(ui, deltaTime);
                            break;
                        }
                    }
                }
                if (ui.Handle.State.Value == UIState.OpenEnd)
                    ui.OnUpdate();
            }
        }
    }
}
