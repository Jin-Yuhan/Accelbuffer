using accelc.Properties;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace accelc.Compiler
{
    public sealed class Scanner
    {
        public static readonly Dictionary<string, TokenType> Keywords;

        static Scanner()
        {
            Keywords = new Dictionary<string, TokenType>()
            {
                ["namespace"]= TokenType.NamespaceKeyword,
                ["using"]= TokenType.UsingKeyword,
                ["public"]= TokenType.PublicKeyword,
                ["internal"]= TokenType.InternalKeyword,
                ["final"]= TokenType.FinalKeyword,
                ["runtime"]= TokenType.RuntimeKeyword,
                ["compact"]= TokenType.CompactKeyword,
                ["ref"]= TokenType.RefKeyword,
                ["type"]= TokenType.TypeKeyword,
                [".init_memory"]= TokenType.DotInitMemoryKeyword,
                [".before"]= TokenType.DotBeforeKeyword,
                [".after"]= TokenType.DotAfterKeyword,
                ["checkref"]= TokenType.CheckrefKeyword,
                ["utf8"]= TokenType.UTF8Keyword,
                ["unicode"]= TokenType.UnicodeKeyword,
                ["ascii"]= TokenType.ASCIIKeyword,
                ["fixed"]= TokenType.FixedKeyword,

                ["bool"]= TokenType.BoolKeyword,
                ["sbyte"]= TokenType.SByteKeyword,
                ["byte"]= TokenType.ByteKeyword,
                ["short"]= TokenType.ShortKeyword,
                ["ushort"]= TokenType.UShortKeyword,
                ["int"]= TokenType.IntKeyword,
                ["uint"]= TokenType.UIntKeyword,
                ["long"]= TokenType.LongKeyword,
                ["ulong"]= TokenType.ULongKeyword,
                ["float"]= TokenType.FloatKeyoword,
                ["double"]= TokenType.DoubleKeyword,
                ["decimal"]= TokenType.DecimalKeyword,
                ["char"]= TokenType.CharKeyword,
                ["string"]= TokenType.StringKeyword,
            };
        }

        private readonly TextWriter m_ErrorWriter;
        private readonly string m_FilePath;
        private readonly string m_Source;
        public bool IsError { get; private set; }
        private int m_Index;
        private int m_LineNumber;

        public Scanner(TextWriter errorWriter, string filePath)
        {
            m_ErrorWriter = errorWriter;
            m_FilePath = filePath;
            m_Source = File.ReadAllText(filePath);
            IsError = false;
            m_Index = -1;
            m_LineNumber = 1;
        }

        public Token[] ToTokens()
        {
            List<Token> tokens = new List<Token>(25);
            bool lastIsEquals = false;

            while (MoveNext())
            {
                Token token = default;
                char c = Current();

                if (lastIsEquals)
                {
                    token = GetDefaultValue();
                    lastIsEquals = false;
                }
                else
                {
                    switch (c)
                    {
                        case ' ':
                        case '\t':
                        case '\0':
                        case '\r':
                            continue;
                        case '\n':
                            m_LineNumber++;
                            continue;

                        case '{':
                            if (tokens.Count > 0)
                            {
                                TokenType type = tokens[tokens.Count - 1].Type;
                                if (type == TokenType.DotBeforeKeyword || type == TokenType.DotAfterKeyword)
                                {
                                    token = GetCSharpCode();
                                    break;
                                }
                            }

                            token = CreateToken("{", TokenType.OpenBrace);
                            break;
                        case '}':
                            token = CreateToken("}", TokenType.CloseBrace);
                            break;
                        case '[':
                            token = CreateToken("[", TokenType.OpenBracket);
                            break;
                        case ']':
                            token = CreateToken("]", TokenType.CloseBracket);
                            break;
                        case ';':
                            token = CreateToken(";", TokenType.Semicolon);
                            break;
                        case ',':
                            token = CreateToken(",", TokenType.Comma);
                            break;
                        case '=':
                            token = CreateToken("=", TokenType.Equals);
                            lastIsEquals = true;
                            break;

                        case '-' when ExpectNextChar(1, '-')://文档
                            MoveNext();
                            token = GetDocument();
                            break; ;

                        case '/' when ExpectNextChar(1, '/')://单行注释
                            MoveNext();
                            IgnoreSingleLineComment();
                            continue;

                        default:
                            if (char.IsLetter(c) || c == '_' || c == '@' || c == '.')
                            {
                                token = GetIdentifierOrKeyword();
                            }
                            else
                            {
                                LogError(string.Format(Resources.Error_A1029_UnknownChar, c));
                            }
                            break;
                    }
                }

                tokens.Add(token);
            }

            tokens.Add(CreateToken(string.Empty, TokenType.EOF));//结束
            return tokens.ToArray();
        }

        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <returns></returns>
        private Token GetDefaultValue()
        {
            StringBuilder sb = new StringBuilder();

            do
            {
                char c = Current();

                if (c == ';')
                {
                    MoveBack();
                    break;
                }

                if (c == '\n')
                {
                    m_LineNumber++;
                }

                sb.Append(c);

            } while (MoveNext());

            return CreateToken(sb.ToString(), TokenType.DefaultValue);
        }

        /// <summary>
        /// 获取标识符或者关键字
        /// </summary>
        /// <returns></returns>
        private Token GetIdentifierOrKeyword()
        {
            StringBuilder sb = new StringBuilder();
            bool hasGeneric = false;//是否有过泛型
            bool isGeneric = false;

            do
            {
                char c = Current();

                switch (c)
                {
                    case '<' when !hasGeneric:
                        hasGeneric = true;
                        isGeneric = true;
                        sb.Append(c);
                        break;

                    case '>' when isGeneric:
                        isGeneric = false;
                        sb.Append(c);
                        break;

                    case ',' when isGeneric:
                    case ' ' when isGeneric:
                    case '\t' when isGeneric:
                        sb.Append(c);
                        break;

                    default:
                        if (char.IsLetterOrDigit(c) || c == '@' || c == '_' || c == '.')
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            MoveBack();
                            goto End;
                        }
                        break;
                }

            } while (MoveNext());

        End:

            string str = sb.ToString();

            if (Keywords.ContainsKey(str))
                return CreateToken(str, Keywords[str]);
            else
                return CreateToken(str, TokenType.Identifier);
        }

        /// <summary>
        /// 获取文档
        /// </summary>
        private Token GetDocument()
        {
            StringBuilder sb = new StringBuilder();
            bool end = false;

            while (MoveNext())
            {
                char c = Current();

                switch (c)
                {
                    case '\n':
                        m_LineNumber++;
                        break;
                    case '-' when ExpectNextChar(1, '-'):
                        end = true;
                        MoveNext();
                        goto End;
                }

                sb.Append(c);
            }

        End:
            if (!end)
            {
                LogError(Resources.Error_A1013_ExpectSubSub);
            }
            return CreateToken(sb.ToString().Trim(), TokenType.Document);
        }

        /// <summary>
        /// 获取C#代码
        /// </summary>
        /// <returns></returns>
        private Token GetCSharpCode()
        {
            StringBuilder sb = new StringBuilder();
            int depth = 1;
            bool isChar = false;
            bool isString = false;

            while (MoveNext())
            {
                char c = Current();

                switch (c)
                {
                    case '\n':
                        m_LineNumber++;
                        break;

                    case '"' when !isChar && !isString:
                        isString = true;
                        break;

                    case '"' when !isChar && isString && sb[sb.Length - 1] != '\\':
                        isString = false;
                        break;

                    case '\'' when !isString && !isChar:
                        isChar = true;
                        break;

                    case '\'' when !isString && isChar && sb[sb.Length - 1] != '\\':
                        isChar = false;
                        break;

                    case '{' when !isChar && !isString:
                        depth++;
                        break;

                    case '}' when !isChar && !isString:
                        depth--;
                        break;
                }

                if (depth == 0)
                {
                    break;
                }

                sb.Append(c);
            }

            if (depth != 0)
            {
                LogError(Resources.Error_A1003_ExpectCloseBrace);
            }

            return CreateToken(sb.ToString().Trim(), TokenType.CSharpCode);
        }

        /// <summary>
        /// 忽略单行注释
        /// </summary>
        private void IgnoreSingleLineComment()
        {
            while (MoveNext())
            {
                if (Current() == '\n')
                {
                    m_LineNumber++;
                    break;
                }
            }
        }

        private Token CreateToken(string raw, TokenType type)
        {
            return new Token(raw, type, m_LineNumber, m_FilePath);
        }

        private bool ExpectNextChar(int nextCount, char c)
        {
            int i = m_Index + nextCount;
            if (i >= m_Source.Length || i < 0) return false;
            return m_Source[m_Index + nextCount] == c;
        }

        private bool MoveNext()
        {
            bool next = m_Index < m_Source.Length - 1 && !IsError;
            if (next) m_Index++;
            return next;
        }

        private bool MoveBack()
        {
            bool back = m_Index > -1 && !IsError;
            if (back) m_Index--;
            return back;
        }

        private void LogError(string message)
        {
            if (!IsError)
            {
                m_ErrorWriter.WriteLine(message);
                m_ErrorWriter.WriteLine(Resources.FilePath + m_FilePath);
                m_ErrorWriter.WriteLine(Resources.LineNumber + m_LineNumber.ToString());
                IsError = true;
            }
        }

        private char Current()
        {
            if (m_Index < 0 || m_Index >= m_Source.Length)
            {
                return default;
            }
            return m_Source[m_Index];
        }
    }
}