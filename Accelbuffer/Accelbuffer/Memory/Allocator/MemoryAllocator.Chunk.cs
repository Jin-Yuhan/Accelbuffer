namespace Accelbuffer.Memory
{
    public sealed partial class MemoryAllocator
    {
        private unsafe struct Chunk
        {
            public Chunk* NextChunkPtr;
        }
    }
}
