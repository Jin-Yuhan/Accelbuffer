using Accelbuffer.Injection;
using Accelbuffer.Memory;
using Accelbuffer.Properties;
using System;

namespace Accelbuffer
{
    /// <summary>
    /// 公开序列化对象的接口
    /// </summary>
    public static unsafe class Serializer
    {
        /// <summary>
        /// 准备序列化代理
        /// </summary>
        /// <typeparam name="T">需要准备序列化代理的类型</typeparam>
        public static void PrepareSerializer<T>()
        {
            SerializerInjector.InternalCache<T>.Initialize();
        }

        /// <summary>
        /// 获取内存分配器
        /// </summary>
        /// <typeparam name="T">需要获取的内存分配器所属的对象类型</typeparam>
        /// <returns>类型的内存分配器</returns>
        public static MemoryAllocator GetAllocator<T>()
        {
            return SerializerInjector.InternalCache<T>.Allocator;
        }

        /// <summary>
        /// 序列化对象，并返回序列化数据（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <returns>对象的序列化结果</returns>
        public static byte[] Serialize<T>(T obj)
        {
            StreamingWriter writer = new StreamingWriter(GetAllocator<T>());
            writer.BeginWrite();

            try
            {
                SerializerInjector.InternalCache<T>.Serializer.Serialize(obj, ref writer);
                return writer.ToArray();
            }
            finally
            {
                writer.EndWrite();
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">用于接受序列化数据的缓冲区</param>
        /// <param name="index"><paramref name="buffer"/>开始写入的索引</param>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <returns>序列化数据的大小</returns>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        public static int Serialize<T>(T obj, byte[] buffer, int index)
        {
            StreamingWriter writer = new StreamingWriter(GetAllocator<T>());
            writer.BeginWrite();

            try
            {
                SerializerInjector.InternalCache<T>.Serializer.Serialize(obj, ref writer);
                return writer.CopyToArray(buffer, index);
            }
            finally
            {
                writer.EndWrite();
            }
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <param name="bytes">被反序列化的字节数组</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <param name="length">可以读取的字节大小</param>
        /// <typeparam name="T">反序列化的对象类型</typeparam>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes"/>长度不足</exception>
        public static T Deserialize<T>(byte[] bytes, int index, int length)
        {
            if (length == 0)
            {
                return default;
            }

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), Resources.ByteArrayIsNull);
            }

            if (bytes.Length - index < length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), Resources.ByteArrayTooShort);
            }

            fixed (byte* p = bytes)
            {
                StreamingIterator iterator = new StreamingIterator(p + index, length);
                return SerializerInjector.InternalCache<T>.Serializer.Deserialize(ref iterator);
            }
        }
    }
}
