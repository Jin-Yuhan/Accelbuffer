using System.Collections.Generic;

namespace asc.Compiler
{
    public sealed class KeywordManager
    {
        public static KeywordManager Default { get; }

        static KeywordManager()
        {
            Default = new KeywordManager();

            Default.RegisterKeyword("package", TokenType.PackageKeyword, KeywordCategory.Other);
            Default.RegisterKeyword("using", TokenType.UsingKeyword, KeywordCategory.Other);
            //Default.RegisterKeyword("as", TokenType.AsKeyword, KeywordCategory.Other);
            Default.RegisterKeyword("struct", TokenType.StructKeyword, KeywordCategory.Other);
            Default.RegisterKeyword("about", TokenType.AboutKeyword, KeywordCategory.Other);
            Default.RegisterKeyword("var", TokenType.VarKeyword, KeywordCategory.Other);
            Default.RegisterKeyword("obsolete", TokenType.ObsoleteKeyword, KeywordCategory.Other);

            Default.RegisterKeyword("public", TokenType.PublicKeyword, KeywordCategory.StructModifier);
            Default.RegisterKeyword("internal", TokenType.InternalKeyword, KeywordCategory.StructModifier);
            Default.RegisterKeyword("private", TokenType.PrivateKeyword, KeywordCategory.StructModifier);
            Default.RegisterKeyword("protected", TokenType.ProtectedKeyword, KeywordCategory.StructModifier);
            Default.RegisterKeyword("final", TokenType.FinalKeyword, KeywordCategory.StructModifier);
            Default.RegisterKeyword("ref", TokenType.RefKeyword, KeywordCategory.StructModifier);

            Default.RegisterKeyword("boolean", TokenType.BooleanKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("uint8", TokenType.UInt8Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("int8", TokenType.Int8Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("uint16", TokenType.UInt16Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("int16", TokenType.Int16Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("uint32", TokenType.UInt32Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("int32", TokenType.Int32Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("uint64", TokenType.UInt64Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("int64", TokenType.Int64Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("float32", TokenType.Float32Keyoword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("float64", TokenType.Float64Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("float128", TokenType.Float128Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("char", TokenType.CharKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("string", TokenType.StringKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("uintptr", TokenType.UIntPtrKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("intptr", TokenType.IntPtrKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vuint", TokenType.VUIntKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vint", TokenType.VIntKeyword, KeywordCategory.TypeKeyword);

#if UNITY
            Default.RegisterKeyword("vector2", TokenType.Vector2Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vector3", TokenType.Vector3Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vector4", TokenType.Vector4Keyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vector2int", TokenType.Vector2IntKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vector3int", TokenType.Vector3IntKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("quaternion", TokenType.QuaternionKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("color", TokenType.ColorKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("color32", TokenType.Color32Keyword, KeywordCategory.TypeKeyword);
#endif
        }

        private readonly Dictionary<string, TokenType> m_TypeKeywords;
        private readonly Dictionary<string, TokenType> m_StructModifierKeywords;
        private readonly Dictionary<string, TokenType> m_OtherKeywords;

        public KeywordManager()
        {
            m_TypeKeywords = new Dictionary<string, TokenType>();
            m_StructModifierKeywords = new Dictionary<string, TokenType>();
            m_OtherKeywords = new Dictionary<string, TokenType>();
        }

        public bool RegisterKeyword(string raw, TokenType type, KeywordCategory category) => category switch
        {
            KeywordCategory.TypeKeyword => m_TypeKeywords.TryAdd(raw, type),
            KeywordCategory.StructModifier => m_StructModifierKeywords.TryAdd(raw, type),
            _ => m_OtherKeywords.TryAdd(raw, type)
        };

        public bool IsInCategory(string raw, KeywordCategory category) => category switch
        {
            KeywordCategory.TypeKeyword => m_TypeKeywords.ContainsKey(raw),
            KeywordCategory.StructModifier => m_StructModifierKeywords.ContainsKey(raw),
            _ => m_OtherKeywords.ContainsKey(raw)
        };

        public bool TryMatchKeyword(string raw, out TokenType type) =>
            m_TypeKeywords.TryGetValue(raw, out type) || m_StructModifierKeywords.TryGetValue(raw, out type) || m_OtherKeywords.TryGetValue(raw, out type);
    }
}
