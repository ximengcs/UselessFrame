
using System;
using System.Threading;

namespace UselessFrame.UIElements
{
    public interface IUIGroupHelper
    {
        bool MatchType(Type type);

        void OnBind(IUIGroup group);

        void OnUnbind(IUIGroup group);

        void OnUpdate(float deltaTime);

        void OnUIOpen(IUI ui, Action openCompleteCbk, CancellationToken cancellation);

        void OnUIClose(IUI ui, Action closeCompleteCbk, CancellationToken cancellation);

        void OnUIUpdate(IUI ui, float deltaTime);

        void OnGroupLayerChange();
    }
}
