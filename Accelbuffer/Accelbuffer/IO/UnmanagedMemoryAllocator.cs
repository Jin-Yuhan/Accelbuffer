using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对非托管缓冲区内存的操作权限
    /// </summary>
    public sealed unsafe class UnmanagedMemoryAllocator
    {
        private static readonly List<UnmanagedMemoryAllocator> s_Allocated = new List<UnmanagedMemoryAllocator>();

        private UnmanagedWriter* m_CachedWriter;
        private long m_InitialSize;

        internal UnmanagedWriter* Writer
        {
            get
            {
                if (m_CachedWriter == null)
                {
                    m_CachedWriter = (UnmanagedWriter*)Marshal.AllocHGlobal(sizeof(UnmanagedWriter)).ToPointer();
                    *m_CachedWriter = new UnmanagedWriter(m_InitialSize, false);
                }

                m_CachedWriter->Reset();
                return m_CachedWriter;
            }
        }

        /// <summary>
        /// 获取/设置 初始化缓冲区时应该分配的字节大小
        /// </summary>
        public long InitialBufferSize
        {
            get => m_InitialSize;
            set => m_InitialSize = value <= 0 ? m_InitialSize : value;
        }

        /// <summary>
        /// 获取 当前使用的内存大小
        /// </summary>
        public long CurrentUsedSize => m_CachedWriter == null ? 0L : m_CachedWriter->m_Size;


        private UnmanagedMemoryAllocator() { }


        /// <summary>
        /// 释放当前缓冲区使用的内存
        /// </summary>
        public void FreeUsedMemory()
        {
            if (m_CachedWriter != null)
            {
                m_CachedWriter->Free();
                Marshal.FreeHGlobal(new IntPtr(m_CachedWriter));
                m_CachedWriter = null;
            }
        }

        /// <summary>
        /// 释放所有使用<see cref="UnmanagedMemoryAllocator"/>分配的内存
        /// </summary>
        public static void FreeAll()
        {
            for (int i = 0; i < s_Allocated.Count; i++)
            {
                s_Allocated[i].FreeUsedMemory();
            }
        }

        /// <summary>
        /// 分配一个默认大小是<paramref name="size"/>的<see cref="UnmanagedWriter"/>
        /// </summary>
        /// <param name="size"><see cref="UnmanagedWriter"/>的默认大小</param>
        /// <returns>创建的对象</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/>是负数</exception>
        public static UnmanagedWriter AllocWriter(long size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "缓冲区默认大小必须是非负数");
            }

            return new UnmanagedWriter(size, true);
        }

        /// <summary>
        /// 分配一个<see cref="UnmanagedReader"/>
        /// </summary>
        /// <param name="source">源数据指针</param>
        /// <param name="offset">指针的偏移量</param>
        /// <param name="length">允许对象读取的字节长度</param>
        /// <param name="strictMode">是否使用严格的序列化模式（开启对序列化索引不匹配的错误警告）</param>
        /// <returns>创建的对象</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>是负数</exception>
        public static UnmanagedReader AllocReader(byte* source, long offset, long length, bool strictMode)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "长度必须是非负数");
            }

            return new UnmanagedReader(source + offset, strictMode, length);
        }

        internal static UnmanagedMemoryAllocator Alloc(long initialSize)
        {
            UnmanagedMemoryAllocator allocator = new UnmanagedMemoryAllocator { m_CachedWriter = null, m_InitialSize = initialSize };
            s_Allocated.Add(allocator);
            return allocator;
        }
    }
}
