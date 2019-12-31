using System;
using System.Collections.Generic;
using System.Reflection;
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
        private readonly bool m_ReadOnly;
        private bool m_StrictMode;
        private long m_InitialBufferSize;

        /// <summary>
        /// 获取/设置是否使用严格模式（严格模式下创建的<see cref="UnmanagedReader"/>会开启对序列化索引的严格匹配）
        /// </summary>
        public bool StrictMode
        {
            get => m_StrictMode;
            set
            {
                if (m_ReadOnly)
                {
                    throw new UnauthorizedAccessException("无法设置只读属性");
                }

                m_StrictMode = value;
            }
        }

        /// <summary>
        /// 获取/设置初始化缓冲区时应该分配的字节大小
        /// </summary>
        public long InitialBufferSize
        {
            get => m_InitialBufferSize;
            set
            {
                if (m_ReadOnly)
                {
                    throw new UnauthorizedAccessException("无法设置只读属性");
                }

                m_InitialBufferSize = value <= 0 ? m_InitialBufferSize : value;
            }
        }

        /// <summary>
        /// 获取 当前使用的内存大小
        /// </summary>
        public long CurrentUsedSize => m_CachedWriter == null ? 0L : m_CachedWriter->m_Size;


        private UnmanagedMemoryAllocator(long initialSize, bool strictMode, bool isReadOnly)
        {
            m_CachedWriter = null;
            m_InitialBufferSize = initialSize;
            m_StrictMode = strictMode;
            m_ReadOnly = isReadOnly;
        }


        internal UnmanagedWriter* GetCachedWriter()
        {
            if (m_CachedWriter == null)
            {
                m_CachedWriter = (UnmanagedWriter*)Marshal.AllocHGlobal(sizeof(UnmanagedWriter)).ToPointer();
                *m_CachedWriter = new UnmanagedWriter(InitialBufferSize, false);
            }

            m_CachedWriter->Reset();
            return m_CachedWriter;
        }

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
        /// 分配一个默认大小是<see cref="InitialBufferSize"/>的<see cref="UnmanagedWriter"/>
        /// </summary>
        /// <returns>创建的对象</returns>
        public UnmanagedWriter AllocWriter()
        {
            return new UnmanagedWriter(InitialBufferSize, true);
        }

        /// <summary>
        /// 分配一个<see cref="UnmanagedReader"/>
        /// </summary>
        /// <param name="source">源数据指针</param>
        /// <param name="offset">指针的偏移量</param>
        /// <param name="length">允许对象读取的字节长度</param>
        /// <returns>创建的对象</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>是负数</exception>
        public UnmanagedReader AllocReader(byte* source, long offset, long length)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "长度必须是非负数");
            }

            return new UnmanagedReader(source + offset, StrictMode, length);
        }

        internal static UnmanagedMemoryAllocator Alloc<T>()
        {
            Type objectType = typeof(T);
            MemoryAllocatorSettingsAttribute attr = objectType.GetCustomAttribute<MemoryAllocatorSettingsAttribute>(true);

            long initialBufferSize = SerializationUtility.GetBufferSize(objectType, attr == null ? 0L : attr.InitialBufferSize);
            bool strictMode = attr == null ? false : attr.StrictMode;
            bool readOnly = attr == null ? false : attr.RuntimeReadOnly;

            UnmanagedMemoryAllocator allocator = new UnmanagedMemoryAllocator(initialBufferSize, strictMode, readOnly);
            s_Allocated.Add(allocator);
            return allocator;
        }
    }
}
