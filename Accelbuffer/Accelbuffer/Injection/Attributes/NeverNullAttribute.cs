using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指示在序列化时这个字段的值永远不会是null。
    /// 标记了这个特性的字段在写入时不会进行null检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NeverNullAttribute : Attribute { }
}
