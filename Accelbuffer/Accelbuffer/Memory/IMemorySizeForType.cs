namespace Accelbuffer.Memory
{
    /// <summary>
    /// 实现接口，在初始化时提供<typeparamref name="T"/>类型将占用内存的近似大小。
    /// 如果实现这个接口的类型不实现<see cref="ITypeSerializer{T}"/>，则这个接口的实现无效。
    /// 在这个接口中设置的值的使用优先级大于使用<see cref="MemorySizeAttribute"/>设置的值。
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    public interface IMemorySizeForType<T>
    {
        /// <summary>
        /// 获取类型的对象将会占用的近似字节大小，这个值必须大于0且应该尽可能返回准确的值。
        /// 在序列化时，会使用这个值进行第一次的内存分配，如果数值合理，可以避免realloc和memcpy的时间损失。
        /// 在类型的大小不固定时，如果这个值有一个上限，可以考虑使用这个上限值。
        /// </summary>
        int ApproximateMemorySize { get; }
    }
}
