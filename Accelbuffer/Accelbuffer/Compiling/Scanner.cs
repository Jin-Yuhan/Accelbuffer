using Accelbuffer.Properties;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个字符扫描器，用于将文件转化为一系列标记
    /// </summary>
    public sealed class Scanner
    {
        private readonly KeywordManager m_KeywordManager;
        private readonly ErrorWriter m_Writer;
        private readonly StringBuilder m_StrBuilder;
        private StreamReader m_Reader;
        private string m_FilePath;
        private int m_Line;
        private int m_Column;

        /// <summary>
        /// 初始化Scanner
        /// </summary>
        /// <param name="writer">错误编写器</param>
        /// <param name="manager">关键字管理器</param>
        public Scanner(ErrorWriter writer, KeywordManager manager)
        {
            m_KeywordManager = manager;
            m_Writer = writer;
            m_StrBuilder = new StringBuilder();
        }

        /// <summary>
        /// 重置Scanner
        /// </summary>
        /// <param name="filePath">扫描的文件路径</param>
        /// <param name="reader">文件内容读取器</param>
        public void Reset(string filePath, StreamReader reader)
        {
            m_Reader = reader;
            m_FilePath = filePath;
        }

        /// <summary>
        /// 扫描文件并转换为一系列的标记
        /// </summary>
        /// <returns>从文件中读取的标记</returns>
        public Token[] ToTokens()
        {
            if (m_Reader == null)
            {
                return null;
            }

            m_Line = 1;
            m_Column = 0;

            List<Token> tokens = new List<Token>(25);
            m_Reader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (HasNext())
            {
                Token token = default;
                m_StrBuilder.Clear();
                char c = Next();

                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\0':
                    case '\r':
                    case '\n':
                        continue;

                    case '{':
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
                    case '(':
                        token = CreateToken("(", TokenType.OpenParen);
                        break;
                    case ')':
                        token = CreateToken(")", TokenType.CloseParen);
                        break;
                    case '|':
                        token = CreateToken("|", TokenType.Bar);
                        break;
                    case '*':
                        token = CreateToken("*", TokenType.Asterisk);
                        break;
                    case '?':
                        token = CreateToken("?", TokenType.Question);
                        break;
                    case ':':
                        token = CreateToken(":", TokenType.Colon);
                        break;
                    case ';':
                        token = CreateToken(";", TokenType.Semicolon);
                        break;
                    case '.':
                        token = CreateToken(".", TokenType.Dot);
                        break;
                    case ',':
                        token = CreateToken(",", TokenType.Comma);
                        break;
                    case '<':
                        token = CreateToken("<", TokenType.LessThan);
                        break;
                    case '>':
                        token = CreateToken(">", TokenType.GreaterThan);
                        break;

                    case '-' when ExpectNextChar('-')://文档
                        Next();
                        token = GetDocument();
                        break;

                    case '/' when ExpectNextChar('/')://单行注释
                        Next();
                        IgnoreSingleLineComment();
                        continue;

                    default:
                        if (char.IsLetter(c) || c == '_')
                        {
                            token = GetIdentifierOrKeyword(c);
                        }
                        else if (char.IsDigit(c))
                        {
                            token = GetIntLiteral(c);
                        }
                        else
                        {
                            LogError(Resources.Error_AS001_InvalidChar, c);
                        }
                        break;
                }

                tokens.Add(token);
            }

            return tokens.ToArray();
        }

        private Token GetIntLiteral(char first)
        {
            m_StrBuilder.Append(first);

            while (true)
            {
                char? c = PeekNext();

                if (c != null && char.IsDigit(c.Value))
                {
                    Next();
                    m_StrBuilder.Append(c.Value);
                }
                else
                {
                    break;
                }
            }

            string str = m_StrBuilder.ToString();
            return CreateToken(str, TokenType.IntLiteral);
        }

        private Token GetIdentifierOrKeyword(char first)
        {
            m_StrBuilder.Append(first);

            while (true)
            {
                char? c = PeekNext();

                if (c != null && (char.IsLetterOrDigit(c.Value) || c == '_'))
                {
                    Next();
                    m_StrBuilder.Append(c.Value);
                }
                else
                {
                    break;
                }
            }

            string str = m_StrBuilder.ToString();

            return m_KeywordManager.TryMatchKeyword(str, out TokenType type, out _)
                ? CreateToken(str, type)
                : CreateToken(str, TokenType.Identifier);
        }

        private Token GetDocument()
        {
            bool end = false;

            while (HasNext())
            {
                char c = Next();

                if(c == '-' && ExpectNextChar('-'))
                {
                    end = true;
                    Next();
                    break;
                }

                m_StrBuilder.Append(c);
            }

            if (!end)
            {
                LogError(Resources.Error_AS002_MissingDocEnd);
            }
            return CreateToken(m_StrBuilder.ToString().Trim(), TokenType.Document);
        }

        private void IgnoreSingleLineComment()
        {
            while (HasNext() && Next() != '\n') ;
        }

        private char Next()
        {
            char c = (char)m_Reader.Read();

            if (c == '\n')
            {
                m_Line++;
                m_Column = 0;
            }
            else if (c == '\t')
            {
                m_Column += 4;
            }
            else if (c != '\0')
            {
                m_Column++;
            }

            return c;
        }

        private bool HasNext()
        {
            return !(m_Reader.EndOfStream || m_Writer.IsError);
        }

        private char? PeekNext()
        {
            int result = m_Reader.Peek();
            return result == -1 ? (char?)null : (char)result;
        }

        private bool ExpectNextChar(char expect)
        {
            return m_Reader.Peek() == expect;
        }

        private Token CreateToken(string raw, TokenType type)
        {
            return new Token(raw, type, m_Line, m_Column + 1, m_FilePath);
        }

        private void LogError(string msg)
        {
            m_Writer.LogError(msg, m_FilePath, m_Line, m_Column + 1);
        }

        private void LogError(string msg, params object[] args)
        {
            m_Writer.LogError(msg, m_FilePath, m_Line, m_Column + 1, args);
        }
    }
}
