using System.Collections.Generic;

namespace Accelbuffer
{
    /// <summary>
    /// 公开可序列化枚举数，该枚举数支持在指定类型的集合上进行简单迭代并且可以被序列化。
    /// </summary>
    /// <typeparam name="T">要枚举或序列化的对象的类型</typeparam>
    public interface ISerializableEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// 获取元素的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <param name="value">添加的元素</param>
        void Add(T value);
    }
}
