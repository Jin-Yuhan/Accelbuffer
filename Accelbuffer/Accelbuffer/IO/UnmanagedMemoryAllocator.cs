using System;
using System.Runtime.InteropServices;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对非托管缓冲区内存的操作权限
    /// </summary>
    public sealed partial class UnmanagedMemoryAllocator
    {
        private IntPtr m_MemoryPtr;
        private long m_InitialSize;
        private readonly WritingBlock m_WritingBlock;

        /// <summary>
        /// 获取/设置初始化内存时应该分配的字节大小
        /// </summary>
        public long InitialSize
        {
            get => m_InitialSize;
            set => m_InitialSize = value <= 0L ? m_InitialSize : value;
        }

        /// <summary>
        /// 获取 当前分配的内存大小，以字节为单位
        /// </summary>
        public long AllocatedSize { get; private set; }

        private UnmanagedMemoryAllocator(long initialSize)
        {
            m_MemoryPtr = IntPtr.Zero;
            m_InitialSize = initialSize;
            m_WritingBlock = new WritingBlock();
            AllocatedSize = 0L;
        }

        internal unsafe byte* GetMemoryPtr(long minSize, long offset)
        {
            if (AllocatedSize < minSize)
            {
                if (AllocatedSize == 0L)
                {
                    AllocatedSize = minSize > InitialSize ? minSize : InitialSize;
                    m_MemoryPtr = Marshal.AllocHGlobal(new IntPtr(AllocatedSize));
                }
                else
                {
                    long sizeTwice = AllocatedSize << 1;
                    AllocatedSize = minSize > sizeTwice ? minSize : sizeTwice;
                    m_MemoryPtr = Marshal.ReAllocHGlobal(m_MemoryPtr, new IntPtr(AllocatedSize));
                }
            }

            return (byte*)m_MemoryPtr.ToPointer() + offset;
        }

        internal IDisposable MemoryWritingBlock()
        {
            m_WritingBlock.StartWriting();
            return m_WritingBlock;
        }

        /// <summary>
        /// 释放当前分配的内存
        /// </summary>
        /// <exception cref="InvalidOperationException">分配的内存当前正在被使用</exception>
        public void FreeMemory()
        {
            if (m_WritingBlock.IsWriting)
            {
                throw new InvalidOperationException("无法释放内存，因为这块内存正在被使用");
            }

            if (AllocatedSize > 0L)
            {
                Marshal.FreeHGlobal(m_MemoryPtr);
                m_MemoryPtr = IntPtr.Zero;
                AllocatedSize = 0L;
            }
        }
    }
}
