using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指定内存分配器的设置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class MemoryAllocatorAttribute : Attribute
    {
        /// <summary>
        /// 获取初始的序列化缓冲区大小，以字节为单位
        /// </summary>
        public long InitialBufferSize { get; }

        /// <summary>
        /// 获取是否使用严格模式（严格模式下创建的<see cref="UnmanagedReader"/>会开启对序列化索引的严格匹配）
        /// </summary>
        public bool StrictMode { get; }

        /// <summary>
        /// 获取/设置内存分配器的设置是否在运行时为只读
        /// </summary>
        public bool RuntimeReadOnly { get; set; }

        /// <summary>
        /// 创建 MemoryAllocatorAttribute 实例
        /// </summary>
        /// <param name="initialBufferSize">初始的序列化缓冲区大小，以字节为单位</param>
        /// <param name="strictMode">是否使用严格模式（严格模式下创建的<see cref="UnmanagedReader"/>会开启对序列化索引的严格匹配）</param>
        public MemoryAllocatorAttribute(long initialBufferSize, bool strictMode)
        {
            InitialBufferSize = initialBufferSize;
            StrictMode = strictMode;
        }
    }
}
