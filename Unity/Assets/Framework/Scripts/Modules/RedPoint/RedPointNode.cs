
using System.Collections.Generic;
using System;

namespace UselessFrameUnity.RedPointSystem
{
    internal class RedPointNode : IRedPoint
    {
        private string _fullName;
        private string _name;
        private IRedPoint _parent;
        private int _state;
        private Dictionary<string, IRedPoint> _children;
        private Action<IRedPoint> _changeHandler;

        public int State
        {
            get
            {
                int result = _state;
                foreach (IRedPoint child in Children)
                {
                    result += child.State;
                }
                return result;
            }
        }

        public string FullName => _fullName;

        public string Name => _name;

        public IRedPoint Parent => _parent;

        public IReadOnlyCollection<IRedPoint> Children => _children.Values;

        public RedPointNode(string name, RedPointNode parent)
        {
            _parent = parent;
            _name = name;
            _fullName = parent != null ? $"{parent.FullName}/{name}" : name;
            _children = new Dictionary<string, IRedPoint>();
            if (parent != null)
                parent.AddChild(this);
        }

        public IRedPoint GetChild(string name)
        {
            int index = name.IndexOf('/');
            if (index == -1)
            {
                if (_children.TryGetValue(name, out IRedPoint red))
                    return red;
            }
            else
            {
                string parentName = name.Substring(0, index);
                string suplusName = name.Substring(index + 1);
                if (_children.TryGetValue(parentName, out IRedPoint red))
                {
                    return red.GetChild(suplusName);
                }
            }

            return null;
        }

        public IRedPoint AddChild(string name)
        {
            int index = name.IndexOf('/');
            if (index == -1)
            {
                if (_children.TryGetValue(name, out IRedPoint red))
                    return red;
                else
                    return new RedPointNode(name, this);
            }
            else
            {
                string parentName = name.Substring(0, index);
                string suplusName = name.Substring(index + 1);
                if (!_children.TryGetValue(parentName, out IRedPoint red))
                {
                    red = new RedPointNode(parentName, this);
                }
                return red.AddChild(suplusName);
            }
        }

        public IRedPoint AddChild(IRedPoint point)
        {
            _children.Add(point.Name, point);
            if (point.State > 0)
            {
                IRedPoint pointParent = point.Parent;
                if (pointParent != null)
                {
                    RedPointNode parentNode = (RedPointNode)pointParent;
                    parentNode?.InnerTrigger();
                }
            }
            return point;
        }

        public void Trigger(int changeState)
        {
            _state += changeState;
            InnerTrigger();
        }

        public void SetTrigger(int changeState)
        {
            if (_state == changeState)
                return;
            _state = changeState;
            InnerTrigger();
        }

        private void InnerTrigger()
        {
            _changeHandler?.Invoke(this);
            if (_parent != null)
            {
                RedPointNode parentNode = (RedPointNode)_parent;
                parentNode?.InnerTrigger();
            }
        }

        public void Subscribe(Action<IRedPoint> handler, bool atonceTrigger)
        {
            _changeHandler += handler;
            if (atonceTrigger)
                _changeHandler?.Invoke(this);
        }

        public void Unsubscribe(Action<IRedPoint> handler)
        {
            _changeHandler -= handler;
        }

        public void Unsubscribe()
        {
            _changeHandler = null;
        }
    }
}
