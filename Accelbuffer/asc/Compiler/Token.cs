namespace asc.Compiler
{
    public readonly struct Token
    {
        public readonly string Raw;
        public readonly TokenType Type;
        public readonly int Line;
        public readonly int Column;
        public readonly string FilePath;

        public Token(string raw, TokenType type, int line, int column, string filePath)
        {
            Raw = raw;
            Type = type;
            Line = line;
            Column = column;
            FilePath = filePath;
        }
    }
}
