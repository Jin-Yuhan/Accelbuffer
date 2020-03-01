namespace Accelbuffer
{
    /// <summary>
    /// 序列化/反序列化回调类型
    /// </summary>
    public enum SerializationCallbackType
    {
        /// <summary>
        /// 指示回调在序列化前调用
        /// </summary>
        OnBeforeSerialization,
        /// <summary>
        /// 指示回调在反序列化后调用
        /// </summary>
        OnAfterSerialization
    }
}
