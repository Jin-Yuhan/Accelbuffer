namespace Accelbuffer
{
    /// <summary>
    /// 实现接口完成<typeparamref name="T"/>类型对象的序列化代理
    /// </summary>
    /// <typeparam name="T">指定序列化的类型</typeparam>
    public interface ISerializeProxy<T>
    {
        /// <summary>
        /// 方法用于实现<typeparamref name="T"/>类型对象的序列化
        /// </summary>
        /// <param name="obj">将被序列化的对象</param>
        /// <param name="context">序列化上下文</param>
        /// <param name="writer">数据输出流</param>
        void Serialize(T obj, ref UnmanagedWriter writer, SerializationContext context);

        /// <summary>
        /// 方法用于实现<typeparamref name="T"/>类型对象的反序列化
        /// </summary>
        /// <param name="reader">数据输入流</param>
        /// <param name="context">序列化上下文</param>
        /// <returns>反序列化对象</returns>
        T Deserialize(ref UnmanagedReader reader, SerializationContext context);
    }
}
