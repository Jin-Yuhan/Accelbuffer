namespace Accelbuffer
{
    /// <summary>
    /// 指示一个序列化回调方法类型
    /// </summary>
    public enum SerializationCallbackMethod : byte
    {
        /// <summary>
        /// 在被序列化前调用
        /// </summary>
        OnBeforeSerialize,
        /// <summary>
        /// 在被反序列化后调用
        /// </summary>
        OnAfterDeserialize
    }
}
