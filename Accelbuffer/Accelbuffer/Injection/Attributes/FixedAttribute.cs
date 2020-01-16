using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指示字段使用固定长度格式，
    /// 该特性只对 
    /// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,
    /// <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/> 类型的字段有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FixedAttribute : Attribute { }
}
