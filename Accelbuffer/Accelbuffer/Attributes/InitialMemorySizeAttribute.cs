using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指定<see cref="UnmanagedMemoryAllocator"/>初始内存分配大小
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class InitialMemorySizeAttribute : Attribute
    {
        /// <summary>
        /// 获取初始的内存分配大小，以字节为单位
        /// </summary>
        public long InitialSize { get; }

        /// <summary>
        /// 创建 InitialMemorySizeAttribute 实例
        /// </summary>
        /// <param name="initialSize">获取初始的内存分配大小，以字节为单位</param>
        public InitialMemorySizeAttribute(long initialSize)
        {
            InitialSize = initialSize;
        }
    }
}
