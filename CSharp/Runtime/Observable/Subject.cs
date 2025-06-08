
using System;

namespace UselessFrame.Runtime.Observable
{
    public class Subject<OwnerT, T>
    {
        private T _value;
        private OwnerT _owner;
        private Func<T> _getter;
        private Action<T> _setter;

        private Action<T> _changeEvent;
        private Action<T, T> _changeEventWithOldValue;
        private Action<OwnerT, T> _changeEventWithOwner;
        private Action<OwnerT, T, T> _changeEventWithOwnerAndOldValue;

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
                {
                    _setter(value);
                    _value = Value;
                }
                else
                {
                    _value = value;
                }

                _changeEvent?.Invoke(_value);
                _changeEventWithOldValue?.Invoke(oldValue, _value);
                _changeEventWithOwner?.Invoke(_owner, _value);
                _changeEventWithOwnerAndOldValue?.Invoke(_owner, oldValue, _value);
            }
        }

        public Subject(OwnerT owner, Func<T> getter, Action<T> setter)
        {
            _owner = owner;
            _getter = getter;
            _setter = setter;
            _value = _getter();
        }

        public Subject(OwnerT owner, T value = default)
        {
            _owner = owner;
            _value = value;
            _getter = null;
            _setter = null;
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
