namespace Accelbuffer
{
    /// <summary>
    /// 实现接口完成<typeparamref name="T"/>类型对象的序列化代理
    /// </summary>
    /// <typeparam name="T">指定序列化的类型</typeparam>
    public unsafe interface ISerializeProxy<T>
    {
        /// <summary>
        /// 方法用于实现<typeparamref name="T"/>类型对象的序列化
        /// </summary>
        /// <param name="obj">将被序列化的对象（RO）</param>
        /// <param name="writer">用于写入序列化数据的对象（RO）</param>
        void Serialize(in T obj, in UnmanagedWriter* writer);

        /// <summary>
        /// 方法用于实现<typeparamref name="T"/>类型对象的反序列化
        /// </summary>
        /// <param name="reader">用于读取反序列化数据的对象(RO)</param>
        /// <returns>反序列化对象</returns>
        T Deserialize(in UnmanagedReader* reader);
    }
}
