using System;

namespace Accelbuffer.Memory
{
    /// <summary>
    /// 设置类型的序列化/反序列化选项
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class MemorySizeAttribute : Attribute
    {
        /// <summary>
        /// 获取类型的对象将会占用的近似字节大小。
        /// </summary>
        public int ApproximateMemorySize { get; }

        /// <summary>
        /// 初始化MemorySizeAttribute实例
        /// </summary>
        /// <param name="approximateMemorySize">
        /// 类型的对象将会占用的近似字节大小，这个值必须大于0且应该尽可能传入准确的值。
        /// 在序列化时，会使用这个值进行第一次的内存分配，如果数值合理，可以避免realloc和memcpy的时间损失。
        /// 在类型的大小不固定时，如果这个值有一个上限，可以考虑使用这个上限值。
        /// </param>
        public MemorySizeAttribute(int approximateMemorySize)
        {
            ApproximateMemorySize = approximateMemorySize;
        }
    }
}
