using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指示序列化时检查字段引用是否为null，如果是null，则不写入字段，这个特性只对引用类型有效；
    /// 在<see cref="CompactLayoutAttribute"/>下，这个特性无效
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CheckRefAttribute : Attribute { }
}
