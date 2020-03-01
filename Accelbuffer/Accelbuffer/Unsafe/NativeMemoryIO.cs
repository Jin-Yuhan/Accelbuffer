using Accelbuffer.Memory;
using System;

namespace Accelbuffer.Unsafe
{
    /// <summary>
    /// 提供一系列<see cref="NativeMemory"/>的便捷写入/读取方法
    /// </summary>
    public static unsafe class NativeMemoryIO
    {
        /// <summary>
        /// 从内存中读取一个字节
        /// </summary>
        /// <param name="memory">保存了数据的一块非托管内存</param>
        /// <param name="offset">读取的偏移量</param>
        /// <returns>
        /// 强制转换为<see cref="int"/>类型的字节对象，
        /// 如果这个值为-1，则说明偏移处已经没有合法的字节可以读取
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数</exception>
        public static int ReadByte(this ref NativeMemory memory, int offset)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (memory.Size <= offset)
            {
                return -1;
            }

            return *(memory.GetPointer() + offset);
        }

        /// <summary>
        /// 从内存中读取指定数量的字节
        /// </summary>
        /// <param name="memory">保存了数据的一块非托管内存</param>
        /// <param name="offset">读取的偏移量</param>
        /// <param name="count">读取的字节数量</param>
        /// <returns>读取的所有字节</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数，或者<paramref name="count"/>是负数</exception>
        /// <exception cref="OutOfMemoryException"><paramref name="memory"/>中没有足够的大小用于读取</exception>
        public static byte[] ReadBytes(this ref NativeMemory memory, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (memory.Size < offset + count)
            {
                throw new OutOfMemoryException();
            }

            byte[] result = new byte[count];

            fixed (byte* p = result)
            {
                byte* ptr = p;

                while (count-- > 0)
                {
                    *ptr++ = memory.GetPointer()[offset++];
                }
            }

            return result;
        }

        /// <summary>
        /// 从内存中读取指定数量的字节，并写入指定内存中
        /// </summary>
        /// <param name="memory">保存了数据的一块非托管内存</param>
        /// <param name="offset">读取的偏移量</param>
        /// <param name="count">读取的字节数量</param>
        /// <param name="destination">用于接受读取的字节数组</param>
        /// <param name="destinationStartIndex">向<paramref name="destinationStartIndex"/>开始写入的索引</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/>是null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数，或者<paramref name="count"/>是负数</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="destinationStartIndex"/>是负数</exception>
        /// <exception cref="OutOfMemoryException"><paramref name="memory"/>中没有足够的大小用于读取，或者<paramref name="destination"/>中没有足够的大小用于写入</exception>
        public static void ReadBytes(this ref NativeMemory memory, int offset, int count, byte[] destination, int destinationStartIndex)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (destinationStartIndex < 0)
            {
                throw new IndexOutOfRangeException(nameof(destinationStartIndex));
            }

            if ((memory.Size < offset + count) || (destination.Length < destinationStartIndex + count))
            {
                throw new OutOfMemoryException();
            }

            fixed (byte* p = destination)
            {
                byte* ptr = p + destinationStartIndex;

                while (count-- > 0)
                {
                    *ptr++ = memory.GetPointer()[offset++];
                }
            }
        }

        /// <summary>
        /// 从内存中读取指定数量的字节，并写入指定内存中
        /// </summary>
        /// <param name="memory">保存了数据的一块非托管内存</param>
        /// <param name="offset">读取的偏移量</param>
        /// <param name="count">读取的字节数量</param>
        /// <param name="destination">用于接受读取的字节的内存的指针</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/>是null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数，或者<paramref name="count"/>是负数</exception>
        /// <exception cref="OutOfMemoryException"><paramref name="memory"/>中没有足够的大小用于读取</exception>
        [CLSCompliant(false)]
        public static void ReadBytes(this ref NativeMemory memory, int offset, int count, byte* destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (memory.Size < offset + count)
            {
                throw new OutOfMemoryException();
            }

            while (count-- > 0)
            {
                *destination++ = memory.GetPointer()[offset++];
            }
        }

        /// <summary>
        /// 向内存中写入一个字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="data">需要写入的字节</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数</exception>
        public static void WriteByte(this ref NativeMemory memory, int offset, byte data)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            *(memory.GetPointer(offset + 1) + offset) = data;
        }

        /// <summary>
        /// 向内存中写入两个字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="arg0">需要写入的字节</param>
        /// <param name="arg1">需要写入的字节</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数</exception>
        public static void WriteByte(this ref NativeMemory memory, int offset, byte arg0, byte arg1)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            byte* p = memory.GetPointer(offset + 2);
            p[offset] = arg0;
            p[offset + 1] = arg1;
        }

        /// <summary>
        /// 向内存中写入三个字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="arg0">需要写入的字节</param>
        /// <param name="arg1">需要写入的字节</param>
        /// <param name="arg2">需要写入的字节</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数</exception>
        public static void WriteByte(this ref NativeMemory memory, int offset, byte arg0, byte arg1, byte arg2)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            byte* p = memory.GetPointer(offset + 3);
            p[offset] = arg0;
            p[offset + 1] = arg1;
            p[offset + 2] = arg2;
        }

        /// <summary>
        /// 向内存中写入四个字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="arg0">需要写入的字节</param>
        /// <param name="arg1">需要写入的字节</param>
        /// <param name="arg2">需要写入的字节</param>
        /// <param name="arg3">需要写入的字节</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数</exception>
        public static void WriteByte(this ref NativeMemory memory, int offset, byte arg0, byte arg1, byte arg2, byte arg3)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            byte* p = memory.GetPointer(offset + 4);
            p[offset] = arg0;
            p[offset + 1] = arg1;
            p[offset + 2] = arg2;
            p[offset + 3] = arg3;
        }

        /// <summary>
        /// 向内存中写入数个字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="bytes">需要写入的字节</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数</exception>
        public static void WriteByte(this ref NativeMemory memory, int offset, params byte[] bytes)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            int count = bytes.Length;
            byte* p = memory.GetPointer(offset + count) + offset;

            fixed (byte* src = bytes)
            {
                byte* s = src;

                while (count-- > 0)
                {
                    *p++ = *s++;
                }
            }
        }

        /// <summary>
        /// 向内存中写入指定数量的字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="bytes">需要写入的字节数组</param>
        /// <param name="startIndex">读取<paramref name="bytes"/>的偏移量</param>
        /// <param name="count">需要写入的字节数量</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数，或者<paramref name="count"/>是负数</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="startIndex"/>是负数</exception>
        public static void WriteBytes(this ref NativeMemory memory, int offset, byte[] bytes, int startIndex, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (startIndex < 0)
            {
                throw new IndexOutOfRangeException(nameof(startIndex));
            }

            byte* p = memory.GetPointer(offset + count) + offset;

            fixed (byte* src = bytes)
            {
                byte* s = src + startIndex;

                while (count-- > 0)
                {
                    *p++ = *s++;
                }
            }
        }

        /// <summary>
        /// 向内存中写入指定数量的字节
        /// </summary>
        /// <param name="memory">需要写入的非托管内存</param>
        /// <param name="offset">写入的偏移量</param>
        /// <param name="bytes">需要写入的字节数组的指针</param>
        /// <param name="count">需要写入的字节数量</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/>是负数，或者<paramref name="count"/>是负数</exception>
        [CLSCompliant(false)]
        public static void WriteBytes(this ref NativeMemory memory, int offset, byte* bytes, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            byte* p = memory.GetPointer(offset + count) + offset;

            while (count-- > 0)
            {
                *p++ = *bytes++;
            }
        }

        /// <summary>
        /// 将当前内存转换为一个非托管缓冲区。
        /// 这个方法会开辟新的内存，
        /// 如果需要释放<paramref name="memory"/>的内存，可以调用<see cref="NativeMemory.Dispose"/>
        /// </summary>
        /// <param name="memory">需要拷贝的非托管内存</param>
        /// <param name="usedSize">暴露给非托管缓冲区使用的字节大小</param>
        /// <returns>与当前对象等效的非托管缓冲区</returns>
        /// <exception cref="ObjectDisposedException">当前内存已经被释放或者内存大小为0</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="usedSize"/>是负数或者大于<paramref name="memory"/>的大小</exception>
        public static NativeBuffer CopyToNativeBuffer(this ref NativeMemory memory, int usedSize)
        {
            if (memory.Size == 0)
            {
                throw new ObjectDisposedException(nameof(NativeMemory));
            }

            if (usedSize > memory.Size || usedSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(usedSize));
            }

            byte* src = memory.GetPointer();
            int newMemorySize = usedSize;
            int remainCopyCount = usedSize;
            byte* p = MemoryAllocator.Shared.Allocate(ref newMemorySize);
            byte* dst = p;

            while (remainCopyCount-- > 0)
            {
                *dst++ = *src++;
            }

            return new NativeBuffer(p, usedSize, newMemorySize);
        }

        /// <summary>
        /// 将一段非托管内存转换为托管字节数组
        /// </summary>
        /// <param name="memory">需要转换的非托管内存</param>
        /// <param name="size">转换的字节大小</param>
        /// <returns>保存了<paramref name="size"/>个来自<paramref name="memory"/>的字节的托管字节数组</returns>
        public static byte[] ToArray(this ref NativeMemory memory, int size)
        {
            NativeBuffer buffer = memory.ToNativeBufferNoCopy(size);
            return buffer.ToArray();
        }

        /// <summary>
        /// 将一段非托管内存拷贝到指定的托管字节数组
        /// </summary>
        /// <param name="memory">需要拷贝的非托管内存</param>
        /// <param name="size">拷贝的字节数量</param>
        /// <param name="buffer">拷贝到的托管字节数组</param>
        /// <param name="startIndex">开始写入的索引位置</param>
        /// <returns>写入的数据长度</returns>
        public static int CopyToArray(this ref NativeMemory memory, int size, byte[] buffer, int startIndex)
        {
            NativeBuffer nbuffer = memory.ToNativeBufferNoCopy(size);
            return nbuffer.CopyToArray(buffer, startIndex);
        }
    }
}
