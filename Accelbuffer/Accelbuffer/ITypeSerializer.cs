namespace Accelbuffer
{
    /// <summary>
    /// 实现接口完成指定类型对象的序列化和反序列化
    /// </summary>
    /// <typeparam name="T">序列化的对象的类型</typeparam>
    public interface ITypeSerializer<T>
    {
        /// <summary>
        /// 方法用于实现对象的序列化
        /// </summary>
        /// <param name="obj">将被序列化的对象</param>
        /// <param name="writer">数据输出流</param>
        void Serialize(T obj, ref AccelWriter writer);

        /// <summary>
        /// 方法用于实现对象的反序列化
        /// </summary>
        /// <param name="reader">数据输入流</param>
        /// <returns>反序列化对象</returns>
        T Deserialize(ref AccelReader reader);
    }
}
