using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Runtime.Observable
{
    internal partial class EventToFiberEnumSubject<OwnerT, EnumT> : ISubject<OwnerT, EnumT> where EnumT : System.Enum
    {
        private EnumT _value;
        private OwnerT _owner;
        private IFiber _eventFiber;

        private Action<EnumT> _changeEvent;
        private Action<EnumT, EnumT> _changeEventWithOldValue;
        private Action<OwnerT, EnumT> _changeEventWithOwner;
        private Action<OwnerT, EnumT, EnumT> _changeEventWithOwnerAndOldValue;

        private ConcurrentQueue<StateObject> _pool;

        public EnumT Value
        {
            get => _value;

            set
            {
                EnumT old = _value;
                if (!_value.Equals(value))
                {
                    _value = value;
                    _eventFiber.Post(Notify, CreateState(old, value));
                }
            }
        }

        public EventToFiberEnumSubject(OwnerT owner, EnumT value, IFiber eventFiber)
        {
            _eventFiber = eventFiber;
            _owner = owner;
            _value = value;
            _pool = new ConcurrentQueue<StateObject>();
        }

        private StateObject CreateState(EnumT oldValue, EnumT newValue)
        {
            if (_pool.TryDequeue(out StateObject obj))
            {
                obj.oldValue = oldValue;
                obj.newValue = newValue;
                obj.changeEvent = _changeEvent;
                obj.changeEventWithOwner = _changeEventWithOwner;
                obj.changeEventWithOldValue = _changeEventWithOldValue;
                obj.changeEventWithOwnerAndOldValue = _changeEventWithOwnerAndOldValue;
            }
            else
            {
                obj = new StateObject(oldValue, newValue, _changeEvent, _changeEventWithOldValue, _changeEventWithOwner, _changeEventWithOwnerAndOldValue);
            }
            return obj;
        }

        private void Notify(object state)
        {
            StateObject stateObject = (StateObject)state;
            EnumT newValue = stateObject.newValue;
            EnumT oldValue = stateObject.oldValue;

            stateObject.changeEvent?.Invoke(newValue);
            stateObject.changeEventWithOldValue?.Invoke(oldValue, newValue);
            stateObject.changeEventWithOwner?.Invoke(_owner, newValue);
            stateObject.changeEventWithOwnerAndOldValue?.Invoke(_owner, oldValue, newValue);

            _pool.Enqueue(stateObject);
        }

        public void Subscribe(Action<OwnerT, EnumT> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOwner += changeHandler;
            if (onceTrigger)
            {
                StateObject obj = new StateObject(_value, _value, null, null, changeHandler, null);
                _eventFiber.Post(Notify, obj);
            }
        }

        public void Subscribe(Action<OwnerT, EnumT, EnumT> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOwnerAndOldValue += changeHandler;
            if (onceTrigger)
            {
                StateObject obj = new StateObject(_value, _value, null, null, null, changeHandler);
                _eventFiber.Post(Notify, obj);
            }
        }

        public void Subscribe(Action<EnumT> changeHandler, bool onceTrigger = false)
        {
            _changeEvent += changeHandler;
            if (onceTrigger)
            {
                StateObject obj = new StateObject(_value, _value, changeHandler, null, null, null);
                _eventFiber.Post(Notify, obj);
            }
        }

        public void Subscribe(Action<EnumT, EnumT> changeHandler, bool onceTrigger = false)
        {
            _changeEventWithOldValue += changeHandler;
            if (onceTrigger)
            {
                StateObject obj = new StateObject(_value, _value, null, changeHandler, null, null);
                _eventFiber.Post(Notify, obj);
            }
        }

        public void Unsubscribe(Action<EnumT> changeHandler)
        {
            _changeEvent -= changeHandler;
        }

        public void Unsubscribe(Action<EnumT, EnumT> changeHandler)
        {
            _changeEventWithOldValue -= changeHandler;
        }

        public void Unsubscribe(Action<OwnerT, EnumT> changeHandler)
        {
            _changeEventWithOwner -= changeHandler;
        }

        public void Unsubscribe(Action<OwnerT, EnumT, EnumT> changeHandler)
        {
            _changeEventWithOwnerAndOldValue -= changeHandler;
        }
    }
}
