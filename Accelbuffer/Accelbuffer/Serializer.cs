using Accelbuffer.Memory;
using Accelbuffer.Properties;
using Accelbuffer.Reflection;
using Accelbuffer.Unsafe;
using System;

namespace Accelbuffer
{
    /// <summary>
    /// 公开序列化对象的接口
    /// </summary>
    public static unsafe class Serializer
    {
        /// <summary>
        /// 初始化类型的序列化代理
        /// </summary>
        /// <typeparam name="T">需要初始化的类型</typeparam>
        public static void InitializeForType<T>()
        {
            InternalTypeCache<T>.Initialize();
        }

        /// <summary>
        /// 添加一个序列化/反序列化事件的回调
        /// </summary>
        /// <typeparam name="T">序列化/反序列化对象的类型</typeparam>
        /// <param name="type">回调的类型</param>
        /// <param name="callback">回调委托</param>
        public static void AddCallback<T>(SerializationCallbackType type, SerializationCallback<T> callback)
        {
            if (type == SerializationCallbackType.OnBeforeSerialization)
            {
                InternalTypeCache<T>.OnBeforeSerializationCallbacks += callback;
            }
            else
            {
                InternalTypeCache<T>.OnAfterDeserializationCallbacks += callback;
            }
        }

        /// <summary>
        /// 添加序列化和反序列化事件的回调
        /// </summary>
        /// <typeparam name="T">序列化/反序列化对象的类型</typeparam>
        /// <param name="onBeforeSerialization">在序列化前调用的回调</param>
        /// <param name="onAfterDeserialization">在反序列化后调用的回调</param>
        public static void AddCallback<T>(SerializationCallback<T> onBeforeSerialization, SerializationCallback<T> onAfterDeserialization)
        {
            InternalTypeCache<T>.OnBeforeSerializationCallbacks += onBeforeSerialization;
            InternalTypeCache<T>.OnAfterDeserializationCallbacks += onAfterDeserialization;
        }

        /// <summary>
        /// 移除一个序列化/反序列化事件的回调
        /// </summary>
        /// <typeparam name="T">序列化/反序列化对象的类型</typeparam>
        /// <param name="type">回调的类型</param>
        /// <param name="callback">回调委托</param>
        public static void RemoveCallback<T>(SerializationCallbackType type, SerializationCallback<T> callback)
        {
            if (type == SerializationCallbackType.OnBeforeSerialization)
            {
                InternalTypeCache<T>.OnBeforeSerializationCallbacks -= callback;
            }
            else
            {
                InternalTypeCache<T>.OnAfterDeserializationCallbacks -= callback;
            }
        }

        /// <summary>
        /// 移除序列化和反序列化事件的回调
        /// </summary>
        /// <typeparam name="T">序列化/反序列化对象的类型</typeparam>
        /// <param name="onBeforeSerialization">在序列化前调用的回调</param>
        /// <param name="onAfterDeserialization">在反序列化后调用的回调</param>
        public static void RemoveCallback<T>(SerializationCallback<T> onBeforeSerialization, SerializationCallback<T> onAfterDeserialization)
        {
            InternalTypeCache<T>.OnBeforeSerializationCallbacks -= onBeforeSerialization;
            InternalTypeCache<T>.OnAfterDeserializationCallbacks -= onAfterDeserialization;
        }

        /// <summary>
        /// 序列化对象，并返回序列化数据
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <returns>保存了对象序列化数据的一块非托管缓冲区</returns>
        public static NativeBuffer Serialize<T>(T obj, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            InternalTypeCache<T>.OnBeforeSerializationCallbacks?.Invoke(ref obj);
            int memSize = InternalTypeCache<T>.ApproximateMemorySize;
            NativeMemory mem = NativeMemory.Allocate(ref memSize);

            AccelWriter writer = new AccelWriter(&mem, encoding, endian == Endian.LittleEndian);
            writer.WriteGlobalConfig(encoding, endian);

            try
            {
                InternalTypeCache<T>.Serializer.Serialize(obj, ref writer);
                return mem.ToNativeBufferNoCopy(writer.ByteCount);
            }
            catch
            {
                mem.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入缓冲区中
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">对象的序列化结果</param>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        public static void Serialize<T>(T obj, out byte[] buffer, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            InternalTypeCache<T>.OnBeforeSerializationCallbacks?.Invoke(ref obj);
            int memSize = InternalTypeCache<T>.ApproximateMemorySize;
            NativeMemory mem = NativeMemory.Allocate(ref memSize);

            AccelWriter writer = new AccelWriter(&mem, encoding, endian == Endian.LittleEndian);
            writer.WriteGlobalConfig(encoding, endian);

            try
            {
                InternalTypeCache<T>.Serializer.Serialize(obj, ref writer);
                buffer = mem.ToArray(writer.ByteCount);
            }
            finally
            {
                mem.Dispose();
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">用于接受序列化数据的缓冲区</param>
        /// <param name="index"><paramref name="buffer"/>开始写入的索引</param>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <returns>序列化数据的大小</returns>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        public static int Serialize<T>(T obj, byte[] buffer, int index, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            InternalTypeCache<T>.OnBeforeSerializationCallbacks?.Invoke(ref obj);
            int memSize = InternalTypeCache<T>.ApproximateMemorySize;
            NativeMemory mem = NativeMemory.Allocate(ref memSize);

            AccelWriter writer = new AccelWriter(&mem, encoding, endian == Endian.LittleEndian);
            writer.WriteGlobalConfig(encoding, endian);

            try
            {
                InternalTypeCache<T>.Serializer.Serialize(obj, ref writer);
                return mem.CopyToArray(writer.ByteCount, buffer, index);
            }
            finally
            {
                mem.Dispose();
            }
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入缓冲区中
        /// </summary>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="type">序列化的对象类型</param>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <returns>对象的序列化结果</returns>
        public static byte[] Serialize(object obj, AccelTypeInfo type, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            return type.SerializeFunction(obj, encoding, endian);
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <typeparam name="T">反序列化的对象类型</typeparam>
        /// <param name="bytes">保存了对象数据的字节数组</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <param name="length">可以读取的字节大小</param>
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

                T result = serializer.Deserialize(ref reader);
                InternalTypeCache<T>.OnAfterDeserializationCallbacks?.Invoke(ref result);
                return result;
            }
        }

        /// <summary>
        /// 反序列化<typeparamref name="T"/>类型对象实例
        /// </summary>
        /// <typeparam name="T">反序列化的对象类型</typeparam>
        /// <param name="buffer">保存了对象数据的非托管缓冲区</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <param name="length">可以读取的字节大小</param>
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

            T result = serializer.Deserialize(ref reader);
            InternalTypeCache<T>.OnAfterDeserializationCallbacks?.Invoke(ref result);
            return result;
        }

        /// <summary>
        /// 反序列化<see cref="AccelTypeInfo"/>类型对象实例
        /// </summary>
        /// <param name="type">反序列化的对象类型</param>
        /// <param name="bytes">保存了对象数据的字节数组</param>
        /// <param name="index">开始读取的索引位置</param>
        /// <param name="length">可以读取的字节大小</param>
        /// <returns>反序列化的对象实例</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes"/>长度不足</exception>
        public static object Deserialize(AccelTypeInfo type, byte[] bytes, int index, int length)
        {
            //这里不需要检测，下面的委托会调用public static T Deserialize<T>(byte[] bytes, int index, int length)
            return type.DeserializeFunction(bytes, index, length);
        }
    }
}
