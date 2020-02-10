namespace asc.Compiler
{
    public enum TokenType
    {
        /// <summary>
        /// 无效值
        /// </summary>
        Invalid = 0,

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
        /// '('
        /// </summary>
        OpenParen,
        /// <summary>
        /// ')'
        /// </summary>
        CloseParen,

        /// <summary>
        /// ';'
        /// </summary>
        Semicolon,

        /// <summary>
        /// '|'
        /// </summary>
        Bar,

        /// <summary>
        /// '*'
        /// </summary>
        Asterisk,

        /// <summary>
        /// '?'
        /// </summary>
        Question,

        /// <summary>
        /// ':'
        /// </summary>
        Colon,

        /// <summary>
        /// '.'
        /// </summary>
        Dot,

        /// <summary>
        /// ','
        /// </summary>
        Comma,

        /// <summary>
        /// '&gt;'
        /// </summary>
        GreaterThan,

        /// <summary>
        /// '&lt;'
        /// </summary>
        LessThan,

        /// <summary>
        /// 整数值
        /// </summary>
        IntLiteral,

        /// <summary>
        /// 标识符
        /// </summary>
        Identifier,

        /// <summary>
        /// package
        /// </summary>
        PackageKeyword,

        /// <summary>
        /// using
        /// </summary>
        UsingKeyword,

        ///// <summary>
        ///// as
        ///// </summary>
        //AsKeyword,

        /// <summary>
        /// public
        /// </summary>
        PublicKeyword,

        /// <summary>
        /// internal
        /// </summary>
        InternalKeyword,

        /// <summary>
        /// private
        /// </summary>
        PrivateKeyword,

        /// <summary>
        /// protected
        /// </summary>
        ProtectedKeyword,

        /// <summary>
        /// final
        /// </summary>
        FinalKeyword,

        /// <summary>
        /// ref
        /// </summary>
        RefKeyword,

        /// <summary>
        /// struct
        /// </summary>
        StructKeyword,

        /// <summary>
        /// about
        /// </summary>
        AboutKeyword,

        /// <summary>
        /// var
        /// </summary>
        VarKeyword,

        /// <summary>
        /// obsolete
        /// </summary>
        ObsoleteKeyword,

        /// <summary>
        /// boolean
        /// </summary>
        BooleanKeyword,

        /// <summary>
        /// int8
        /// </summary>
        Int8Keyword,

        /// <summary>
        /// uint8
        /// </summary>
        UInt8Keyword,

        /// <summary>
        /// int16
        /// </summary>
        Int16Keyword,

        /// <summary>
        /// uint16
        /// </summary>
        UInt16Keyword,

        /// <summary>
        /// int32
        /// </summary>
        Int32Keyword,

        /// <summary>
        /// uint32
        /// </summary>
        UInt32Keyword,

        /// <summary>
        /// int64
        /// </summary>
        Int64Keyword,

        /// <summary>
        /// uint64
        /// </summary>
        UInt64Keyword,

        /// <summary>
        /// float32
        /// </summary>
        Float32Keyoword,

        /// <summary>
        /// float64
        /// </summary>
        Float64Keyword,

        /// <summary>
        /// float128
        /// </summary>
        Float128Keyword,

        /// <summary>
        /// char
        /// </summary>
        CharKeyword,

        /// <summary>
        /// string
        /// </summary>
        StringKeyword,

        /// <summary>
        /// intptr
        /// </summary>
        IntPtrKeyword,

        /// <summary>
        /// uintptr
        /// </summary>
        UIntPtrKeyword,

        /// <summary>
        /// vint
        /// </summary>
        VIntKeyword,

        /// <summary>
        /// vuint
        /// </summary>
        VUIntKeyword,

#if UNITY
        /// <summary>
        /// vector2
        /// </summary>
        Vector2Keyword,

        /// <summary>
        /// vector3
        /// </summary>
        Vector3Keyword,

        /// <summary>
        /// vector4
        /// </summary>
        Vector4Keyword,

        /// <summary>
        /// vector2int
        /// </summary>
        Vector2IntKeyword,

        /// <summary>
        /// vector3int
        /// </summary>
        Vector3IntKeyword,

        /// <summary>
        /// quaternion
        /// </summary>
        QuaternionKeyword,

        /// <summary>
        /// color
        /// </summary>
        ColorKeyword,

        /// <summary>
        /// color32
        /// </summary>
        Color32Keyword
#endif
    }
}
