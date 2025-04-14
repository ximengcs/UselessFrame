
using System;

namespace UselessFrame.Runtime.Pools
{
    /// <summary>
    /// 对象池辅助器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PoolHelperAttribute : Attribute
    {
        /// <summary>
        /// 持有类型
        /// </summary>
        public Type Target { get; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="target">持有类型</param>
        public PoolHelperAttribute(Type target)
        {
            Target = target;
        }
    }
}
