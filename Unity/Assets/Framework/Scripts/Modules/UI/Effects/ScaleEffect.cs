
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UselessFrame.UIElements;

namespace UnityXFrameLib.UIElements
{
    /// <summary>
    /// 缩放效果
    /// </summary>
    public class ScaleEffect : IUIGroupHelperEffect
    {
        private float m_Dur;
        private Vector3 m_Start;
        private Vector3 m_Target;
        private Dictionary<int, Tween> m_Anims;

        public ScaleEffect(Vector2 target, float duration = 0.2f)
        {
            m_Dur = duration;
            m_Start = new Vector3(0, 0, 1);
            m_Target = target;
            m_Target.z = m_Start.z;
            m_Anims = new Dictionary<int, Tween>();
        }

        public ScaleEffect(Vector2 start, Vector2 target, float duration = 0.2f)
        {
            m_Dur = duration;
            m_Start = start;
            m_Target = target;
            m_Start.z = 1;
            m_Target.z = m_Start.z;
            m_Anims = new Dictionary<int, Tween>();
        }

        void IUIGroupHelperEffect.OnUpdate()
        {

        }

        public async UniTask<bool> Do(IUI ui, CancellationToken token)
        {
            int key = ui.GetHashCode();
            ui.MainRect.localScale = m_Start;
            Tween tween = ui.MainRect.DOScale(m_Target, m_Dur);
            m_Anims.Add(key, tween);
            AutoResetUniTaskCompletionSource taskSource = AutoResetUniTaskCompletionSource.Create();
            tween.OnComplete(() => taskSource.TrySetResult());
            await taskSource.Task;
            m_Anims.Remove(key);
            return !token.IsCancellationRequested;
        }

        public bool Kill(IUI ui)
        {
            int key = ui.GetHashCode();
            if (m_Anims.TryGetValue(key, out Tween tween))
            {
                tween.Kill();
                m_Anims.Remove(key);
            }
            return true;
        }
    }
}
