using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Accelbuffer.Memory
{
    /// <summary>
    /// 表示一个非托管内存分配器
    /// </summary>
    public sealed unsafe partial class MemoryAllocator : IDebuggable
    {
        string IDebuggable.FriendlyName => "memory_allocator";

        private FreeList m_FreeList;//内存块链表
        private int m_MaxFindChunkCount;
        private readonly object m_SyncObj;

        /// <summary>
        /// 获取/设置分配内存时，寻找合适内存块的最大次数
        /// </summary>
        public int MaxFindChunkCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_MaxFindChunkCount;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value > 0)
                {
                    this.Log($"寻找合适内存块的最大次数{m_MaxFindChunkCount}->{value}");
                    Interlocked.Exchange(ref m_MaxFindChunkCount, value);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Chunk* GetChunkPtrAt(int index) => (Chunk*)m_FreeList.List[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetChunkPtrAt(int index, Chunk* value) => m_FreeList.List[index] = (long)value;

        private MemoryAllocator()
        {
            m_FreeList = new FreeList(0L);

            m_MaxFindChunkCount = DEFAULT_MAX_FIND_CHUNK_COUNT;
            TotalMemorySizeInChunks = 0;

            m_SyncObj = new object();
        }

        /// <summary>
        /// 释放所有内存
        /// </summary>
        ~MemoryAllocator()
        {
            for (int i = 0; i < FREE_LIST_LENGTH; i++)
            {
                Chunk* chunk = GetChunkPtrAt(i);
                
                while (chunk != null)
                {
                    Chunk* next = chunk->NextChunkPtr;
                    Marshal.FreeHGlobal(new IntPtr(chunk));
                    chunk = next;
                }
            }
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="leaveOneChunkEachSize">指示是否为每一种内存大小保留一个chunk</param>
        public void FreeMemory(bool leaveOneChunkEachSize = true)
        {
#if DEBUG
            int sizePrevious = TotalMemorySizeInChunks;
#endif

            lock (m_SyncObj)
            {
                for (int i = 0, size = ALIGN; i < FREE_LIST_LENGTH; i++, size += ALIGN)
                {
                    Chunk* chunk = GetChunkPtrAt(i);

                    while (chunk != null)
                    {
                        if (leaveOneChunkEachSize && chunk->NextChunkPtr == null)
                        {
                            break;//保留一个chunk
                        }

                        Chunk* next = chunk->NextChunkPtr;
                        Marshal.FreeHGlobal(new IntPtr(chunk));
                        chunk = next;
                        TotalMemorySizeInChunks -= size;
                    }

                    SetChunkPtrAt(i, chunk);
                }
            }

#if DEBUG
            this.Log($"释放内存 {sizePrevious}字节 -> {TotalMemorySizeInChunks}字节");
#endif
        }

        /// <summary>
        /// 分配指定字节大小的内存，内存没有被初始化，可能包含一些随机的数据
        /// </summary>
        /// <param name="size">需要分配的字节大小，这个值的大小在分配内存时可能被向上调整</param>
        /// <returns>分配的内存指针，如果为null，则分配失败</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/>是负数</exception>
        /// <exception cref="OutOfMemoryException">内存不足</exception>
        internal byte* Allocate(ref int size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            if (size == 0)
            {
                this.Log("分配内存0字节，地址NULL");
                return null;
            }

            if (size > MAX_CHUNK_SIZE)
            {
                byte* result = (byte*)Marshal.AllocHGlobal(size).ToPointer();
                this.Log($"分配大内存{size}字节，地址{(long)result}");
                return result;
            }

            Align(ref size);
            int index = GetFreeListIndex(size);
            int count = 0;
            
            lock (m_SyncObj)
            {
                int chunkSize = size;

                do
                {
                    Chunk* chunk = GetChunkPtrAt(index);
                    this.Log(index.ToString());
                    this.Log(chunkSize.ToString());
                    this.Log(((long)chunk).ToString());
                    if (chunk != null)
                    {
                        SetChunkPtrAt(index, chunk->NextChunkPtr);
                        chunk->NextChunkPtr = null;

                        TotalMemorySizeInChunks -= chunkSize;

                        this.Log($"返回内存{chunkSize}字节，位置[{index}]，地址{(long)chunk}");
                        return (byte*)chunk;
                    }

                    chunkSize += ALIGN;
                }
                while ((++count < MaxFindChunkCount) && (index < FREE_LIST_LENGTH));
            }

            byte* result1 = (byte*)Marshal.AllocHGlobal(size).ToPointer();
            this.Log($"分配内存{size}字节，地址{(long)result1}");
            return result1;
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
                this.Log($"释放{size}字节，地址{(long)p}");
                Marshal.FreeHGlobal(new IntPtr(p));
                return;
            }

            this.Assert(size % ALIGN == 0, $"无效的内存大小，因为不是{ALIGN}的整数倍，请使用分配内存时返回的大小");

            int index = GetFreeListIndex(size);

            lock (m_SyncObj)
            {
                Chunk* chunk = (Chunk*)p;
                chunk->NextChunkPtr = GetChunkPtrAt(index);
                SetChunkPtrAt(index, chunk);
                TotalMemorySizeInChunks += size;

                this.Log($"回收内存{size}字节，位置[{index}]，地址{(long)chunk}");
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

                this.Log($"拷贝内存{count}字节({(long)src})->({(long)dst})");

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
