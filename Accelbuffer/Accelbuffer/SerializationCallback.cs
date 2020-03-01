namespace Accelbuffer
{
    /// <summary>
    /// 序列化/反序列化回调
    /// </summary>
    /// <typeparam name="T">序列化/反序列化的对象类型</typeparam>
    /// <param name="obj">序列化/反序列化的对象</param>
    public delegate void SerializationCallback<T>(ref T obj);
}
