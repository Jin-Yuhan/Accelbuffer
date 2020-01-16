using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指示类型应该被序列化成紧凑的布局，紧凑的布局可能导致 '不安全的反序列化'，但可以减少少量序列化后的字节大小并提高速度。
    /// !!!使用这个模式，请保证所有的字段的引用都不为null，且字节数据一定没有损坏!!!
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public sealed class CompactLayoutAttribute : Attribute { }
}
