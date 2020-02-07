using asc.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace asc.Compiler
{
    public class Scanner : IDisposable
    {
        private readonly KeywordManager m_KeywordManager;
        private readonly StreamReader m_Reader;
        private readonly ErrorWriter m_Writer;
        private readonly StringBuilder m_StrBuilder;
        private readonly string m_FilePath;
        private int m_Line;
        private int m_Column;

        public Scanner(string filePath, ErrorWriter writer, KeywordManager manager)
        {
            m_KeywordManager = manager;
            m_Reader = new StreamReader(filePath);
            m_Writer = writer;
            m_StrBuilder = new StringBuilder();
            m_FilePath = filePath;
            m_Line = 1;
            m_Column = 0;
            m_Writer.OnAfterLogError += Dispose;
        }

        public void Dispose()
        {
            m_Reader.Dispose();
            m_Writer.OnAfterLogError -= Dispose;
        }

        public Token[] ToTokens()
        {
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

            if (m_KeywordManager.TryMatchKeyword(str, out TokenType type))
                return CreateToken(str, type);
            else
                return CreateToken(str, TokenType.Identifier);
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
            return !m_Reader.EndOfStream;
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
            LogError(msg, Array.Empty<object>());
        }

        private void LogError(string msg, params object[] args)
        {
            m_Writer.LogError(msg, m_FilePath, m_Line, m_Column + 1, args);
        }
    }
}
