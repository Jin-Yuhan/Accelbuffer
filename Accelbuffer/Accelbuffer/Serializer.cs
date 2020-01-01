using System;
using System.Collections.Generic;
using System.Reflection;

namespace Accelbuffer
{
    /// <summary>
    /// 公开序列化<typeparamref name="T"/>类型对象的接口
    /// </summary>
    /// <typeparam name="T">序列化类型</typeparam>
    public static unsafe class Serializer<T>
    {
        private static readonly ISerializeProxy<T> s_CachedProxy;
        private static readonly object s_Lock;

        /// <summary>
        /// 当前类型分配的缓冲区内存管理器
        /// </summary>
        public static UnmanagedMemoryAllocator Allocator { get; }

        static Serializer()
        {
            s_CachedProxy = SerializeProxyInjector.Inject<T>();
            Allocator = UnmanagedMemoryAllocator.Alloc<T>();
            s_Lock = new object();
        }

        /// <summary>
        /// 初始化，包括被当前类型引用的对象
        /// </summary>
        public static void Initialize()
        {
            List<FieldData> fields = SerializeProxyInjector.GetSerializedFields(typeof(T));

            for (int i = 0; i < fields.Count; i++)
            {
                Type type = fields[i].Field.FieldType;

                if (SerializationUtility.IsTrulyComplex(type))
                {
                    if (type.IsArray)
                    {
                        type = type.GetElementType();
                    }

                    typeof(Serializer<>).MakeGenericType(type).GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                }
            }
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
                UnmanagedWriter* writer = Allocator.GetCachedWriter();
                s_CachedProxy.Serialize(in obj, in writer);
                return writer->ToArray();
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
                UnmanagedWriter* writer = Allocator.GetCachedWriter();
                s_CachedProxy.Serialize(in obj, in writer);
                return writer->CopyToArray(buffer, index);
            }
        }

        /// <summary>
        /// 序列化对象，并写入序列化数据
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="writer">用于序列化对象的写入指针</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>为null</exception>
        public static void Serialize(T obj, UnmanagedWriter* writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer), "写入指针不能为空");
            }

            s_CachedProxy.Serialize(in obj, in writer);
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
                UnmanagedReader reader = Allocator.AllocReader(p, index, length);
                return s_CachedProxy.Deserialize(&reader);
            }
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <param name="reader">反序列化读取指针</param>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/>为null</exception>
        public static T Deserialize(UnmanagedReader* reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader), "读取指针不能为空");
            }

            return s_CachedProxy.Deserialize(reader);
        }
    }
}
