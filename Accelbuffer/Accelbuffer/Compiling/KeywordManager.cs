using System.Collections.Generic;

namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个关键字管理器
    /// </summary>
    public class KeywordManager
    {
        /// <summary>
        /// 获取默认的关键字管理器
        /// </summary>
        public static KeywordManager Default { get; }

        static KeywordManager()
        {
            Default = new KeywordManager();

            Default.RegisterKeyword("package", TokenType.PackageKeyword, KeywordCategory.Other);
            Default.RegisterKeyword("using", TokenType.UsingKeyword, KeywordCategory.Other);
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
            Default.RegisterKeyword("nuint", TokenType.NUIntKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("nint", TokenType.NIntKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vuint", TokenType.VUIntKeyword, KeywordCategory.TypeKeyword);
            Default.RegisterKeyword("vint", TokenType.VIntKeyword, KeywordCategory.TypeKeyword);
        }

        private readonly Dictionary<string, TokenType> m_TypeKeywords;
        private readonly Dictionary<string, TokenType> m_StructModifierKeywords;
        private readonly Dictionary<string, TokenType> m_OtherKeywords;

        /// <summary>
        /// 初始化 KeywordManager
        /// </summary>
        public KeywordManager()
        {
            m_TypeKeywords = new Dictionary<string, TokenType>();
            m_StructModifierKeywords = new Dictionary<string, TokenType>();
            m_OtherKeywords = new Dictionary<string, TokenType>();
        }

        /// <summary>
        /// 注册一个关键字
        /// </summary>
        /// <param name="raw">关键字的字符串形式</param>
        /// <param name="type">关键字所属的标记类型</param>
        /// <param name="category">关键字的类别</param>
        protected void RegisterKeyword(string raw, TokenType type, KeywordCategory category)
        {
            switch (category)
            {
                case KeywordCategory.TypeKeyword:
                    m_TypeKeywords.Add(raw, type);
                    break;
                case KeywordCategory.StructModifier:
                    m_StructModifierKeywords.Add(raw, type);
                    break;
                default:
                    m_OtherKeywords.Add(raw, type);
                    break;
            }
        }

        /// <summary>
        /// 获取关键字是否在指定的类别中
        /// </summary>
        /// <param name="raw">关键字的字符串形式</param>
        /// <param name="category">查询的关键字类别</param>
        /// <returns>如果关键字在<paramref name="category"/>类别中，返回true，否则false</returns>
        public bool IsInCategory(string raw, KeywordCategory category)
        {
            switch (category)
            {
                case KeywordCategory.TypeKeyword:
                    return m_TypeKeywords.ContainsKey(raw);
                case KeywordCategory.StructModifier:
                    return m_StructModifierKeywords.ContainsKey(raw);
                default:
                    return m_OtherKeywords.ContainsKey(raw);
            }
        }

        /// <summary>
        /// 尝试匹配关键字的标记类型
        /// </summary>
        /// <param name="raw">关键字的字符串形式</param>
        /// <param name="type">关键字的标记类型</param>
        /// <param name="category">关键字的类别</param>
        /// <returns>如果匹配成功，返回true，否则false</returns>
        public bool TryMatchKeyword(string raw, out TokenType type, out KeywordCategory category)
        {
            if (m_OtherKeywords.TryGetValue(raw, out type))
            {
                category = KeywordCategory.Other;
                return true;
            }

            if (m_TypeKeywords.TryGetValue(raw, out type))
            {
                category = KeywordCategory.TypeKeyword;
                return true;
            }

            if (m_StructModifierKeywords.TryGetValue(raw, out type))
            {
                category = KeywordCategory.StructModifier;
                return true;
            }

            category = KeywordCategory.Other;
            return false;
        }
    }
}
