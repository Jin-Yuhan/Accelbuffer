using System;

namespace Accelbuffer.Runtime.Injection
{
    /// <summary>
    /// 指示字段的索引
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FieldIndexAttribute : Attribute
    {
        /// <summary>
        /// 获取索引
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// 初始化 FieldIndexAttribute 实例
        /// </summary>
        /// <param name="index">索引</param>
        public FieldIndexAttribute(byte index)
        {
            Index = index;
        }
    }
}
