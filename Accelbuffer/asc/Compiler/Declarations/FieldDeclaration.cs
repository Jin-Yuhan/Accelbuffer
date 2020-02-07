namespace asc.Compiler.Declarations
{
    public sealed class FieldDeclaration : IDeclaration
    {
        public string Type = string.Empty;
        public string Name = string.Empty;
        public string? Doc = null;
        public int Index = 0;
        public bool IsObsolete = false;
        public bool IsNeverNull = false;
    }
}
