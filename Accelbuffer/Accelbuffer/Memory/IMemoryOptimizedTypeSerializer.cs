namespace Accelbuffer.Memory
{
    /// <summary>
    /// 实现接口完成指定类型对象的序列化和反序列化
    /// </summary>
    /// <typeparam name="T">序列化的对象的类型</typeparam>
    public interface IMemoryOptimizedTypeSerializer<T> : ITypeSerializer<T>
    {
        /// <summary>
        /// 初始分配的内存大小
        /// </summary>
        int InitialMemorySize { get; }
    }
}
