using System;

namespace Accelbuffer.Runtime.Injection
{
    /// <summary>
    /// 指示字段的序列化索引，只有标记了这个特性的字段才会参与序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FieldIndexAttribute : Attribute
    {
        /// <summary>
        /// 获取字段的序列化索引，0表示既不写入索引，也不读取索引
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// 初始化 FieldIndexAttribute 实例
        /// </summary>
        /// <param name="index">字段的序列化索引，0表示既不写入索引，也不读取索引</param>
        public FieldIndexAttribute(byte index)
        {
            Index = index;
        }
    }
}
