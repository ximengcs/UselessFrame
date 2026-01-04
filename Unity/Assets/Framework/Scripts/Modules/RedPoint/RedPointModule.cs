using UselessFrame.Runtime;
using System.Collections.Generic;
using UselessFrameUnity.Attributes;

namespace UselessFrameUnity.RedPointSystem
{
    [FrameModule]
    public class RedPointModule : ModuleBase, IRedPointModule
    {
        private const string GLOBAL = nameof(GLOBAL);
        private Dictionary<string, IRedPoint> _points;

        protected override void OnInit(object param)
        {
            base.OnInit(param);
            _points = new Dictionary<string, IRedPoint>();
        }

        public IRedPoint GetOrAdd(string path)
        {
            IRedPoint red = Get(GLOBAL, path);
            if (red == null)
                red = Add(GLOBAL, path);
            return red;
        }

        public IRedPoint Get(string path)
        {
            return Get(GLOBAL, path);
        }

        public IRedPoint Get(string sysName, string path)
        {
            path = $"{sysName}/{path}";
            if (_points.TryGetValue(sysName, out IRedPoint root))
            {
                int index = path.IndexOf('/');
                if (index == -1)
                    return root;

                string splusName = path.Substring(index + 1);
                return root.GetChild(splusName);
            }

            return null;
        }

        public IRedPoint Add(string name)
        {
            return Add(GLOBAL, name);
        }

        public IRedPoint Add(string sysName, string name)
        {
            name = $"{sysName}/{name}";
            if (_points.TryGetValue(sysName, out IRedPoint root))
            {
                int index = name.IndexOf('/');
                return root.AddChild(name.Substring(index + 1));
            }
            else
            {
                string[] group = name.Split('/');
                RedPointNode last = null;
                foreach (string item in group)
                {
                    RedPointNode node = new RedPointNode(item, last);
                    if (last == null)
                        _points.Add(sysName, node);
                    last = node;
                }
                return last;
            }
        }
    }
}
