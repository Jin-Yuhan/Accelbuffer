using Accelbuffer.Runtime.Injection;
using System;

namespace Accelbuffer
{
    /// <summary>
    /// 公开序列化<typeparamref name="T"/>类型对象的接口
    /// </summary>
    /// <typeparam name="T">序列化类型</typeparam>
    public static unsafe class Serializer<T>
    {
        /// <summary>
        /// 获取/设置是否使用严格模式（严格模式会开启对序列化索引的严格匹配）
        /// </summary>
        public static bool StrictMode { get; set; }

        /// <summary>
        /// 当前类型分配的缓冲区内存管理器
        /// </summary>
        public static UnmanagedMemoryAllocator Allocator { get; }

        private static readonly ISerializeProxy<T> s_CachedProxy;
        private static readonly object s_Lock;

        static Serializer()
        {
            StrictMode = SerializationUtility.GetIsStrictMode<T>();
            Allocator = UnmanagedMemoryAllocator.Alloc<T>();

            s_CachedProxy = SerializeProxyInjector.Inject<T>();
            s_Lock = new object();
        }

        /// <summary>
        /// 序列化对象，并返回序列化数据（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <returns>对象的序列化结果</returns>
        public static byte[] Serialize(T obj)
        {
            lock (s_Lock)
            {
                using (Allocator.MemoryWritingBlock())
                {
                    UnmanagedWriter writer = new UnmanagedWriter(Allocator);
                    s_CachedProxy.Serialize(obj, ref writer);
                    return writer.ToArray();
                }
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">用于接受序列化数据的缓冲区</param>
        /// <param name="index"><paramref name="buffer"/>开始写入的索引</param>
        /// <returns>序列化数据的大小</returns>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        public static long Serialize(T obj, byte[] buffer, long index)
        {
            lock (s_Lock)
            {
                using (Allocator.MemoryWritingBlock())
                {
                    UnmanagedWriter writer = new UnmanagedWriter(Allocator);
                    s_CachedProxy.Serialize(obj, ref writer);
                    return writer.CopyToArray(buffer, index);
                }
            }
        }

        /// <summary>
        /// 序列化对象，并写入序列化数据
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="writer">数据输出流</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>是不合法的</exception>
        public static void Serialize(T obj, ref UnmanagedWriter writer)
        {
            if (writer.IsDefault())
            {
                throw new ArgumentNullException(nameof(writer), "UnmanagedWriter 不合法");
            }

            s_CachedProxy.Serialize(obj, ref writer);
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <param name="bytes">被反序列化的字节数组</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <param name="length">可以读取的字节大小</param>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes"/>长度不足</exception>
        public static T Deserialize(byte[] bytes, long index, long length)
        {
            if (length == 0)
            {
                return default;
            }

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), "字节数组不能为空");
            }

            if (bytes.Length - index < length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "字节数组长度不足");
            }

            fixed (byte* p = bytes)
            {
                UnmanagedReader reader = new UnmanagedReader(p + index, StrictMode, length);
                return s_CachedProxy.Deserialize(ref reader);
            }
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <param name="reader">数据输入流</param>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>是不合法的</exception>
        public static T Deserialize(ref UnmanagedReader reader)
        {
            if (reader.IsDefault())
            {
                throw new ArgumentNullException(nameof(reader), "UnmanagedReader 不合法");
            }

            return s_CachedProxy.Deserialize(ref reader);
        }
    }
}
