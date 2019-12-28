using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指示字段作为数字被序列化的选项，
    /// 该特性只对 
    /// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,
    /// <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/> 
    /// 类型和这些类型的集合的字段有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NumberAttribute : Attribute
    {
        /// <summary>
        /// 获取数字序列化选项
        /// </summary>
        public NumberOption Option { get; }

        /// <summary>
        /// 初始化 OptimizedNumberAttribute 类型
        /// </summary>
        /// <param name="option">数字的序列化选项</param>
        public NumberAttribute(NumberOption option)
        {
            Option = option;
        }
    }
}
