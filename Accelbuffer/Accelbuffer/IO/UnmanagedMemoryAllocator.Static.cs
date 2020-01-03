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

        private static readonly List<UnmanagedMemoryAllocator> s_Allocated;

        static UnmanagedMemoryAllocator()
        {
            DefaultInitialSize = 45L;
            s_Allocated = new List<UnmanagedMemoryAllocator>();
        }

        /// <summary>
        /// 释放所有使用<see cref="UnmanagedMemoryAllocator"/>分配的内存
        /// </summary>
        public static void FreeAllMemory()
        {
            for (int i = 0; i < s_Allocated.Count; i++)
            {
                s_Allocated[i].FreeMemory();
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
