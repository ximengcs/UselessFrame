using System;
using System.Threading;

namespace UselessFrame.UIElements
{
    public abstract class UIGroupHelperBase : IUIGroupHelper
    {
        protected IUIGroup _owner;

        void IUIGroupHelper.OnGroupLayerChange()
        {
        }

        bool IUIGroupHelper.MatchType(Type type)
        {
            return MatchType(type);
        }

        void IUIGroupHelper.OnBind(IUIGroup owner)
        {
            _owner = owner;
            OnInit();
        }

        void IUIGroupHelper.OnUnbind(IUIGroup group)
        {
            _owner = null;
            OnDestroy();
        }

        void IUIGroupHelper.OnUpdate(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        void IUIGroupHelper.OnUIClose(IUI ui, Action closeCompleteCbk, CancellationToken cancellation)
        {
            OnUIClose(ui, closeCompleteCbk, cancellation);
        }

        void IUIGroupHelper.OnUIOpen(IUI ui, Action openCompleteCbk, CancellationToken cancellation)
        {
            OnUIOpen(ui, openCompleteCbk, cancellation);
        }

        void IUIGroupHelper.OnUIUpdate(IUI ui, float deltaTime)
        {
            OnUIUpdate(ui, deltaTime);
        }

        protected virtual bool MatchType(Type type) { return true; }
        protected virtual void OnInit() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnDestroy() { }
        protected virtual void OnUIClose(IUI ui, Action closeCompleteCbk, CancellationToken cancellation)
        {
        }
        protected virtual void OnUIDestroy(IUI ui)
        {
        }
        protected virtual void OnUIOpen(IUI ui, Action openCompleteCbk, CancellationToken cancellation)
        {
        }
        protected virtual void OnUIUpdate(IUI ui, float deltaTime)
        {
        }

        protected void SetUIState(IUI ui, UIState state)
        {
            if (ui is IUIGroupElement uiElement)
            {
                uiElement.Handle.State.Value = state;
            }
        }
    }
}
