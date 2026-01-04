using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityXFrameLib.Animations
{
    public partial class AnimatorChecker
    {
        private List<CheckInfo> m_Checks;
        private List<CheckInfo> _removeList;

        public AnimatorChecker()
        {
            m_Checks = new List<CheckInfo>();
            _removeList = new List<CheckInfo>();
        }

        public void UpdateState()
        {
            if (_removeList.Count > 0)
                _removeList.Clear();
            foreach (CheckInfo info in m_Checks)
            {
                AnimatorStateInfo state = info.Anim.GetCurrentAnimatorStateInfo(info.Layer);
                if (!info.IsReady)
                {
                    if (state.IsName(info.Name))
                    {
                        info.IsReady = true;
                    }
                }
                else
                {
                    if (!state.IsName(info.Name) || state.normalizedTime >= 1)
                    {
                        info.SetFinish();
                        _removeList.Add(info);
                    }
                }
            }

            if (_removeList.Count > 0)
            {
                foreach (CheckInfo removeItem in _removeList)
                {
                    m_Checks.Remove(removeItem);
                }
                _removeList.Clear();
            }
        }

        public IHandle Add(Animator anim, string name, int layer)
        {
            CheckInfo info = new CheckInfo(anim, name, layer);
            m_Checks.Add(info);
            return info;
        }

        public void Remove(Animator anim)
        {
            foreach (CheckInfo node in m_Checks)
            {
                if (node.Anim == anim)
                {
                    m_Checks.Remove(node);
                    break;
                }
            }
        }

        public void Remove(Animator anim, string name)
        {
            foreach (CheckInfo node in m_Checks)
            {
                if (node.Anim == anim && node.Name == name)
                {
                    m_Checks.Remove(node);
                    break;
                }
            }
        }
    }
}
