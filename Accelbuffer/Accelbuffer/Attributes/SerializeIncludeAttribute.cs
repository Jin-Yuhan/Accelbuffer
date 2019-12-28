using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指示字段参与序列化，且拥有指定的索引值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class SerializeIncludeAttribute : Attribute
    {
        /// <summary>
        /// 获取 序列化索引
        /// </summary>
        public byte SerializeIndex { get; }

        /// <summary>
        /// 初始化 SerializeIndexAttribute 实例
        /// </summary>
        /// <param name="serializeIndex">序列化索引</param>
        public SerializeIncludeAttribute(byte serializeIndex)
        {
            SerializeIndex = serializeIndex;
        }
    }
}
