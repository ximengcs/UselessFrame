
using System;

namespace UselessFrame.Runtime.Observable
{
    public interface ISubject<OwnerT, T>
    {
        T Value { get; set; }

        void Subscribe(Action<OwnerT, T> changeHandler, bool onceTrigger = false);

        void Subscribe(Action<OwnerT, T, T> changeHandler, bool onceTrigger = false);

        void Subscribe(Action<T> changeHandler, bool onceTrigger = false);

        void Subscribe(Action<T, T> changeHandler, bool onceTrigger = false);

        void Unsubscribe(Action<T> changeHandler);

        void Unsubscribe(Action<T, T> changeHandler);

        void Unsubscribe(Action<OwnerT, T> changeHandler);

        void Unsubscribe(Action<OwnerT, T, T> changeHandler);
    }
}
