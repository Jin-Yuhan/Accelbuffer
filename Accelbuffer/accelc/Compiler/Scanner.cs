using accelc.Properties;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace accelc.Compiler
{
    public sealed class Scanner
    {
        private static readonly Dictionary<string, TokenType> s_Keywords;

        static Scanner()
        {
            s_Keywords = new Dictionary<string, TokenType>()
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
                [".ctor"]= TokenType.DotCtorKeyword,
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

            while (MoveNext())
            {
                Token token = default;
                char c = Current();

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
                        if (char.IsDigit(c))
                        {
                            token = GetInt32DecimalLiteral();
                        }
                        else if (char.IsLetter(c) || c == '_' || c == '@' || c == '.')
                        {
                            token = GetIdentifierOrKeyword();
                        }
                        else
                        {
                            LogError(string.Format(Resources.UnknownChar, c));
                        }
                        break;
                }

                tokens.Add(token);
            }

            tokens.Add(CreateToken(string.Empty, TokenType.EOF));//结束
            return tokens.ToArray();
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

            if (s_Keywords.ContainsKey(str))
                return CreateToken(str, s_Keywords[str]);
            else
                return CreateToken(str, TokenType.Identifier);
        }

        /// <summary>
        /// 获取十进制的32位整数
        /// </summary>
        /// <returns></returns>
        private Token GetInt32DecimalLiteral()
        {
            StringBuilder sb = new StringBuilder();
            bool isBin = false;
            bool isHex = false;

            do
            {
                char c = Current();

                switch (c)
                {
                    case 'x' when isBin:
                    case 'X' when isBin:
                    case 'x' when isHex:
                    case 'X' when isHex:
                    case 'b' when isBin:
                    case 'B' when isBin:
                        MoveBack();
                        goto End;

                    case 'a' when isHex:
                    case 'A' when isHex:
                    case 'b' when isHex:
                    case 'B' when isHex:
                    case 'c' when isHex:
                    case 'C' when isHex:
                    case 'd' when isHex:
                    case 'D' when isHex:
                    case 'e' when isHex:
                    case 'E' when isHex:
                    case 'f' when isHex:
                    case 'F' when isHex:
                        sb.Append(c);
                        break;

                    case 'x':
                    case 'X':
                        sb.Append(c);
                        isHex = true;
                        break;

                    case 'b':
                    case 'B':
                        sb.Append(c);
                        isBin = true;
                        break;

                    default:
                        if (char.IsDigit(c))
                        {
                            if (isBin && c != '0' && c != '1')
                            {
                                MoveBack();
                                goto End;
                            }
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
            return CreateToken(sb.ToString(), TokenType.Int32Literal);
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
                LogError(Resources.DocumentNotEnd);
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
                LogError(Resources.ExpectCloseBrace);
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
                m_ErrorWriter.WriteLine(Resources.SyntaxError);
                m_ErrorWriter.WriteLine("\t" + message);
                m_ErrorWriter.WriteLine("\t" + Resources.LineNumber + m_LineNumber.ToString());
                m_ErrorWriter.WriteLine("\t" + Resources.FilePath + m_FilePath);
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