using System;

namespace Accelbuffer
{
    /// <summary>
    /// 用于指定类型的序列化和反序列化代理的类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class SerializeByAttribute : Attribute
    {
        /// <summary>
        /// 获取序列化代理类型
        /// </summary>
        public Type SerializerType { get; }

        /// <summary>
        /// 初始化 SerializeByAttribute 实例，并指示运行时使用<paramref name="serializerType"/>类型实现序列化与反序列化
        /// </summary>
        /// <param name="serializerType">序列化代理类型</param>
        public SerializeByAttribute(Type serializerType)
        {
            SerializerType = serializerType;
        }
    }
}
