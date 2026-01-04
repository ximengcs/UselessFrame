using System;

namespace UselessFrameUnity.RedPointSystem
{
    public interface IRedPoint
    {
        string FullName { get; }

        string Name { get; }

        int State { get; }

        IRedPoint Parent { get; }

        void SetTrigger(int changeState);

        void Trigger(int changeState);

        IRedPoint GetChild(string name);

        IRedPoint AddChild(string name);

        IRedPoint AddChild(IRedPoint point);

        void Subscribe(Action<IRedPoint> handler, bool atonceTrigger = true);

        void Unsubscribe(Action<IRedPoint> handler);

        void Unsubscribe();
    }
}
