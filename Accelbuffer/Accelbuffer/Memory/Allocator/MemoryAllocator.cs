using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Accelbuffer.Memory
{
    /// <summary>
    /// 表示一个非托管内存分配器
    /// </summary>
    public sealed unsafe partial class MemoryAllocator
    {
        private readonly Chunk*[] m_FreeList;//内存块链表
        private readonly object m_SyncObj;
        private int m_MaxFindChunkCount;

        /// <summary>
        /// 获取/设置分配内存时，寻找合适内存块的最大次数
        /// </summary>
        public int MaxFindChunkCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_MaxFindChunkCount;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                lock (m_SyncObj)
                {
                    m_MaxFindChunkCount = value > 1 ? value : m_MaxFindChunkCount;
                }
            }
        }

        /// <summary>
        /// 获取当前保存的所有内存块的总内存大小，以字节为单位
        /// </summary>
        public int TotalMemorySizeInChunks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        private MemoryAllocator()
        {
            m_FreeList = new Chunk*[FREE_LIST_LENGTH];
            m_SyncObj = new object();
            m_MaxFindChunkCount = DEFAULT_MAX_FIND_CHUNK_COUNT;
            TotalMemorySizeInChunks = 0;
        }

        /// <summary>
        /// 释放所有内存
        /// </summary>
        ~MemoryAllocator()
        {
            for (int i = 0; i < m_FreeList.Length; i++)
            {
                Chunk* chunk = m_FreeList[i];
                
                while (chunk != null)
                {
                    Chunk* next = chunk->NextChunkPtr;
                    Marshal.FreeHGlobal(new IntPtr(chunk));
                    chunk = next;
                }
            }
        }

        /// <summary>
        /// 释放多余的内存
        /// </summary>
        /// <param name="trimAll">指示是否释放所有的内存</param>
        public void Trim(bool trimAll = false)
        {
#if DEBUG
            int sizePrevious = TotalMemorySizeInChunks;
#endif

            lock (m_SyncObj)
            {
                for (int i = 0; i < m_FreeList.Length; i++)
                {
                    Chunk* chunk = m_FreeList[i];
                    int size = GetChunkSize(i);

                    while (chunk != null)
                    {
                        if (!trimAll && chunk->NextChunkPtr == null)
                        {
                            break;//保留一个chunk
                        }

                        Chunk* next = chunk->NextChunkPtr;
                        Marshal.FreeHGlobal(new IntPtr(chunk));
                        chunk = next;
                        TotalMemorySizeInChunks -= size;
                    }

                    m_FreeList[i] = chunk;
                }
            }

#if DEBUG
            Debug.Log($"[memory_allocator] 释放内存 {sizePrevious}字节 -> {TotalMemorySizeInChunks}字节");
#endif
        }

        /// <summary>
        /// 分配指定字节大小的内存，内存没有被初始化，可能包含一些随机的数据
        /// </summary>
        /// <param name="size">需要分配的字节大小，这个值的大小在分配内存时可能被向上调整</param>
        /// <returns>分配的内存指针，如果为null，则分配失败</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/>是负数</exception>
        internal byte* Allocate(ref int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            if (size == 0)
            {
                Debug.Log("[memory_allocator] 分配内存0字节");
                return null;
            }


            if (size > MAX_CHUNK_SIZE)
            {
                Debug.Log($"[memory_allocator] 分配大内存{size}字节");
                return (byte*)Marshal.AllocHGlobal(size).ToPointer();
            }

            int index = GetFreeListIndex(size);

            lock (m_SyncObj)
            {
                do
                {
                    Chunk* chunk = m_FreeList[index];

                    if (chunk != null)
                    {
                        m_FreeList[index] = chunk->NextChunkPtr;
                        chunk->NextChunkPtr = null;

                        size = GetChunkSize(index);
                        TotalMemorySizeInChunks -= size;

                        Debug.Log($"[memory_allocator] 返回内存{size}字节，位置[{index}]，地址{(long)chunk}");
                        return (byte*)chunk;
                    }
                }
                while ((++index < m_FreeList.Length + MaxFindChunkCount) && (index < m_FreeList.Length));
            }

            size = Align(size);
            Debug.Log($"[memory_allocator] 分配内存{size}字节");
            return (byte*)Marshal.AllocHGlobal(size).ToPointer();
        }

        /// <summary>
        /// 释放指定大小的内存
        /// </summary>
        /// <param name="p">指向内存的指针</param>
        /// <param name="size">内存的字节大小</param>
        internal void Free(void* p, int size)
        {
            if (p == null || size <= 0)
            {
                return;
            }

            if (size > MAX_CHUNK_SIZE)
            {
                Debug.Log($"[memory_allocator] 释放{size}字节");
                Marshal.FreeHGlobal(new IntPtr(p));
                return;
            }

            int index = GetFreeListIndex(size);
            lock (m_SyncObj)
            {
                Chunk* chunk = (Chunk*)p;
                chunk->NextChunkPtr = m_FreeList[index];
                m_FreeList[index] = chunk;
                TotalMemorySizeInChunks += size;

                Debug.Log($"[memory_allocator] 回收内存{size}字节，位置[{index}]，地址{(long)chunk}");
            }
        }

        /// <summary>
        /// 重新分配一段内存的大小
        /// </summary>
        /// <param name="p">指向原内存的指针</param>
        /// <param name="oldSize">原内存的字节大小</param>
        /// <param name="newSize">新内存的字节大小，这个值的大小在重新分配内存时可能被向上调整</param>
        /// <returns>指向新内存的指针，如果为null，则分配失败</returns>
        /// <exception cref="ArgumentNullException"><paramref name="p"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="oldSize"/>不是正数，或<paramref name="newSize"/>是负数</exception>
        internal byte* Reallocate(void* p, int oldSize, ref int newSize)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            if (oldSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(oldSize));
            }

            byte* ptr = Allocate(ref newSize);

            if (ptr != null)
            {
                int count = oldSize;
                byte* dst = ptr;
                byte* src = (byte*)p;

                Debug.Log($"[memory_allocator] 拷贝内存{count}字节({(long)src})->({(long)dst})");

                while (count-- > 0)
                {
                    *dst++ = *src++;
                }
            }

            Free(p, oldSize);
            return ptr;
        }
    }
}
