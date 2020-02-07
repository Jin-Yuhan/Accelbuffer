using System.Runtime.CompilerServices;

namespace Accelbuffer.Memory
{
    public sealed partial class MemoryAllocator
    {
        private const int ALIGN = 8;
        private const int MAX_CHUNK_SIZE = 256;
        private const int DEFAULT_MAX_FIND_CHUNK_COUNT = 2;
        private const int FREE_LIST_LENGTH = MAX_CHUNK_SIZE / ALIGN;

        /// <summary>
        /// 获取全局共享的内存分配器实例
        /// </summary>
        public static MemoryAllocator Shared { get; } = new MemoryAllocator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Align(int size)
        {
            return (int)((uint)(size + ALIGN - 1) & (~(uint)(ALIGN - 1)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetFreeListIndex(int size)
        {
            return ((size + ALIGN - 1) / ALIGN) - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetChunkSize(int index)
        {
            return (index + 1) * ALIGN;
        }
    }
}
