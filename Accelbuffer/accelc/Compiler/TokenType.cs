namespace accelc.Compiler
{
    public enum TokenType
    {
        /// <summary>
        /// 无效默认值
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// 文件末尾
        /// </summary>
        EOF,

        /// <summary>
        /// C#代码
        /// </summary>
        CSharpCode,

        /// <summary>
        /// 文档
        /// </summary>
        Document,

        /// <summary>
        /// '{'
        /// </summary>
        OpenBrace,

        /// <summary>
        /// '}'
        /// </summary>
        CloseBrace,

        /// <summary>
        /// '['
        /// </summary>
        OpenBracket,

        /// <summary>
        /// ']'
        /// </summary>
        CloseBracket,

        /// <summary>
        /// ';'
        /// </summary>
        Semicolon,

        /// <summary>
        /// ,
        /// </summary>
        Comma,

        /// <summary>
        /// '='
        /// </summary>
        Equals,

        /// <summary>
        /// 标识符
        /// </summary>
        Identifier,

        /// <summary>
        /// 十进制32位整数
        /// </summary>
        Int32Literal,

        /// <summary>
        /// namespace
        /// </summary>
        NamespaceKeyword,

        /// <summary>
        /// using
        /// </summary>
        UsingKeyword,

        /// <summary>
        /// public
        /// </summary>
        PublicKeyword,

        /// <summary>
        /// internal
        /// </summary>
        InternalKeyword,

        /// <summary>
        /// final
        /// </summary>
        FinalKeyword,

        /// <summary>
        /// runtime
        /// </summary>
        RuntimeKeyword,
    
        /// <summary>
        /// compact
        /// </summary>
        CompactKeyword,

        /// <summary>
        /// ref
        /// </summary>
        RefKeyword,

        /// <summary>
        /// type
        /// </summary>
        TypeKeyword,

        /// <summary>
        /// .init_memory
        /// </summary>
        DotInitMemoryKeyword,

        /// <summary>
        /// .ctor
        /// </summary>
        DotCtorKeyword,

        /// <summary>
        /// .before
        /// </summary>
        DotBeforeKeyword,

        /// <summary>
        /// .after
        /// </summary>
        DotAfterKeyword,

        /// <summary>
        /// checkref
        /// </summary>
        CheckrefKeyword,

        /// <summary>
        /// utf8
        /// </summary>
        UTF8Keyword,

        /// <summary>
        /// unicode
        /// </summary>
        UnicodeKeyword,

        /// <summary>
        /// ascii
        /// </summary>
        ASCIIKeyword,

        /// <summary>
        /// fixed
        /// </summary>
        FixedKeyword,

        /// <summary>
        /// bool
        /// </summary>
        BoolKeyword,

        /// <summary>
        /// sbyte
        /// </summary>
        SByteKeyword,

        /// <summary>
        /// byte
        /// </summary>
        ByteKeyword,

        /// <summary>
        /// short
        /// </summary>
        ShortKeyword,

        /// <summary>
        /// ushort
        /// </summary>
        UShortKeyword,

        /// <summary>
        /// int
        /// </summary>
        IntKeyword,

        /// <summary>
        /// uint
        /// </summary>
        UIntKeyword,
        
        /// <summary>
        /// long
        /// </summary>
        LongKeyword,

        /// <summary>
        /// ulong
        /// </summary>
        ULongKeyword,

        /// <summary>
        /// float
        /// </summary>
        FloatKeyoword,

        /// <summary>
        /// double
        /// </summary>
        DoubleKeyword,

        /// <summary>
        /// decimal
        /// </summary>
        DecimalKeyword,

        /// <summary>
        /// char
        /// </summary>
        CharKeyword,

        /// <summary>
        /// string
        /// </summary>
        StringKeyword
    }
}
