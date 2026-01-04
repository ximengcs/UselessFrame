using UnityEngine;
using UselessFrame.UIElements;
using System.Collections.Generic;
using UselessFrame.NewRuntime;

namespace UnityXFrame.Core.UIElements
{
    public class UIComFinder : Container<IUI>
    {
        private const string UI_PREFIX = "[C]";
        private Dictionary<string, RectTransform> m_ComsRoot;

        protected override void OnInit()
        {
            base.OnInit();
            m_ComsRoot = new Dictionary<string, RectTransform>();
            RectTransform root = Owner.RootRect;
            m_ComsRoot.Add(root.name, root);
            InnerFindUIComponent(root);
        }

        public GameObject GetObject(string name)
        {
            if (m_ComsRoot.TryGetValue(name, out RectTransform tf))
                return tf.gameObject;
            else
                return default;
        }

        public T GetUI<T>(string name) where T : Component
        {
            name = $"{UI_PREFIX}{name}";
            if (m_ComsRoot.TryGetValue(name, out RectTransform tf))
                return tf.GetComponent<T>();
            else
                return default;
        }

        private void InnerFindUIComponent(Transform tf)
        {
            InnerCheckTf(tf);
            foreach (Transform child in tf)
                InnerFindUIComponent(child);
        }

        private void InnerCheckTf(Transform tf)
        {
            if (tf.name.StartsWith(UI_PREFIX))
                m_ComsRoot.Add(tf.name, tf.GetComponent<RectTransform>());
        }
    }
}
