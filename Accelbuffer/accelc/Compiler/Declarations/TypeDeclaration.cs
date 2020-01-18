namespace accelc.Compiler
{
    public sealed class TypeDeclaration : Declaration
    {
        public bool IsInternal;
        public bool IsFinal;
        public bool IsRuntime;
        public bool IsCompact;
        public bool IsRef;
        public string Name;
        public string Doc;

        public Declaration[] Declarations;
        public InitMemoryDeclaration InitMemory;
        public MessageDeclaration Before;
        public MessageDeclaration After;
    }
}
