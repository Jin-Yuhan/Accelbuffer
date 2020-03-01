namespace Accelbuffer.Memory
{
    public sealed partial class MemoryAllocator
    {
        private unsafe struct Chunk
        {
            public Chunk* NextChunkPtr;
        }

        private unsafe struct FreeList
        {
            public fixed long List[FREE_LIST_LENGTH];

            public FreeList(long initialValue)
            {
                for (int i = 0; i < FREE_LIST_LENGTH; i++)
                {
                    List[i] = initialValue;
                }
            }
        }
    }
}
