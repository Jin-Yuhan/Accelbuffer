using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指示字段的序列化索引，只有标记了这个特性的字段才会参与序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FieldIndexAttribute : Attribute
    {
        /// <summary>
        /// 获取字段的序列化索引
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 初始化 FieldIndexAttribute 实例
        /// </summary>
        /// <param name="index">字段的序列化索引，必须是 (0, 268435456] 范围内的整数；如果可能，索引应该尽可能全部连续（以1递增或递减）</param>
        public FieldIndexAttribute(int index)
        {
            Index = index;
        }
    }
}
