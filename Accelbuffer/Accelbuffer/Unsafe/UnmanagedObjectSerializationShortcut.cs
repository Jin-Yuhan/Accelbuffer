using Accelbuffer.Memory;
using System;
using System.Runtime.CompilerServices;

namespace Accelbuffer.Unsafe
{
    /// <summary>
    /// 提供更加方便的序列化非托管对象的方法
    /// </summary>
    public static class UnmanagedObjectSerializationShortcut
    {
        /// <summary>
        /// 序列化对象，并返回序列化数据
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <returns>保存了对象序列化数据的一块非托管缓冲区</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeBuffer WriteToBufferUnsafe<T>(this T obj) where T : unmanaged
        {
            return UnmanagedTypeSerializer.Serialize<T>(obj);
        }

        /// <summary>
        /// 序列化对象，并将序列化数据写入指定的缓冲区中
        /// </summary>
        /// <typeparam name="T">序列化的对象类型</typeparam>
        /// <param name="obj">被序列化的对象</param>
        /// <param name="buffer">保存了对象序列化数据的缓冲区</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteToBytesUnsafe<T>(this T obj, out byte[] buffer) where T : unmanaged
        {
            UnmanagedTypeSerializer.Serialize<T>(obj, out buffer);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteToBytesUnsafe<T>(this T obj, byte[] buffer, int index) where T : unmanaged
        {
            return UnmanagedTypeSerializer.Serialize<T>(obj, buffer, index);
        }
    }
}
