namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个标记
    /// </summary>
    public readonly struct Token
    {
        /// <summary>
        /// 获取原始字符串
        /// </summary>
        public readonly string Raw;
        /// <summary>
        /// 获取标记的类型
        /// </summary>
        public readonly TokenType Type;
        /// <summary>
        /// 获取标记所在的行号
        /// </summary>
        public readonly int Line;
        /// <summary>
        /// 获取标记所在的列号
        /// </summary>
        public readonly int Column;
        /// <summary>
        /// 获取标记所在的文件路径
        /// </summary>
        public readonly string FilePath;

        /// <summary>
        /// 创建一个新的标记
        /// </summary>
        /// <param name="raw">原始字符串</param>
        /// <param name="type">标记的类型</param>
        /// <param name="line">标记所在的行号</param>
        /// <param name="column">标记所在的列号</param>
        /// <param name="filePath">标记所在的文件路径</param>
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
