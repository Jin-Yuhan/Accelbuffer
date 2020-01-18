using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Accelbuffer.Memory
{
    /// <summary>
    /// 公开对序列化内存分配的管理与查询接口
    /// </summary>
    public sealed partial class MemoryAllocator
    {
        private IntPtr m_MemoryPtr;
        private int m_InitialSize;
        private readonly object m_Lock;
        internal readonly MemoryAllocator Previous;

        /// <summary>
        /// 获取/设置初始化内存时应该分配的字节大小
        /// </summary>
        public int InitialSize
        {
            get => m_InitialSize;
            set => m_InitialSize = value <= 0 ? m_InitialSize : value;
        }

        /// <summary>
        /// 获取当前分配的内存大小，以字节为单位
        /// </summary>
        public int AllocatedSize { get; private set; }

        /// <summary>
        /// 获取是否正在写入内存
        /// </summary>
        public bool IsWritingMemory { get; private set; }

        /// <summary>
        /// 获取当前线程是否正在写入内存
        /// </summary>
        public bool IsCurrentThreadWritingMemory => IsWritingMemory && Monitor.IsEntered(m_Lock);

        private MemoryAllocator(int initialSize, MemoryAllocator previous)
        {
            m_MemoryPtr = IntPtr.Zero;
            m_InitialSize = initialSize;
            m_Lock = new object();

            AllocatedSize = 0;
            IsWritingMemory = false;

            Previous = previous;
        }

        /// <summary>
        /// 这个方法不是线程安全的，如果需要使方法线程安全，
        /// 请在前后插入<see cref="BeginThreadSafeMemoryWriting"/>和<see cref="EndThreadSafeMemoryWriting"/>
        /// </summary>
        internal IntPtr GetMemoryPtr(int minSize, int offset)
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
                    int sizeTwice = AllocatedSize << 1;
                    AllocatedSize = minSize > sizeTwice ? minSize : sizeTwice;
                    m_MemoryPtr = Marshal.ReAllocHGlobal(m_MemoryPtr, new IntPtr(AllocatedSize));
                }
            }

            return m_MemoryPtr + offset;
        }

        internal void BeginThreadSafeMemoryWriting()
        {
            Monitor.Enter(m_Lock);
            IsWritingMemory = true;
        }

        internal void EndThreadSafeMemoryWriting()
        {
            IsWritingMemory = false;
            Monitor.Exit(m_Lock);
        }

        /// <summary>
        /// 尝试释放当前分配的内存
        /// </summary>
        /// <returns>如果释放成功，返回true，否则，false</returns>
        public bool TryFreeMemory()
        {
            if (IsWritingMemory || AllocatedSize <= 0 || !Monitor.TryEnter(m_Lock))
            {
                return false;
            }

            Marshal.FreeHGlobal(m_MemoryPtr);
            m_MemoryPtr = IntPtr.Zero;
            AllocatedSize = 0;

            Monitor.Exit(m_Lock);
            return true;
        }
    }
}
