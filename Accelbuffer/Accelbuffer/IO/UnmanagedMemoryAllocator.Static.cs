using System;
using System.Collections.Generic;
using System.Reflection;

namespace Accelbuffer
{
    public sealed partial class UnmanagedMemoryAllocator
    {
        /// <summary>
        /// 获取/设置默认初始内存分配大小
        /// </summary>
        public static long DefaultInitialSize { get; set; }

        /// <summary>
        /// 获取总共分配的非托管内存大小，以字节为单位
        /// </summary>
        public static long TotalAllocatedSize
        {
            get
            {
                long result = 0L;

                for (int i = 0; i < s_Allocated.Count; i++)
                {
                    result += s_Allocated[i].AllocatedSize;
                }

                return result;
            }
        }

        /// <summary>
        /// 获取所有的内存分配器的数量
        /// </summary>
        public static int AllocatorCount => s_Allocated.Count;

        private static readonly List<UnmanagedMemoryAllocator> s_Allocated;

        static UnmanagedMemoryAllocator()
        {
            DefaultInitialSize = 45L;
            s_Allocated = new List<UnmanagedMemoryAllocator>();
        }

        /// <summary>
        /// 尝试释放所有使用<see cref="UnmanagedMemoryAllocator"/>分配的内存
        /// </summary>
        public static void FreeAllAvailableMemory()
        {
            for (int i = 0; i < s_Allocated.Count; i++)
            {
                s_Allocated[i].TryFreeMemory();
            }
        }

        internal static UnmanagedMemoryAllocator Alloc<T>()
        {
            Type objectType = typeof(T);
            InitialMemorySizeAttribute attr = objectType.GetCustomAttribute<InitialMemorySizeAttribute>(true);

            long initialBufferSize = (attr == null || attr.InitialSize <= 0L) ? DefaultInitialSize : attr.InitialSize;
            UnmanagedMemoryAllocator allocator = new UnmanagedMemoryAllocator(initialBufferSize);

            s_Allocated.Add(allocator);
            return allocator;
        }
    }
}
