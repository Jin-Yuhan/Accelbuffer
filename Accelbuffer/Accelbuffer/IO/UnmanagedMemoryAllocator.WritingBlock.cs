using System;

namespace Accelbuffer
{
    public sealed partial class UnmanagedMemoryAllocator
    {
        private class WritingBlock : IDisposable
        {
            public bool IsWriting { get; private set; }

            public void StartWriting()
            {
                IsWriting = true;
            }

            public void Dispose()
            {
                IsWriting = false;
            }
        }
    }
}
