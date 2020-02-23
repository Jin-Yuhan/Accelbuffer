using Accelbuffer.Memory;
using Accelbuffer.Properties;
using System;

namespace Accelbuffer.Unsafe
{
    /// <summary>
    /// 提供最快速的序列化非托管对象的接口，
    /// 但是通过该接口序列化的数据不做任何优化，
    /// 数据的大小等于对象类型的字节大小，
    /// 字符的编码统一使用Unicode编码，
    /// 字节序根据不同操作系统而不同。
    /// 换句话说这个接口序列化类型时，
    /// 只是一个memcpy的过程，
    /// 如果需要更多功能支持，
    /// 应该使用<see cref="Serializer"/>进行类型的序列化。
    /// </summary>
    public static unsafe class UnmanagedTypeSerializer
    {
        /// <summary>
        /// 序列化对象，并返回序列化数据
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <returns>保存了对象序列化数据的一块非托管缓冲区</returns>
        public static NativeBuffer Serialize<T>(T obj) where T : unmanaged
        {
            int memorySize = sizeof(T);
            byte* p = MemoryAllocator.Shared.Allocate(ref memorySize);
            *(T*)p = obj;
            return new NativeBuffer(p, sizeof(T), memorySize);
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">保存了对象序列化数据的缓冲区</param>
        public static void Serialize<T>(T obj, out byte[] buffer) where T : unmanaged
        {
            buffer = new byte[sizeof(T)];

            fixed (byte* p = buffer)
            {
                *(T*)p = obj;
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">用于接受序列化数据的缓冲区</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <returns>序列化数据的大小</returns>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        public static int Serialize<T>(T obj, byte[] buffer, int index) where T : unmanaged
        {
            int size = sizeof(T);

            if (buffer.Length - index < size)
            {
                throw new ArgumentException(Resources.ByteArrayTooShort);
            }

            fixed (byte* p = buffer)
            {
                *(T*)(p + index) = obj;
            }

            return size;
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="bytes">保存了对象数据的字节数组</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes"/>长度不足</exception>
        public static T Deserialize<T>(byte[] bytes, int index) where T : unmanaged
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), Resources.ByteArrayIsNull);
            }

            int size = sizeof(T);

            if (bytes.Length - index < size)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), Resources.ByteArrayTooShort);
            }

            fixed (byte* p = bytes)
            {
                return *(T*)(p + index);
            }
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="buffer">保存了对象数据的非托管缓冲区</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentException"><paramref name="buffer"/>已经被释放</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffer"/>长度不足</exception>
        public static T Deserialize<T>(NativeBuffer buffer, int index) where T : unmanaged
        {
            if (!buffer)
            {
                throw new ArgumentException(nameof(buffer), Resources.ByteArrayIsNull);
            }

            int size = sizeof(T);

            if (buffer.Length - index < size)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), Resources.ByteArrayTooShort);
            }

            return *(T*)((byte*)buffer + index);
        }
    }
}
