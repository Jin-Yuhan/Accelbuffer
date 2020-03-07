using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using Accelbuffer.Properties;

namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 提供线程安全的编译AccelbufferScript的接口
    /// </summary>
    public sealed class Compiler
    {
        /// <summary>
        /// 获取编译器版本号
        /// </summary>
        public static string Version => Resources.CompilerVersion;

        private readonly ErrorWriter m_ErrorWriter;
        private readonly KeywordManager m_KeywordManager;
        private readonly object m_SyncObj;

        private Scanner m_Scanner;
        private Parser m_Parser;
        private RuntimeCompiler m_Compiler;

        /// <summary>
        /// 获取是否出现编译错误
        /// </summary>
        public bool HasError => m_ErrorWriter.IsError;

        /// <summary>
        /// 初始化Compiler
        /// </summary>
        /// <param name="errorWriter"></param>
        /// <param name="keywordManager"></param>
        public Compiler(ErrorWriter errorWriter, KeywordManager keywordManager)
        {
            m_ErrorWriter = errorWriter ?? throw new ArgumentNullException(nameof(errorWriter));
            m_KeywordManager = keywordManager ?? throw new ArgumentNullException(nameof(keywordManager));
            m_SyncObj = new object();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Token[] CompileToTokens(string filePath)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    Scanner scanner = GetScanner();
                    scanner.Reset(filePath, reader);
                    return scanner.ToTokens();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DeclarationArray CompileToDeclarations(string filePath)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                Token[] tokens;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    Scanner scanner = GetScanner();
                    scanner.Reset(filePath, reader);
                    tokens = scanner.ToTokens();
                }

                if (HasError)
                {
                    return default;
                }

                Parser parser = GetParser();
                parser.Reset(tokens);
                return parser.ToDeclaration();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="outputPath"></param>
        /// <param name="languageManager"></param>
        public void CompileToFile(string filePath, string outputPath, LanguageManager languageManager)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                Token[] tokens;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    Scanner scanner = GetScanner();
                    scanner.Reset(filePath, reader);
                    tokens = scanner.ToTokens();
                }

                if (HasError)
                {
                    return;
                }

                Parser parser = GetParser();
                parser.Reset(tokens);
                DeclarationArray declarations = parser.ToDeclaration();

                if (HasError)
                {
                    return;
                }

                using (StreamWriter writer = new StreamWriter(outputPath, false))
                {
                    languageManager.GenerateCode(declarations, writer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, AccelTypeInfo> CompileToTypes(string filePath)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                Token[] tokens;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    Scanner scanner = GetScanner();
                    scanner.Reset(filePath, reader);
                    tokens = scanner.ToTokens();
                }

                if (HasError)
                {
                    return default;
                }

                Parser parser = GetParser();
                parser.Reset(tokens);
                DeclarationArray declarations = parser.ToDeclaration();

                if (HasError)
                {
                    return default;
                }

                RuntimeCompiler runtimeCompiler = GetRuntimeCompiler();
                return runtimeCompiler.Compile(declarations);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public DeclarationArray CompileToDeclarations(Token[] tokens)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                Parser parser = GetParser();
                parser.Reset(tokens);
                return parser.ToDeclaration();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="tokens"></param>
        /// <param name="languageManager"></param>
        public void CompileToFile(string filePath, Token[] tokens, LanguageManager languageManager)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                Parser parser = GetParser();
                parser.Reset(tokens);
                DeclarationArray declarations = parser.ToDeclaration();

                if (HasError)
                {
                    return;
                }

                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    languageManager.GenerateCode(declarations, writer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, AccelTypeInfo> CompileToTypes(Token[] tokens)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                Parser parser = GetParser();
                parser.Reset(tokens);
                DeclarationArray declarations = parser.ToDeclaration();

                if (HasError)
                {
                    return default;
                }

                RuntimeCompiler runtimeCompiler = GetRuntimeCompiler();
                return runtimeCompiler.Compile(declarations);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="declarations"></param>
        /// <param name="languageManager"></param>
        public void CompileToFile(string filePath, DeclarationArray declarations, LanguageManager languageManager)
        {
            lock (m_SyncObj)
            {
                m_ErrorWriter.Reset();

                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    languageManager.GenerateCode(declarations, writer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="declarations"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<string, AccelTypeInfo> CompileToTypes(DeclarationArray declarations)
        {
            lock (m_SyncObj)
            {
                RuntimeCompiler runtimeCompiler = GetRuntimeCompiler();
                return runtimeCompiler.Compile(declarations);
            }
        }

        private Scanner GetScanner() => m_Scanner ?? (m_Scanner = new Scanner(m_ErrorWriter, m_KeywordManager));

        private Parser GetParser() => m_Parser ?? (m_Parser = new Parser(m_ErrorWriter, m_KeywordManager));

        private RuntimeCompiler GetRuntimeCompiler() => m_Compiler ?? (m_Compiler = new RuntimeCompiler());
    }
}
