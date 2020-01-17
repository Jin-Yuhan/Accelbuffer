namespace accelc.Compiler
{
    public sealed class FieldDeclaration : Declaration
    {
        public bool IsFixed;
        public bool IsUnicode;
        public bool IsASCII;
        public bool IsUTF8;
        public bool IsCheckref;
        public string Type;
        public string Name;
        public string Doc;
        public string Assignment;
    }
}
