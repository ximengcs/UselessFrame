using System;

namespace UselessFrame.Runtime.Observable
{
    internal partial class EventToFiberEnumSubject<OwnerT, EnumT>
    {
        private class StateObject
        {
            public EnumT oldValue;
            public EnumT newValue;
            public Action<EnumT> changeEvent;
            public Action<EnumT, EnumT> changeEventWithOldValue;
            public Action<OwnerT, EnumT> changeEventWithOwner;
            public Action<OwnerT, EnumT, EnumT> changeEventWithOwnerAndOldValue;

            public StateObject(EnumT oldValue, EnumT newValue, Action<EnumT> changeEvent, Action<EnumT, EnumT> changeEventWithOldValue, Action<OwnerT, EnumT> changeEventWithOwner, Action<OwnerT, EnumT, EnumT> changeEventWithOwnerAndOldValue)
            {
                this.oldValue=oldValue;
                this.newValue=newValue;
                this.changeEvent=changeEvent;
                this.changeEventWithOldValue=changeEventWithOldValue;
                this.changeEventWithOwner=changeEventWithOwner;
                this.changeEventWithOwnerAndOldValue=changeEventWithOwnerAndOldValue;
            }
        }
    }
}
