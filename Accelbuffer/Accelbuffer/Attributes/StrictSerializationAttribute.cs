using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指示对类型使用严格序列化模式（开启对序列化索引的严格匹配）
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class StrictSerializationAttribute : Attribute { }
}
