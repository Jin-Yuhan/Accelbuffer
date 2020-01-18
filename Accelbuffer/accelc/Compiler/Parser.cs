using accelc.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace accelc.Compiler
{
    public sealed class Parser
    {
        private static readonly HashSet<string> s_Reserved;

        static Parser()
        {
            s_Reserved = new HashSet<string>();

            string reserved = "bool|byte|sbyte|char|decimal|double|float|int|uint|long|ulong|short|ushort|object|string|void|var|abstract|as|base|break|case|catch|checked|class|const|continue|default|delegate|do|else|enum|event|explicit|extern|finally|fixed|for|foreach|goto|if|implicit|in|interface|internal|is|lock|nameof|namespace|new|null|operator|out|override|params|private|protected|public|readonly|ref|return|sealed|sizeof|stackalloc|static|string|struct|switch|this|throw|try|typeof|unchecked|unsafe|using|virtual|volatile|while";

            string[] words = reserved.Split('|');

            for (int i = 0; i < words.Length; i++)
            {
                s_Reserved.Add(words[i]);
            }
        }

        private readonly TextWriter m_ErrorWriter;
        private readonly Token[] m_Tokens;
        public bool IsError { get; private set; }
        private int m_Index;
        private bool m_HasNamespace;

        public Parser(TextWriter errorWriter, Token[] tokens)
        {
            m_ErrorWriter = errorWriter;
            m_Tokens = tokens;
            IsError = false;
            m_Index = -1;
            m_HasNamespace = false;
        }

        public Declaration[] GetDeclarations()
        {
            List<Declaration> declarations = new List<Declaration>(25)
            {
                new UsingDeclaration { Name = Resources.AccelbufferNamespace },
                new UsingDeclaration { Name = Resources.AccelbufferInjectionNamespace },
                new UsingDeclaration { Name = Resources.AccelbufferMemoryNamespace },
                new UsingDeclaration { Name = Resources.AccelbufferTextNamespace }
            };

            while (MoveNext())
            {
                Token token = Current();
                Declaration declaration;

                switch (token.Type)
                {
                    case TokenType.NamespaceKeyword:
                        declaration = GetNamespace();
                        break;
                    case TokenType.UsingKeyword:
                        declaration = GetUsing();
                        break;
                    case TokenType.Semicolon:
                    case TokenType.Document:
                        continue;
                    default:
                        MoveBack();
                        goto TypeDef;
                }

                declarations.Add(declaration);
            }

        TypeDef:

            while (MoveNext())
            {
                Token token = Current();
                Declaration declaration = null;

                switch (token.Type)
                {
                    case TokenType.NamespaceKeyword:
                        LogError(Resources.Error_A1022_UnexpectedNamespace, token);
                        break;
                    case TokenType.UsingKeyword:
                        LogError(Resources.Error_A1023_UnexpectedUsing, token);
                        break;

                    case TokenType.Semicolon:
                    case TokenType.Document:
                        continue;

                    case TokenType.PublicKeyword:
                    case TokenType.InternalKeyword:
                    case TokenType.FinalKeyword:
                    case TokenType.RuntimeKeyword:
                    case TokenType.CompactKeyword:
                    case TokenType.RefKeyword:
                    case TokenType.TypeKeyword:
                        MoveBack();
                        declaration = GetTypeDef();
                        break;

                    case TokenType.EOF:
                        goto End;

                    default:
                        LogError(string.Format(Resources.Error_A1018_InvalidToken, token.Raw), token);
                        break;
                }

                declarations.Add(declaration);
            }

        End:
            return declarations.ToArray();
        }

        private Declaration GetNamespace()
        {
            if (m_HasNamespace)
            {
                LogError(Resources.Error_A1019_MultiNamespace, Current());
            }
            else if (ExpectNextTokenType(1, TokenType.Identifier))
            {
                m_HasNamespace = true;
                MoveNext();
                string name = Current().Raw;
                CheckIdentifier(name);

                if (ExpectNextTokenType(1, TokenType.Semicolon))
                {
                    MoveNext();
                    return new NamespaceDeclaration { Name = name };
                }

                LogError(Resources.Error_A1010_ExpectSemicolon, Current());
            }
            else
            {
                LogError(Resources.Error_A1007_ExpectIdentifier, Current());
            }

            return null;
        }

        private Declaration GetUsing()
        {
            if (ExpectNextTokenType(1, TokenType.Identifier))
            {
                MoveNext();
                string name = Current().Raw;
                CheckIdentifier(name);

                if (ExpectNextTokenType(1, TokenType.Semicolon))
                {
                    MoveNext();
                    return new UsingDeclaration { Name = name };
                }

                LogError(Resources.Error_A1010_ExpectSemicolon, Current());
            }
            else
            {
                LogError(Resources.Error_A1007_ExpectIdentifier, Current());
            }

            return null;
        }

        private Declaration GetTypeDef()
        {
            bool isPublic = false;
            bool isInternal = false;
            bool isRef = false;
            bool isFinal = false;
            bool isRuntime = false;
            bool isCompact = false;
            string name;
            string doc = null;

            Token docToken = Current();

            if (docToken.Type == TokenType.Document)
            {
                doc = docToken.Raw;
            }

            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.PublicKeyword when isPublic:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.PublicKeyword when isInternal:
                        LogError(string.Format(Resources.Error_A1020_MultipleAccessKeyword, token.Raw), token);
                        break;
                    case TokenType.PublicKeyword:
                        isPublic = true;
                        break;

                    case TokenType.InternalKeyword when isInternal:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.InternalKeyword when isPublic:
                        LogError(string.Format(Resources.Error_A1020_MultipleAccessKeyword, token.Raw), token);
                        break;
                    case TokenType.InternalKeyword:
                        isInternal = true;
                        break;

                    case TokenType.RefKeyword when isRef:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.RefKeyword:
                        isRef = true;
                        break;

                    case TokenType.FinalKeyword when isFinal:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.FinalKeyword:
                        isFinal = true;
                        break;

                    case TokenType.RuntimeKeyword when isRuntime:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.RuntimeKeyword:
                        isRuntime = true;
                        break;

                    case TokenType.CompactKeyword when isCompact:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.CompactKeyword:
                        isCompact = true;
                        break;

                    case TokenType.TypeKeyword:
                        goto NameDef;
                }
            }

            LogError(Resources.Error_A1011_ExpectTypeKeyword, Current());
            return null;

        NameDef:

            if (isFinal && !isRef)
            {
                LogError(Resources.Error_A1016_InvalidFinal, Current());
            }

            if (isRuntime && isInternal)
            {
                LogError(Resources.Error_A1030_InvalidRuntime, Current());
            }

            if (ExpectNextTokenType(1, TokenType.Identifier))
            {
                MoveNext();
                name = Current().Raw;
                CheckIdentifier(name);

                if (ExpectNextTokenType(1, TokenType.OpenBrace))
                {
                    MoveNext();

                    Declaration[] declarations = GetMembers(isRef, isCompact, out InitMemoryDeclaration initMemory, out MessageDeclaration before, out MessageDeclaration after);

                    while (MoveNext())
                    {
                        Token token = Current();

                        if (token.Type == TokenType.Semicolon || token.Type == TokenType.Document)
                        {
                            continue;
                        }

                        MoveBack();
                        break;
                    }

                    if (ExpectNextTokenType(1, TokenType.CloseBrace))
                    {
                        MoveNext();

                        return new TypeDeclaration
                        {
                            IsInternal = isInternal,
                            IsFinal = isFinal,
                            IsRuntime = isRuntime,
                            IsRef = isRef,
                            IsCompact = isCompact,
                            Name = name,
                            Doc = doc,
                            Declarations = declarations,
                            InitMemory = initMemory,
                            Before = before,
                            After = after
                        };
                    }
                    else
                    {
                        LogError(Resources.Error_A1003_ExpectCloseBrace, Current());
                    }
                }
                else
                {
                    LogError(Resources.Error_A1009_ExpectOpenBrace, Current());
                }
            }
            else
            {
                LogError(Resources.Error_A1007_ExpectIdentifier, Current());
            }

            return null;
        }

        private Declaration[] GetMembers(bool isTypeRef, bool isCompact, out InitMemoryDeclaration initMemory, out MessageDeclaration before, out MessageDeclaration after)
        {
            List<Declaration> declarations = new List<Declaration>(5);
            initMemory = null;
            before = null;
            after = null;
          
            while (MoveNext())
            {
                Token token = Current();
                Declaration declaration = null;

                switch (token.Type)
                {
                    case TokenType.Semicolon:
                    case TokenType.Document:
                        continue;

                    case TokenType.DotInitMemoryKeyword when initMemory != null:
                        LogError(Resources.Error_A1031_InvalidDotInitMemory, token);
                        break;

                    case TokenType.DotInitMemoryKeyword:
                        initMemory = GetInitMemory();
                        continue;

                    case TokenType.DotBeforeKeyword when before != null:
                        LogError(Resources.Error_A1032_InvalidDotBefore, token);
                        break;

                    case TokenType.DotBeforeKeyword:
                        before = GetBefore();
                        continue;

                    case TokenType.DotAfterKeyword when after != null:
                        LogError(Resources.Error_A1033_InvalidDotAfter, token);
                        break;

                    case TokenType.DotAfterKeyword:
                        after = GetAfter();
                        continue;

                    case TokenType.CheckrefKeyword when isCompact:
                        LogError(Resources.Error_A1024_InvalidCheckRefWhenCompact, token);
                        break;

                    case TokenType.CheckrefKeyword:
                    case TokenType.FixedKeyword:
                    case TokenType.UnicodeKeyword:
                    case TokenType.UTF8Keyword:
                    case TokenType.ASCIIKeyword:
                    case TokenType.SByteKeyword:
                    case TokenType.ByteKeyword:
                    case TokenType.ShortKeyword:
                    case TokenType.UShortKeyword:
                    case TokenType.IntKeyword:
                    case TokenType.UIntKeyword:
                    case TokenType.LongKeyword:
                    case TokenType.ULongKeyword:
                    case TokenType.BoolKeyword:
                    case TokenType.FloatKeyoword:
                    case TokenType.DoubleKeyword:
                    case TokenType.DecimalKeyword:
                    case TokenType.CharKeyword:
                    case TokenType.StringKeyword:
                    case TokenType.Identifier:
                        MoveBack();
                        declaration = GetField(isTypeRef);
                        break;

                    case TokenType.PublicKeyword:
                    case TokenType.InternalKeyword:
                    case TokenType.FinalKeyword:
                    case TokenType.RuntimeKeyword:
                    case TokenType.CompactKeyword:
                    case TokenType.RefKeyword:
                    case TokenType.TypeKeyword:
                        MoveBack();
                        declaration = GetTypeDef();
                        break;

                    default:
                        MoveBack();
                        goto End;
                }

                declarations.Add(declaration);
            }
        End:
            return declarations.ToArray();
        }

        private InitMemoryDeclaration GetInitMemory()
        {
            if (ExpectNextTokenType(1, TokenType.Equals))
            {
                MoveNext();

                if (ExpectNextTokenType(1, TokenType.DefaultValue))
                {
                    MoveNext();

                    if (!int.TryParse(Current().Raw.Trim(), out int value))
                    {
                        LogError(Resources.Error_A1008_ExpectInt32Literal, Current());
                    }

                    if (ExpectNextTokenType(1, TokenType.Semicolon))
                    {
                        MoveNext();
                        return new InitMemoryDeclaration { Value = value };
                    }
                    else
                    {
                        LogError(Resources.Error_A1010_ExpectSemicolon, Current());
                    }
                }
                else
                {
                    LogError(Resources.Error_A1008_ExpectInt32Literal, Current());
                }
            }
            else
            {
                LogError(Resources.Error_A1005_ExpectEquals, Current());
            }

            return null;
        }

        private MessageDeclaration GetBefore()
        {
            if (ExpectNextTokenType(1, TokenType.CSharpCode))
            {
                MoveNext();
                string code = Current().Raw;
                return new MessageDeclaration { Code = code };
            }
            else
            {
                LogError(Resources.Error_A1004_ExpectCShapCode, Current());
            }

            return null;
        }

        private MessageDeclaration GetAfter()
        {
            if(ExpectNextTokenType(1, TokenType.CSharpCode))
            {
                MoveNext();
                string code = Current().Raw;
                return new MessageDeclaration { Code = code };
            }
            else
            {
                LogError(Resources.Error_A1004_ExpectCShapCode, Current());
            }

            return null;
        }

        private Declaration GetField(bool isRefType)
        {
            bool isFixed = false;
            bool isUnicode = false;
            bool isASCII = false;
            bool isUTF8 = false;
            bool isCheckref = false;
            string type;
            string name;
            string doc = null;
            string assign = null;

            Token docToken = Current();

            if (docToken.Type == TokenType.Document)
            {
                doc = docToken.Raw;
            }

            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.FixedKeyword when isFixed:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.FixedKeyword:
                        isFixed = true;
                        break;

                    case TokenType.UnicodeKeyword when isUnicode:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.UnicodeKeyword:
                        isUnicode = true;
                        break;

                    case TokenType.ASCIIKeyword when isASCII:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.ASCIIKeyword:
                        isASCII = true;
                        break;

                    case TokenType.UTF8Keyword when isUTF8:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.UTF8Keyword:
                        isUTF8 = true;
                        break;

                    case TokenType.CheckrefKeyword when isCheckref:
                        LogError(string.Format(Resources.Error_A1021_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.CheckrefKeyword:
                        isCheckref = true;
                        break;

                    case TokenType.SByteKeyword:
                    case TokenType.ByteKeyword:
                    case TokenType.ShortKeyword:
                    case TokenType.UShortKeyword:
                    case TokenType.IntKeyword:
                    case TokenType.UIntKeyword:
                    case TokenType.LongKeyword:
                    case TokenType.ULongKeyword:
                        if (isUnicode)
                        {
                            LogError(Resources.Error_A1026_InvalidEncodingUnicodeKeyword, token);
                        }
                        else if (isASCII)
                        {
                            LogError(Resources.Error_A1027_InvalidEncodingASCIIKeyword, token);
                        }
                        else if (isUTF8)
                        {
                            LogError(Resources.Error_A1028_InvalidEncodingUTF8Keyword, token);
                        }

                        if (isCheckref)
                        {
                            LogError(Resources.Error_A1025_InvalidCheckrefKeyword, token);
                        }
                        type = token.Raw;
                        goto NameDef;

                    case TokenType.BoolKeyword:
                    case TokenType.FloatKeyoword:
                    case TokenType.DoubleKeyword:
                    case TokenType.DecimalKeyword:
                        if (isUnicode)
                        {
                            LogError(Resources.Error_A1026_InvalidEncodingUnicodeKeyword, token);
                        }
                        else if (isASCII)
                        {
                            LogError(Resources.Error_A1027_InvalidEncodingASCIIKeyword, token);
                        }
                        else if (isUTF8)
                        {
                            LogError(Resources.Error_A1028_InvalidEncodingUTF8Keyword, token);
                        }

                        if (isCheckref)
                        {
                            LogError(Resources.Error_A1025_InvalidCheckrefKeyword, token);
                        }

                        if (isFixed)
                        {
                            LogError(Resources.Error_A1015_InvalidFixedKeyword, token);
                        }
                        type = token.Raw;
                        goto NameDef;

                    case TokenType.CharKeyword:
                    case TokenType.StringKeyword:
                        if (isFixed)
                        {
                            LogError(Resources.Error_A1015_InvalidFixedKeyword, token);
                        }
                        type = token.Raw;
                        goto NameDef;

                    case TokenType.Identifier:
                        type = token.Raw;
                        Type t = Type.GetType(type, false);

                        if (t != null)
                        {
                            if (isUnicode)
                            {
                                LogError(Resources.Error_A1026_InvalidEncodingUnicodeKeyword, token);
                            }
                            else if (isASCII)
                            {
                                LogError(Resources.Error_A1027_InvalidEncodingASCIIKeyword, token);
                            }
                            else if (isUTF8)
                            {
                                LogError(Resources.Error_A1028_InvalidEncodingUTF8Keyword, token);
                            }

                            if (t.IsValueType && isCheckref)
                            {
                                LogError(Resources.Error_A1025_InvalidCheckrefKeyword, token);
                            }

                            if (isFixed)
                            {
                                LogError(Resources.Error_A1015_InvalidFixedKeyword, token);
                            }
                        }

                        goto NameDef;
                }
            }

            LogError(Resources.Error_A1006_ExpectFieldType, Current());
            return null;
        NameDef:

            while (ExpectNextTokenType(1, TokenType.OpenBracket) && ExpectNextTokenType(2, TokenType.CloseBracket))
            {
                MoveNext();
                MoveNext();
                type += Resources.ArraySuffix;
            }

            if (ExpectNextTokenType(1, TokenType.Identifier))
            {
                MoveNext();
                name = Current().Raw;
                CheckIdentifier(name);

                if (ExpectNextTokenType(1, TokenType.Equals))
                {
                    MoveNext();

                    if (isRefType)
                    {
                        if (ExpectNextTokenType(1, TokenType.DefaultValue))
                        {
                            MoveNext();
                            assign = Current().Raw.Trim();
                        }
                    }
                    else
                    {
                        LogError(Resources.Error_A1001_InvalidAssignment, Current());
                    }
                }

                if (ExpectNextTokenType(1, TokenType.Semicolon))
                {
                    MoveNext();
                    return new FieldDeclaration
                    {
                        IsFixed = isFixed,
                        IsUnicode = isUnicode,
                        IsASCII = isASCII,
                        IsUTF8 = isUTF8,
                        IsCheckref = isCheckref,
                        Name = name,
                        Doc = doc,
                        Type = type,
                        Assignment = assign
                    };
                }
                else
                {
                    LogError(Resources.Error_A1010_ExpectSemicolon, Current());
                }
            }
            else
            {
                LogError(Resources.Error_A1007_ExpectIdentifier, Current());
            }

            return null;
        }

        private bool ExpectNextTokenType(int nextCount, TokenType type)
        {
            int i = m_Index + nextCount;
            if (i >= m_Tokens.Length || i < 0) return false;
            return m_Tokens[m_Index + nextCount].Type == type;
        }

        private bool MoveNext()
        {
            bool next = m_Index < m_Tokens.Length - 1 && !IsError;
            if (next) m_Index++;
            return next;
        }

        private bool MoveBack()
        {
            bool back = m_Index > -1 && !IsError;
            if (back) m_Index--;
            return back;
        }

        private void LogError(string message, Token token)
        {
            if (!IsError)
            {
                m_ErrorWriter.WriteLine(message);
                m_ErrorWriter.WriteLine(Resources.FilePath + token.FilePath);
                m_ErrorWriter.WriteLine(Resources.LineNumber + token.LineNumber.ToString());
                IsError = true;
            }
        }

        private void CheckIdentifier(string identifier)
        {
            for (int i = 0; i < identifier.Length; i++)
            {
                char c = identifier[i];
                bool valid = char.IsLetterOrDigit(c) || c == '@' || c == '_' || c == '.';

                if (i == 0)
                {
                    if (!char.IsDigit(c) && valid)
                    {
                        continue;
                    }
                }
                else if (valid)
                {
                    continue;
                }

                goto Log;
            }

            if (identifier[identifier.Length - 1] != '.' && !s_Reserved.Contains(identifier))
            {
                return;
            }

        Log:
            LogError(string.Format(Resources.Error_A1017_InvalidIdentifier, identifier), Current());
        }

        private Token Current()
        {
            if (m_Index < 0 || m_Index >= m_Tokens.Length)
            {
                return default;
            }

            return m_Tokens[m_Index];
        }
    }
}
