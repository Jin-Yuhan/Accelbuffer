using System;

namespace Accelbuffer.Memory
{
    /// <summary>
    /// 表示一段自动变长的非托管内存
    /// </summary>
    public unsafe struct NativeMemory : IDisposable
    {
        private byte* m_Ptr;
        private int m_Size;

        /// <summary>
        /// 获取当前内存的大小，以字节为单位
        /// </summary>
        public int Size => m_Size;

        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            MemoryAllocator.Shared.Free(m_Ptr, Size);
            m_Ptr = null;
            m_Size = 0;
        }

        /// <summary>
        /// 获取内存的指针
        /// </summary>
        /// <returns>指向内存的指针，如果是null，则对象已经被释放或者内存大小为0</returns>
        [CLSCompliant(false)]
        public byte* GetPointer()
        {
            return m_Ptr;
        }

        /// <summary>
        /// 获取内存的指针，且保证内存的大小至少是<paramref name="minSize"/>
        /// </summary>
        /// <param name="minSize">获取的内存的最小值，以字节为单位</param>
        /// <returns>指向内存的指针</returns>
        /// <exception cref="ObjectDisposedException">当前内存已经被释放或者内存大小为0</exception>
        [CLSCompliant(false)]
        public byte* GetPointer(int minSize)
        {
            if (m_Size == 0)
            {
                throw new ObjectDisposedException(nameof(NativeMemory));
            }

            if (m_Size < minSize)
            {
                int oldSize = Size;
                int sizeTwice = oldSize << 1;
                m_Size = sizeTwice < minSize ? minSize : sizeTwice;
                m_Ptr = MemoryAllocator.Shared.Reallocate((void*)m_Ptr, oldSize, ref m_Size);
            }

            return m_Ptr;
        }

        /// <summary>
        /// 将当前内存转换为一个非托管缓冲区。
        /// 这个方法不会开辟新的内存，
        /// 所以如果要继续使用返回的<see cref="NativeBuffer"/>就不应该调用<see cref="Dispose"/>方法，
        /// 如果需要释放这块内存，只需要调用<see cref="NativeBuffer.Dispose"/>和<see cref="Dispose"/>其中一个
        /// </summary>
        /// <param name="usedSize">暴露给非托管缓冲区使用的字节大小</param>
        /// <returns>与当前对象等效的非托管缓冲区</returns>
        /// <exception cref="ObjectDisposedException">当前内存已经被释放或者内存大小为0</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="usedSize"/>是负数或者大于<see cref="Size"/></exception>
        public NativeBuffer ToNativeBufferNoCopy(int usedSize)
        {
            if (m_Size == 0)
            {
                throw new ObjectDisposedException(nameof(NativeMemory));
            }

            if (usedSize > m_Size || usedSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(usedSize));
            }

            return new NativeBuffer(m_Ptr, usedSize, m_Size);
        }

        /// <summary>
        /// 分配一块可以自动变长的非托管内存
        /// </summary>
        /// <param name="size">内存的初始大小，这个值的大小在分配内存时可能被向上调整</param>
        /// <returns>内存对象</returns>
        public static NativeMemory Allocate(ref int size)
        {
            byte* p = MemoryAllocator.Shared.Allocate(ref size);
            return new NativeMemory { m_Ptr = p, m_Size = size };
        }
    }
}
