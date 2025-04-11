
using System.Collections.Generic;
using System.Text;

namespace UselessFrame.Runtime.Collections
{
    public partial class Csv<T>
    {
        /// <summary>
        /// 行数据类
        /// </summary>
        public class Line : IXEnumerable<T>
        {
            #region Inner Fields
            private XItType m_ItType;
            private T[] m_Data;
            #endregion

            #region Constructor
            /// <summary>
            /// 构造一个 <paramref name="count"/> 列的行数据
            /// </summary>
            /// <param name="count">列数</param>
            public Line(int count)
            {
                m_Data = new T[count];
            }
            #endregion

            #region Interface
            /// <summary>
            /// 列数
            /// </summary>
            public int Count => m_Data.Length;

            /// <summary>
            /// 获取或设置数据项
            /// </summary>
            /// <param name="index">索引</param>
            /// <returns>数据</returns>
            public T this[int index]
            {
                get => m_Data[index];
                set => m_Data[index] = value;
            }
            #endregion

            #region IXEnumerable Interface
            /// <summary>
            /// 获取迭代器
            /// </summary>
            /// <returns>迭代器</returns>
            public IEnumerator<T> GetEnumerator()
            {
                switch (m_ItType)
                {
                    case XItType.Forward: return new LineForwardIt(m_Data);
                    case XItType.Backward: return new LineForwardIt(m_Data);
                    default: return default;
                }
            }

            /// <summary>
            /// 设置迭代器类型
            /// </summary>
            /// <param name="type">迭代器类型</param>
            public void SetIt(XItType type)
            {
                m_ItType = type;
            }
            #endregion
            /// <summary>
            /// 获取Csv行数据字符串形式，以逗号分隔
            /// </summary>
            /// <returns>构造字符串</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Count; i++)
                {
                    sb.Append(m_Data[i]);
                    if (i < Count - 1)
                        sb.Append(',');
                }

                return sb.ToString();
            }

            /// <summary>
            /// 返回csv行数据字符串形式
            /// </summary>
            /// <param name="line">csv行实例</param>
            /// <returns>字符串形式</returns>
            public static implicit operator string(Line line)
            {
                return line.ToString();
            }
        }
    }
}
