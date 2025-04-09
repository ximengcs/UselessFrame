
using System.Collections;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace UselessFrame.Runtime
{
    internal partial class ModuleCollection
    {
        public struct BackEnumerator : IEnumerator<ModuleBase>
        {
            private PurePool<List<ModuleBase>> _pool;
            private List<ModuleBase> _list;
            private int _index;

            public ModuleBase Current => _list[_index];

            object IEnumerator.Current => _list[_index];

            public BackEnumerator(PurePool<List<ModuleBase>> pool, List<ModuleBase> target)
            {
                _pool = pool;
                _list = pool.Require();
                if (_list == null)
                    _list = new List<ModuleBase>(64);

                for (int i = target.Count - 1; i >= 0; i--)
                    _list.Add(target[i]);
                _index = -1;
            }

            public void Dispose()
            {
                _list.Clear();
                _pool.Release(_list);
            }

            public bool MoveNext()
            {
                return ++_index < _list.Count;
            }

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
