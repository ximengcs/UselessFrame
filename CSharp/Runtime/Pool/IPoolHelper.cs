using System;
using System.Collections.Generic;
using System.Text;

namespace UselessFrame.Runtime.Pools
{
    /// <summary>
    /// 对象池辅助器
    /// </summary>
    public interface IPoolHelper
    {
        /// <summary>
        /// 池对象数量
        /// </summary>
        int CacheCount { get; }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="poolKey">对象key</param>
        /// <param name="userData">参数数据</param>
        /// <returns>对象实例</returns>
        protected internal IPoolObject Factory(Type type, int poolKey = default);

        /// <summary>
        /// 对象创建生命周期
        /// </summary>
        /// <param name="obj">目标对象</param>
        protected internal void OnObjectCreate(IPoolObject obj);

        /// <summary>
        /// 对象请求生命周期
        /// </summary>
        /// <param name="obj">目标对象</param>
        protected internal void OnObjectRequest(IPoolObject obj);

        /// <summary>
        /// 对象释放生命周期
        /// </summary>
        /// <param name="obj">目标对象</param>
        protected internal void OnObjectRelease(IPoolObject obj);

        /// <summary>
        /// 对象释放生命周期
        /// </summary>
        /// <param name="obj">目标对象</param>
        protected internal void OnObjectDestroy(IPoolObject obj);
    }
}
