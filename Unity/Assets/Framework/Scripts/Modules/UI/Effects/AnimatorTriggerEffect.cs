using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityXFrame.Core.Diagnotics;
using UnityXFrameLib.Animations;
using UselessFrame.NewRuntime;
using UselessFrame.UIElements;

namespace UnityXFrameLib.UIElements
{
    public class AnimatorTriggerEffect : AnimatorChecker, IUIGroupHelperEffect
    {
        private int m_Layer;
        private string m_Trigger;
        private string m_StateName;

        public AnimatorTriggerEffect(string trigger, int layer = 0)
        {
            m_Trigger = trigger;
            m_StateName = trigger;
            m_Layer = layer;
        }

        public AnimatorTriggerEffect(string trigger, string stateName, int layer = 0)
        {
            m_Trigger = trigger;
            m_StateName = stateName;
            m_Layer = layer;
        }

        void IUIGroupHelperEffect.OnUpdate()
        {
            UpdateState();
        }

        public async UniTask<bool> Do(IUI ui, CancellationToken token)
        {
            Animator animator = ui.RootRect.GetComponent<Animator>();
            if (animator != null && animator.enabled)
            {
                animator.SetTrigger(m_Trigger);
                IHandle handle = Add(animator, m_StateName, m_Layer);
                await handle.FinishTask;
                return !token.IsCancellationRequested;
            }
            else
            {
                X.Log.Debug(LogSort.UI, $"UI {ui.GetType().Name} do not has animtor component, will use other effect");
                return false;
            }
        }

        public bool Kill(IUI ui)
        {
            Animator animator = ui.RootRect.GetComponent<Animator>();
            if (animator != null && animator.enabled)
            {
                animator.ResetTrigger(m_Trigger);
                Remove(animator);
                return true;
            }
            else
            {
                X.Log.Debug(LogSort.UI, $"UI {ui.GetType().Name} do not has animtor component, will use other effect");
                return false;
            }
        }
    }
}
