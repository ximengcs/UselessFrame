
using System;

namespace UselessFrame.Runtime.Observable
{
    public class Subject<T>
    {
        private T _value;
        private object _owner;
        private Func<T> _getter;
        private Action<T> _setter;

        private Action<T> _changeEvent;
        private Action<T, T> _changeEventWithOldValue;
        private Action<object, T> _changeEventWithOwner;
        private Action<object, T, T> _changeEventWithOwnerAndOldValue;

        public T Value
        {
            get
            {
                if (_getter != null)
                    return _getter();
                return _value;
            }
            set
            {
                T oldValue = _value;
                if (_setter != null)
                    _setter(value);
                _value = Value;
                _changeEvent?.Invoke(_value);
                _changeEventWithOldValue?.Invoke(oldValue, _value);
                _changeEventWithOwner?.Invoke(_owner, _value);
                _changeEventWithOwnerAndOldValue?.Invoke(_owner, oldValue, _value);
            }
        }

        public Subject(object owner, Func<T> getter, Action<T> setter)
        {
            _owner = owner;
            _getter = getter;
            _setter = setter;
            _value = _getter();
        }

        public Subject(object owner, T value = default)
        {
            _owner = owner;
            _value = default;
            _getter = null;
            _setter = null;
        }

        public void Subscribe(Action<object, T> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOwner += changeHandler;
            if (onceTrigger)
                changeHandler(_owner, Value);
        }

        public void Subscribe(Action<object, T, T> changeHandler, bool onceTrigger = false)
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

        public void Unsubscribe(Action<object, T> changeHandler)
        {
            _changeEventWithOwner -= changeHandler;
        }

        public void Unsubscribe(Action<object, T, T> changeHandler)
        {
            _changeEventWithOwnerAndOldValue -= changeHandler;
        }
    }
}
