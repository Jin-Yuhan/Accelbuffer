using Accelbuffer.Memory;
using System;
using System.Runtime.CompilerServices;

namespace Accelbuffer
{
    /// <summary>
    /// 提供更加方便的序列化对象的方法
    /// </summary>
    public static class ObjectSerializationShortcut
    {
        /// <summary>
        /// 序列化对象，并返回序列化数据
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        /// <returns>保存了对象序列化数据的一块非托管缓冲区</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeBuffer ToNativeBuffer<T>(this T obj, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            return Serializer.Serialize<T>(obj, encoding, endian);
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">对象的序列化结果</param>
        /// <param name="encoding">序列化使用的字符编码</param>
        /// <param name="endian">序列化使用的字节序</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes<T>(this T obj, out byte[] buffer, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            Serializer.Serialize<T>(obj, out buffer, encoding, endian);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToBytes<T>(this T obj, byte[] buffer, int index, Encoding encoding = Encoding.UTF8, Endian endian = Endian.BigEndian)
        {
            return Serializer.Serialize<T>(obj, buffer, index, encoding, endian);
        }
    }
}
