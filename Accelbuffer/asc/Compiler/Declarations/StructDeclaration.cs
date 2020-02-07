using System;

namespace asc.Compiler.Declarations
{
    public sealed class StructDeclaration : IDeclaration
    {
        public Visibility TypeVisibility = Visibility.None;
        public bool IsFinal = false;
        public bool IsRef = false;
        public bool IsNested = false;
        public bool IsFieldIndexContinuous = true;
        public string Name = string.Empty;
        public string? Doc = null;
        public int Size = 0;

        public IDeclaration[] Declarations = Array.Empty<IDeclaration>();
    }
}
