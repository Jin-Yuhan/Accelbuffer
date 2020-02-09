using asc.Compiler.Declarations;
using asc.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace asc.Compiler
{
    public class Parser
    {
        private readonly KeywordManager m_KeywordManager;
        private readonly LanguageManager m_LanguageManager;
        private readonly Token[] m_Tokens;
        private readonly ErrorWriter m_Writer;
        private int m_Index;

        public Parser(Token[] tokens, ErrorWriter writer, KeywordManager keywordManager, LanguageManager languageManager)
        {
            m_KeywordManager = keywordManager;
            m_LanguageManager = languageManager;
            m_Tokens = tokens;
            m_Writer = writer;
        }

        public IDeclaration[] ToDeclaration()
        {
            List<IDeclaration> declarations = new List<IDeclaration>(25);
            declarations.AddRange(m_LanguageManager.Predefines);
            m_Index = -1;
            SkipUseless();

            PackageDeclaration? package = GetPackageDeclaration();

            #region Using def

            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.Document:
                    case TokenType.Semicolon:
                        continue;
                    case TokenType.UsingKeyword:
                        declarations.Add(GetUsingDeclaration());
                        break;
                    case TokenType.PackageKeyword:
                        m_Writer.LogError(Resources.Error_AS006_InvalidPackage, token);
                        break;
                    default:
                        MoveBack();
                        goto StructDef;
                }
            }

        #endregion

        StructDef:

            #region Struct def

            if (package != null)
            {
                declarations.Add(package);
            }

            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.Document:
                    case TokenType.Semicolon:
                        continue;
                    case TokenType.PackageKeyword:
                        m_Writer.LogError(Resources.Error_AS006_InvalidPackage, token);
                        break;
                    default:
                        if (m_KeywordManager.IsInCategory(token.Raw, KeywordCategory.StructModifier) || token.Type == TokenType.StructKeyword)
                        {
                            MoveBack();
                            declarations.Add(GetStructDeclaration());
                        }
                        else
                        {
                            m_Writer.LogError(Resources.Error_AS007_InvalidToken, token);
                        }
                        break;
                }
            }

            #endregion

            return declarations.ToArray();
        }

        private void SkipUseless()
        {
            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.Semicolon:
                    case TokenType.Document:
                        continue;
                    default:
                        MoveBack();
                        return;
                }
            }
        }

        private PackageDeclaration? GetPackageDeclaration()
        {
            if (!ExpectNextTokenType(TokenType.PackageKeyword))
            {
                return null;
            }

            MoveNext();

            string? identifier = GetNextPackageIdentifier();

            if (identifier == null)
            {
                m_Writer.LogError(Resources.Error_AS003_MissingIdentifier, Current());
                return null;
            }

            if (!ExpectNextTokenType(TokenType.Semicolon))
            {
                m_Writer.LogError(Resources.Error_AS004_MissingSemicolon, Current());
                return null;
            }

            MoveNext();
            return new PackageDeclaration { PackageName = identifier };
        }

        private UsingDeclaration GetUsingDeclaration()
        {
            //string? identifier = GetNextPackageOrTypeIdentifier(out bool isGeneric);
            string? identifier = GetNextPackageIdentifier();

            if (identifier == null)
            {
                m_Writer.LogError(Resources.Error_AS003_MissingIdentifier, Current());
                return null!;
            }

            //if (ExpectNextTokenType(TokenType.AsKeyword))
            //{
            //    MoveNext();
            //    string? alias = GetNextPackageOrTypeIdentifier(out bool isGenericAlias);

            //    if (alias == null)
            //    {
            //        m_Writer.Log(Resources.Error_AS003_MissingIdentifier, Current());
            //        return null!;
            //    }

            //    if (isGenericAlias)
            //    {
            //        m_Writer.Log(Resources.Error_AS005_InvalidIdentifier, Current(), alias);
            //        return null!;
            //    }

            //    if (ExpectNextTokenType(TokenType.Semicolon))
            //    {
            //        MoveNext();
            //        return new UsingAsDeclaration { TypeName = identifier, AliasName = alias };
            //    }
            //}
            //else 

            if (ExpectNextTokenType(TokenType.Semicolon))
            {
                //if (isGeneric)
                //{
                //    m_Writer.Log(Resources.Error_AS005_InvalidIdentifier, Current(), identifier);
                //    return null!;
                //}

                if (m_LanguageManager.IsBuiltinPackageName(identifier))
                {
                    m_Writer.LogWarning(Resources.Warning_AS002_UnnecessaryUsing, Current(), identifier);
                }

                MoveNext();
                return new UsingDeclaration { PackageName = identifier };
            }

            m_Writer.LogError(Resources.Error_AS004_MissingSemicolon, Current());
            return null!;
        }

        private StructDeclaration GetStructDeclaration(bool isNested = false)
        {
            string name;
            string? doc = null;
            bool isPublic = false;
            bool isInternal = false;
            bool isPrivate = false;
            bool isProtected = false;
            bool isRef = false;
            bool isFinal = false;
            int size = 160;
            List<IDeclaration> declarations = new List<IDeclaration>();

            int fieldIndex = 0;

            #region Doc

            Token docToken = Current();

            if (docToken.Type == TokenType.Document)
            {
                doc = docToken.Raw;
            }

            #endregion

            #region Modifier

            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.PublicKeyword when isPublic:
                        m_Writer.LogError(string.Format(Resources.Error_AS008_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.PublicKeyword when isInternal || isPrivate || isProtected:
                        m_Writer.LogError(Resources.Error_AS009_MultipleAccessKeyword, token);
                        break;
                    case TokenType.PublicKeyword:
                        isPublic = true;
                        break;

                    case TokenType.InternalKeyword when isInternal:
                        m_Writer.LogError(string.Format(Resources.Error_AS008_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.InternalKeyword when isPublic || isPrivate:
                        m_Writer.LogError(Resources.Error_AS009_MultipleAccessKeyword, token);
                        break;
                    case TokenType.InternalKeyword:
                        isInternal = true;
                        break;

                    case TokenType.PrivateKeyword when isPrivate:
                        m_Writer.LogError(string.Format(Resources.Error_AS008_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.PrivateKeyword when isPublic || isInternal:
                        m_Writer.LogError(Resources.Error_AS009_MultipleAccessKeyword, token);
                        break;
                    case TokenType.PrivateKeyword when isNested:
                        isPrivate = true;
                        break;
                    case TokenType.PrivateKeyword:
                        m_Writer.LogError(Resources.Error_AS018_InvalidPrivateKeyword, token);
                        break;


                    case TokenType.ProtectedKeyword when isProtected:
                        m_Writer.LogError(string.Format(Resources.Error_AS008_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.ProtectedKeyword when isPublic:
                        m_Writer.LogError(Resources.Error_AS009_MultipleAccessKeyword, token);
                        break;
                    case TokenType.ProtectedKeyword when isNested:
                        isProtected = true;
                        break;
                    case TokenType.ProtectedKeyword:
                        m_Writer.LogError(Resources.Error_AS019_InvalidProtectedKeyword, token);
                        break;

                    case TokenType.RefKeyword when isRef:
                        m_Writer.LogError(string.Format(Resources.Error_AS008_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.RefKeyword:
                        isRef = true;
                        break;

                    case TokenType.FinalKeyword when isFinal:
                        m_Writer.LogError(string.Format(Resources.Error_AS008_MultipleKeyword, token.Raw), token);
                        break;
                    case TokenType.FinalKeyword:
                        isFinal = true;
                        break;

                    case TokenType.StructKeyword:
                        goto NameDef;
                }
            }

        #endregion

        NameDef:

            #region identifier & about-expr

            if (isFinal && !isRef)
            {
                m_Writer.LogError(Resources.Error_AS010_InvalidFinalKeyword, Current());
            }

            if (!ExpectNextTokenType(TokenType.Identifier))
            {
                m_Writer.LogError(Resources.Error_AS003_MissingIdentifier, Current());
            }

            MoveNext();
            name = Current().Raw;

            if (ExpectNextTokenType(TokenType.AboutKeyword))
            {
                MoveNext();

                if (!ExpectNextTokenType(TokenType.IntLiteral))
                {
                    m_Writer.LogError(Resources.Error_AS011_MissingIntLiteral, Current());
                    return null!;
                }

                MoveNext();
                size = int.Parse(Current().Raw);

                if (size == 0)
                {
                    m_Writer.LogWarning(Resources.Warning_AS001_InvalidMemorySize, Current());
                }
            }

            #endregion

            if (!ExpectNextTokenType(TokenType.OpenBrace))
            {
                m_Writer.LogError(Resources.Error_AS012_MissingOpenBrace, Current());
                return null!;
            }

            MoveNext();

            while (MoveNext())
            {
                Token token = Current();

                switch (token.Type)
                {
                    case TokenType.Document:
                    case TokenType.Semicolon:
                        continue;
                    case TokenType.VarKeyword:
                        MoveBack();
                        fieldIndex++;
                        declarations.Add(GetFieldDeclaration(ref fieldIndex));
                        break;
                    default:
                        if (m_KeywordManager.IsInCategory(token.Raw, KeywordCategory.StructModifier) || token.Type == TokenType.StructKeyword)
                        {
                            MoveBack();
                            declarations.Add(GetStructDeclaration(true));
                            break;
                        }
                        else
                        {
                            MoveBack();
                            goto End;
                        }
                }
            }

        End:

            if (!ExpectNextTokenType(TokenType.CloseBrace))
            {
                m_Writer.LogError(Resources.Error_AS017_MissingCloseBrace, Current());
                return null!;
            }

            MoveNext();

            bool isContinuous = CheckFieldIndex(declarations);

            if (!isContinuous)
            {
                m_Writer.LogWarning(Resources.Warning_AS003_RequireContinuousIndex, Current());
            }

            return new StructDeclaration
            {
                Name = name,
                TypeVisibility = GetVisibility(isPublic, isInternal, isPrivate, isProtected),
                IsFinal = isFinal,
                IsRef = isRef,
                Doc = doc,
                Size = size,
                IsNested = isNested,
                IsFieldIndexContinuous = isContinuous,
                Declarations = declarations.ToArray()
            };
        }

        private FieldDeclaration GetFieldDeclaration(ref int index)
        {
            string type;
            string name;
            string? doc = null;
            bool isObsolete = false;
            bool isNeverNull = false;

            #region Doc

            Token docToken = Current();

            if (docToken.Type == TokenType.Document)
            {
                doc = docToken.Raw;
            }

            #endregion

            MoveNext();//var

            if (!ExpectNextTokenType(TokenType.Identifier))
            {
                m_Writer.LogError(Resources.Error_AS003_MissingIdentifier, Current());
                return null!;
            }

            MoveNext();//identifier
            name = Current().Raw;

            if (ExpectNextTokenType(TokenType.Bar))
            {
                MoveNext();

                if (!ExpectNextTokenType(TokenType.IntLiteral))
                {
                    m_Writer.LogError(Resources.Error_AS011_MissingIntLiteral, Current());
                    return null!;
                }

                MoveNext();
                index = int.Parse(Current().Raw);

                if ((index <= 0) || (index > 268435456))
                {
                    m_Writer.LogError(Resources.Error_AS013_InvalidFieldIndex, Current());
                    return null!;
                }
            }

            if (!ExpectNextTokenType(TokenType.Colon))
            {
                m_Writer.LogError(Resources.Error_AS014_MissingColonOrBar, Current());
                return null!;
            }

            MoveNext();//colon

            if (ExpectNextTokenType(TokenType.Asterisk))
            {
                MoveNext();
                isNeverNull = true;
            }

            string? typeName = GetNextPackageOrTypeIdentifier(out _);

            if (typeName == null)
            {
                m_Writer.LogError(Resources.Error_AS016_MissingIdentifierOrAsterisk, Current());
                return null!;
            }

            type = typeName;

            if (ExpectNextTokenType(TokenType.ObsoleteKeyword))
            {
                MoveNext();
                isObsolete = true;
            }

            if (!ExpectNextTokenType(TokenType.Semicolon))
            {
                m_Writer.LogError(Resources.Error_AS015_MissingSemicolonOrObsoleteKeyword, Current());
                return null!;
            }

            MoveNext();
            return new FieldDeclaration
            {
                Name = name,
                Type = type,
                Index = index,
                Doc = doc,
                IsNeverNull = isNeverNull,
                IsObsolete = isObsolete
            };
        }

        private Visibility GetVisibility(bool isPublic, bool isInternal, bool isPrivate, bool isProtected)
        {
            Visibility visibility = Visibility.None;

            if (isPublic)
            {
                visibility |= Visibility.Public;
            }

            if (isInternal)
            {
                visibility |= Visibility.Internal;
            }

            if (isPrivate)
            {
                visibility |= Visibility.Private;
            }

            if (isProtected)
            {
                visibility |= Visibility.Protected;
            }

            return visibility == Visibility.None ? Visibility.Internal : visibility;
        }

        private string? GetNextPackageIdentifier()
        {
            StringBuilder sb = new StringBuilder();
            TokenType last = TokenType.Dot;

            while (MoveNext())
            {
                Token token = Current();
                TokenType type = token.Type;

                if ((type == TokenType.Identifier || type == TokenType.Dot) && type != last)
                {
                    sb.Append(token.Raw);
                    last = type;
                }
                else
                {
                    MoveBack();
                    break;
                }
            }

            return sb.Length == 0 ? null : sb.ToString();
        }

        private string? GetNextPackageOrTypeIdentifier(out bool isGeneric)
        {
            StringBuilder sb = new StringBuilder();
            TokenType last = TokenType.Dot;
            bool isKeyword = false;
            isGeneric = false;

            while (MoveNext())
            {
                Token token = Current();
                TokenType type = token.Type;

                switch (type)
                {
                    case TokenType.Identifier when last == TokenType.Dot:
                    case TokenType.Dot when last == TokenType.Identifier && !isKeyword:
                    case TokenType.Question when last == TokenType.Identifier || last == TokenType.GreaterThan || last == TokenType.CloseBracket:
                        last = type;
                        isKeyword = false;
                        sb.Append(token.Raw);
                        break;

                    case TokenType.OpenBracket when last == TokenType.Identifier || last == TokenType.GreaterThan || last == TokenType.Question || last == TokenType.CloseBracket:
                        last = TokenType.CloseBracket;
                        isKeyword = false;
                        sb.Append(token.Raw);

                        if (!ExpectNextTokenType(TokenType.CloseBracket))
                        {
                            return null;
                        }

                        MoveNext();
                        sb.Append(Current().Raw);
                        break;

                    case TokenType.LessThan when last == TokenType.Identifier && !isGeneric && !isKeyword:
                        isGeneric = true;
                        last = TokenType.GreaterThan;
                        isKeyword = false;
                        sb.Append(token.Raw);

                        while (true)
                        {
                            string? innerType = GetNextPackageOrTypeIdentifier(out _);

                            if (innerType == null)
                            {
                                return null;
                            }

                            sb.Append(innerType);

                            if (MoveNext())
                            {
                                Token next = Current();

                                if (next.Type == TokenType.Comma)
                                {
                                    sb.Append(next.Raw);
                                    continue;
                                }
                                else if (next.Type == TokenType.GreaterThan)
                                {
                                    sb.Append(next.Raw);
                                    break;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        break;
                    default:
                        if (m_KeywordManager.IsInCategory(token.Raw, KeywordCategory.TypeKeyword) && !isKeyword)
                        {
                            last = TokenType.Identifier;
                            isKeyword = true;
                            sb.Append(token.Raw);
                            break;
                        }
                        else
                        {
                            MoveBack();
                            goto End;
                        }
                }
            }

        End:

            return sb.Length == 0 ? null : sb.ToString();
        }

        private bool CheckFieldIndex(List<IDeclaration> declarations)
        {
            List<FieldDeclaration> fields = (from def in declarations
                                                    let field = def as FieldDeclaration
                                                    where field != null
                                                    orderby field.Index ascending
                                                    select field).ToList();

            if (fields.Count > 1)
            {
                bool result = true;
                int last = fields[0].Index;

                HashSet<int> indexes = new HashSet<int>(fields.Count) { last };

                for (int i = 1; i < fields.Count; i++)
                {
                    int index = fields[i].Index;

                    if (!indexes.Add(index))
                    {
                        m_Writer.LogError(string.Format(Resources.Error_AS020_MultipleIndex, index), Current());
                        return result;
                    }

                    if (index - last != 1)
                    {
                        result = false;
                    }

                    last = index;
                }

                return result;
            }

            return true;
        }

        private bool ExpectNextTokenType(TokenType type)
        {
            int i = m_Index + 1;
            return (i >= m_Tokens.Length || i < 0) ? false : m_Tokens[i].Type == type;
        }

        private bool MoveNext()
        {
            bool next = m_Index < m_Tokens.Length - 1 && !m_Writer.IsError;
            m_Index += next ? 1 : 0;
            return next;
        }

        private bool MoveBack()
        {
            bool back = m_Index > -1 && !m_Writer.IsError;
            m_Index -= back ? 1 : 0;
            return back;
        }

        private Token Current()
        {
            return m_Index < 0 || m_Index >= m_Tokens.Length ? default : m_Tokens[m_Index];
        }
    }
}
