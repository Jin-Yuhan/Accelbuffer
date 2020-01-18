using System;
using System.Reflection;

namespace Accelbuffer.Memory
{
    public sealed partial class MemoryAllocator
    {
        /// <summary>
        /// 获取/设置默认初始内存分配大小
        /// </summary>
        public static int DefaultInitialSize { get; set; }

        private static MemoryAllocator s_AllocatorLink;//链表保存

        static MemoryAllocator()
        {
            DefaultInitialSize = 50;
            s_AllocatorLink = null;
        }

        /// <summary>
        /// 尝试释放所有使用<see cref="MemoryAllocator"/>分配的内存
        /// </summary>
        public static void FreeAllAvailableMemory()
        {
            MemoryAllocator allocator = s_AllocatorLink;

            while (allocator != null)
            {
                allocator.TryFreeMemory();
                allocator = allocator.Previous;
            }
        }

        internal static MemoryAllocator Alloc<T>(ITypeSerializer<T> serializer)
        {
            int initialSize;

            if (serializer is IMemoryOptimizedTypeSerializer<T> s)
            {
                initialSize = s.InitialMemorySize <= 0 ? DefaultInitialSize : s.InitialMemorySize;
            }
            else
            {
                InitialMemorySizeAttribute attr = typeof(T).GetCustomAttribute<InitialMemorySizeAttribute>(true);
                initialSize = (attr == null || attr.InitialSize <= 0) ? DefaultInitialSize : attr.InitialSize;
            }

            s_AllocatorLink = new MemoryAllocator(initialSize, s_AllocatorLink);
            return s_AllocatorLink;
        }
    }
}
