using System;

namespace Accelbuffer
{
    /// <summary>
    /// 用于指定 类型/结构/接口 序列化的代理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class SerializeByAttribute : Attribute
    {
        /// <summary>
        /// 获取 序列化代理类型
        /// </summary>
        public Type ProxyType { get; }

        /// <summary>
        /// 初始化 SerializeByAttribute 实例，并指示运行时使用<paramref name="proxyType"/>类型作为序列化代理
        /// </summary>
        /// <param name="proxyType">序列化代理类型</param>
        public SerializeByAttribute(Type proxyType)
        {
            ProxyType = proxyType;
        }
    }
}
