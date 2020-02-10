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
        /// 初始化类型的序列化代理和内存分配器
        /// </summary>
        /// <typeparam name="T">需要初始化的类型</typeparam>
        public static void InitializeForType<T>()
        {
            InternalTypeCache<T>.Initialize();
        }

        /// <summary>
        /// 序列化对象，并返回序列化数据（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <returns>对象的序列化结果</returns>
        public static NativeBuffer Serialize<T>(T obj, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            int memSize = InternalTypeCache<T>.ApproximateMemorySize;
            NativeMemory mem = NativeMemory.Allocate(ref memSize);

            AccelWriter writer = new AccelWriter(&mem, encoding, endian == Endian.LittleEndian);
            writer.WriteGlobalConfig(encoding, endian);

            try
            {
                InternalTypeCache<T>.Serializer.Serialize(obj, ref writer);
                return writer.ToNativeBufferNoCopy();
            }
            catch
            {
                writer.Free();
                throw;
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">对象的序列化结果</param>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        public static void Serialize<T>(T obj, out byte[] buffer, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            int memSize = InternalTypeCache<T>.ApproximateMemorySize;
            NativeMemory mem = NativeMemory.Allocate(ref memSize);

            AccelWriter writer = new AccelWriter(&mem, encoding, endian == Endian.LittleEndian);
            writer.WriteGlobalConfig(encoding, endian);

            try
            {
                InternalTypeCache<T>.Serializer.Serialize(obj, ref writer);
                buffer = writer.ToNativeBufferNoCopy().ToArray();
            }
            finally
            {
                writer.Free();
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中（线程安全）
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">用于接受序列化数据的缓冲区</param>
        /// <param name="index"><paramref name="buffer"/>开始写入的索引</param>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <returns>序列化数据的大小</returns>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        public static int Serialize<T>(T obj, byte[] buffer, int index, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            int memSize = InternalTypeCache<T>.ApproximateMemorySize;
            NativeMemory mem = NativeMemory.Allocate(ref memSize);

            AccelWriter writer = new AccelWriter(&mem, encoding, endian == Endian.LittleEndian);
            writer.WriteGlobalConfig(encoding, endian);

            try
            {
                InternalTypeCache<T>.Serializer.Serialize(obj, ref writer);
                return writer.ToNativeBufferNoCopy().CopyToArray(buffer, index);
            }
            finally
            {
                writer.Free();
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
            //if (length < 2)
            //{
            //    return default;
            //}

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), Resources.ByteArrayIsNull);
            }

            if (bytes.Length - index < length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), Resources.ByteArrayTooShort);
            }

            AccelReader.ReadGlobalConfig(bytes[index], out Encoding encoding, out Endian endian);

            fixed (byte* p = bytes)
            {
                AccelReader reader = new AccelReader(p + index + 1, length - 1, encoding, endian == Endian.LittleEndian);
                ITypeSerializer<T> serializer = InternalTypeCache<T>.Serializer;

                if (serializer is IBuiltinTypeSerializer && !reader.HasNext())
                {
                    return default;
                }

                return serializer.Deserialize(ref reader);
            }
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <param name="buffer">被反序列化的字节缓冲区</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <param name="length">可以读取的字节大小</param>
        /// <typeparam name="T">反序列化的对象类型</typeparam>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentException"><paramref name="buffer"/>已经被释放</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffer"/>长度不足</exception>
        public static T Deserialize<T>(NativeBuffer buffer, int index, int length)
        {
            //if (length < 2)
            //{
            //    return default;
            //}

            if (!buffer)
            {
                throw new ArgumentException(nameof(buffer), Resources.ByteArrayIsNull);
            }

            if (buffer.Length - index < length)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), Resources.ByteArrayTooShort);
            }

            AccelReader.ReadGlobalConfig(buffer[index], out Encoding encoding, out Endian endian);
            AccelReader reader = new AccelReader((byte*)buffer + index + 1, length - 1, encoding, endian == Endian.LittleEndian);
            ITypeSerializer<T> serializer = InternalTypeCache<T>.Serializer;

            if (serializer is IBuiltinTypeSerializer && !reader.HasNext())
            {
                return default;
            }

            return serializer.Deserialize(ref reader);
        }
    }
}
