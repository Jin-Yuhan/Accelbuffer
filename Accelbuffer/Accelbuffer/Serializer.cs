using System;
using System.Reflection;
using System.Runtime.Serialization;

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

        /// <summary>
        /// 获取/设置 是否使用严格的序列化模式（开启对序列化索引不匹配的错误警告）
        /// </summary>
        public static bool StrictMode { get; set; }

        static Serializer()
        {
            Type objectType = typeof(T);
            Type proxyType;
            long initialBufferSize;

            if (SerializationSettings.TryGetProxyBinding(objectType, out ProxySettings settings))
            {
                proxyType = settings.ProxyType;
                initialBufferSize = settings.InitialBufferSize;
                StrictMode = settings.StrictMode;
            }
            else
            {
                bool complex = SerializationUtility.IsTrulyComplex(objectType);

                if (complex && !objectType.IsValueType && objectType.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new SerializationException($"类型{objectType.Name}不能被序列化,因为没有无参构造函数");
                }

                SerializeContractAttribute attr = objectType.GetCustomAttribute<SerializeContractAttribute>(true);

                if (complex && attr == null)
                {
                    throw new SerializationException($"类型{objectType.Name}不能被序列化，因为没有被标记{typeof(SerializeContractAttribute).Name}特性，且没有手动绑定代理");
                }

                proxyType = GetProxyType(objectType, attr?.ProxyType);
                initialBufferSize = SerializationSettings.GetBufferSize(objectType, attr == null ? 0 : attr.InitialBufferSize);
                StrictMode = attr == null ? true : attr.StrictMode;
            }

            s_CachedProxy = (ISerializeProxy<T>)Activator.CreateInstance(proxyType);
            s_Lock = new object();

            Allocator = UnmanagedMemoryAllocator.Alloc(initialBufferSize);
        }

        private static Type GetProxyType(Type objectType, Type proxyType)
        {
            if (proxyType == null)
            {
                proxyType = SerializeProxyUtility.GenerateProxy(objectType);
            }
            else
            {
                if (proxyType.IsGenericTypeDefinition)
                {
                    proxyType = proxyType.MakeGenericType(objectType.GenericTypeArguments);
                }

                if (!typeof(ISerializeProxy<T>).IsAssignableFrom(proxyType))
                {
                    throw new NullReferenceException("获取" + typeof(T).Name + "的序列化代理失败");
                }
            }

            return proxyType;
        }

        /// <summary>
        /// 初始化（空方法，TODO：调用静态构造函数）
        /// </summary>
        public static void Initialize() { }

        /// <summary>
        /// 序列化对象，并返回序列化数据（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <returns>对象的序列化结果</returns>
        public static byte[] Serialize(T obj)
        {
            lock (s_Lock)
            {
                UnmanagedWriter* writer = Allocator.Writer;

                s_CachedProxy.Serialize(in obj, in writer);

                byte[] result = writer->ToArray();

                return result;
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
                UnmanagedWriter* writer = Allocator.Writer;

                s_CachedProxy.Serialize(in obj, in writer);

                long result = writer->CopyToArray(buffer, index);

                return result;
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

            T result;

            fixed (byte* p = bytes)
            {
                UnmanagedReader reader = UnmanagedMemoryAllocator.AllocReader(p, index, length, StrictMode);
                result = s_CachedProxy.Deserialize(&reader);
            }

            return result;
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
