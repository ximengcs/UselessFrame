using System;
using UnityEngine;
using UselessFrame.NewRuntime;
using System.Collections.Generic;
using UselessFrame.Runtime;

namespace UselessFrame.UIElements
{
    public class UIModule : ModuleBase, IUIManager
    {
        private Canvas _canvas;
        private GameObject _gameObject;
        private RectTransform _transform;
        private Camera _uiCamera;
        private Dictionary<string, UIGroup> _groups;
        private Dictionary<Type, Dictionary<long, UIHandle>> _handles;

        public Camera Camera => _uiCamera;

        protected override void OnInit(object param)
        {
            base.OnInit(param);
            Initialize((Canvas)param);
        }

        public void Initialize(Canvas canvas)
        {
            _canvas = canvas;
            _uiCamera = _canvas.worldCamera;
            _gameObject = _canvas.gameObject;
            _transform = (RectTransform)_gameObject.transform;
            _groups = new Dictionary<string, UIGroup>();
            _handles = new Dictionary<Type, Dictionary<long, UIHandle>>();
            X.Pool.RegisterCommonHelper<IPoolUI>(new UIPoolHelper(this));
        }

        public IUIHandle OpenUIAsync(Type uiType, string groupName, long id, object userData = null)
        {
            if (!_handles.TryGetValue(uiType, out Dictionary<long, UIHandle> handlesMap))
            {
                handlesMap = new Dictionary<long, UIHandle>();
                _handles.Add(uiType, handlesMap);
            }

            if (handlesMap.TryGetValue(id, out UIHandle uiHandle))
            {
                handlesMap.Remove(id);
                uiHandle.TriggerClose(true);
            }

            uiHandle = X.Pool.Require<UIHandle>(UIHandle.GetPoolKey(uiType), uiType);
            uiHandle.BindId(id);
            handlesMap.Add(id, uiHandle);

            if (!uiHandle.InGroup(groupName))
            {
                UIGroup group = InnerGetOrNewGroup(groupName);
                uiHandle.SetGroup(group);
            }

            uiHandle.TriggerOpen(userData);

            return uiHandle;
        }

        public IUIHandle OpenUIAsync<T>(string groupName, long id, object userData = null) where T : IUI
        {
            Type uiType = typeof(T);
            return OpenUIAsync(uiType, groupName, id, userData);
        }

        public IUIHandle CloseUIAsync(Type uiType, long id, bool destroy = true)
        {
            if (!_handles.TryGetValue(uiType, out Dictionary<long, UIHandle> handlesMap))
            {
                handlesMap = new Dictionary<long, UIHandle>();
                _handles.Add(uiType, handlesMap);
            }
            Debug.Log($"close ui 222 {id} {handlesMap.ContainsKey(id)}");

            if (handlesMap.TryGetValue(id, out UIHandle handle))
            {
                Debug.Log($"Close UI {destroy} {handle.Valid} {id}");
                if (destroy || !handle.Valid)
                {
                    handlesMap.Remove(id);
                }
                handle.TriggerClose(destroy);
            }

            return handle != null ? handle : EmptyUIHandle.Default;
        }

        public IUIHandle CloseUIAsync<T>(long id, bool destroy = true) where T : IUI
        {
            return (IUIHandle)CloseUIAsync(typeof(T), id, destroy);
        }

        public IUIGroup GetOrNewGroup(string groupName)
        {
            return InnerGetOrNewGroup(groupName);
        }

        private UIGroup InnerGetOrNewGroup(string groupName)
        {
            if (_groups.TryGetValue(groupName, out UIGroup group))
            {
                return group;
            }

            GameObject groupRoot = new GameObject(groupName, typeof(RectTransform), typeof(CanvasGroup));
            groupRoot.transform.SetParent(_transform, false);
            group = new UIGroup(this, groupRoot, groupName);
            _groups[groupName] = group;
            return group;
        }

        internal int SetUIGroupLayer(UIGroup group, int layer)
        {
            return SetLayer(_transform, group, layer, InnerGroupLayerChange);
        }

        internal void InnerGroupLayerChange(string nodeName, int index)
        {
            if (_groups.TryGetValue(nodeName, out UIGroup group))
            {
                group.UpdateLayer(index);
            }
        }

        internal static int SetLayer(Transform root, IUINode element, int layer, Action<string, int> onIndexChange)
        {
            if (root.childCount == 0)
                return 0;
            layer = Mathf.Clamp(layer, 0, root.childCount - 1);
            Transform check = root.GetChild(layer);
            if (check.name == element.Name)
                return layer;

            bool find = false;
            int curIndex = 0;
            Transform[] list = new Transform[root.childCount];
            for (int i = 0; i < list.Length; i++, curIndex++)
            {
                Transform child = root.GetChild(i);
                if (!find && child.name == element.Name)
                {
                    find = true;
                    list[layer] = child;
                    if (layer != curIndex)
                        curIndex--;
                }
                else
                {
                    if (layer == curIndex)
                        curIndex++;
                    list[curIndex] = child;
                }
            }

            for (int i = 0; i < list.Length; i++)
            {
                Transform child = list[i];
                if (child.GetSiblingIndex() != i)
                {
                    child.SetSiblingIndex(i);
                    onIndexChange(child.name, i);
                }
            }
            return layer;
        }
    }
}
