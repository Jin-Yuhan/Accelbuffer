using Accelbuffer.Compiling;
using System;
using System.Collections.Generic;

namespace asc
{
    public sealed class CompilerTask
    {
        public RunMode Mode { get; }

        public IEnumerable<string> FilePaths { get; }

        public CompilerTask(RunMode mode, IEnumerable<string> filePaths)
        {
            Mode = mode;
            FilePaths = filePaths;
        }

        public void Execute(Compiler compiler)
        {
            if ((Mode & RunMode.ToBytes) == RunMode.ToBytes)
            {
                Compile(compiler, LanguageManager.AccelbufferByteCode);
            }

            if ((Mode & RunMode.ToCSharp) == RunMode.ToCSharp)
            {
                Compile(compiler, LanguageManager.CSharp);
            }

            if ((Mode & RunMode.ToVisualBasic) == RunMode.ToVisualBasic)
            {
                Compile(compiler, LanguageManager.VisualBasic);
            }
        }

        private void Compile(Compiler compiler, LanguageManager languageManager)
        {
            foreach (string path in FilePaths)
            {
                string filePath = languageManager.ChangeExtension(path);
                compiler.CompileToFile(path, filePath, languageManager);
                Console.WriteLine("output " + filePath);
            }
        }
    }
}
