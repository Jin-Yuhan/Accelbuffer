using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指示字段作为数字被序列化的类型，
    /// 该特性只对 
    /// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,
    /// <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/> 
    /// 类型和这些类型的集合的字段有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NumberTypeAttribute : Attribute
    {
        /// <summary>
        /// 获取数字序列化类型
        /// </summary>
        public Number NumberType { get; }

        /// <summary>
        /// 初始化 NumberTypeAttribute 类型
        /// </summary>
        /// <param name="type">数字的类型选项</param>
        public NumberTypeAttribute(Number type)
        {
            NumberType = type;
        }
    }
}
