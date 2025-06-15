
using System;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Runtime.Observable
{
    public class ValueSubject<OwnerT, T> : ISubject<OwnerT, T>
    {
        private class EventInfo
        {
            private OwnerT _owner;
            private T _oldValue;
            private T _newValue;
            private Action<T> _changeEvent;
            private Action<T, T> _changeEventWithOldValue;
            private Action<OwnerT, T> _changeEventWithOwner;
            private Action<OwnerT, T, T> _changeEventWithOwnerAndOldValue;

            public EventInfo(ValueSubject<OwnerT, T> owner, T oldValue, T newValue)
            {
                _owner = owner._owner;
                _oldValue = oldValue;
                _newValue = newValue;
                _changeEvent = owner._changeEvent;
                _changeEventWithOldValue = owner._changeEventWithOldValue;
                _changeEventWithOwner = owner._changeEventWithOwner;
                _changeEventWithOwnerAndOldValue = owner._changeEventWithOwnerAndOldValue;
            }

            public void Trigger()
            {
                _changeEvent?.Invoke(_newValue);
                _changeEventWithOldValue?.Invoke(_oldValue, _newValue);
                _changeEventWithOwner?.Invoke(_owner, _newValue);
                _changeEventWithOwnerAndOldValue?.Invoke(_owner, _oldValue, _newValue);
            }
        }

        private T _value;
        private OwnerT _owner;
        private IFiber _eventFiber;

        private Action<T> _changeEvent;
        private Action<T, T> _changeEventWithOldValue;
        private Action<OwnerT, T> _changeEventWithOwner;
        private Action<OwnerT, T, T> _changeEventWithOwnerAndOldValue;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                T oldValue = _value;
                _value = value;

                _eventFiber.Post(TriggerEventToFiber, new EventInfo(this, oldValue, _value));
            }
        }

        public ValueSubject(OwnerT owner, IFiber eventFiber, T value = default)
        {
            _owner = owner;
            _value = value;
            _eventFiber = eventFiber;
        }

        private void TriggerEventToFiber(object state)
        {
            EventInfo evtInfo = (EventInfo)state;
            evtInfo.Trigger();
        }

        public void Subscribe(Action<OwnerT, T> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOwner += changeHandler;
            if (onceTrigger)
                changeHandler(_owner, Value);
        }

        public void Subscribe(Action<OwnerT, T, T> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOwnerAndOldValue += changeHandler;
            if (onceTrigger)
            {
                T value = Value;
                changeHandler(_owner, value, value);
            }
        }

        public void Subscribe(Action<T> changeHandler, bool onceTrigger = false)
        {
            _changeEvent += changeHandler;
            if (onceTrigger)
                changeHandler(Value);
        }

        public void Subscribe(Action<T, T> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOldValue += changeHandler;
            if (onceTrigger)
            {
                T value = Value;
                changeHandler(value, value);
            }
        }

        public void Unsubscribe(Action<T> changeHandler)
        {
            _changeEvent -= changeHandler;
        }

        public void Unsubscribe(Action<T, T> changeHandler)
        {
            _changeEventWithOldValue -= changeHandler;
        }

        public void Unsubscribe(Action<OwnerT, T> changeHandler)
        {
            _changeEventWithOwner -= changeHandler;
        }

        public void Unsubscribe(Action<OwnerT, T, T> changeHandler)
        {
            _changeEventWithOwnerAndOldValue -= changeHandler;
        }
    }
}
