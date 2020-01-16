namespace accelc.Compiler
{
    public readonly struct Token
    {
        public readonly string Raw;
        public readonly TokenType Type;
        public readonly int LineNumber;
        public readonly string FilePath;

        public Token(string raw, TokenType type, int lineNumber, string filePath)
        {
            Raw = raw;
            Type = type;
            LineNumber = lineNumber;
            FilePath = filePath;
        }
    }
}
