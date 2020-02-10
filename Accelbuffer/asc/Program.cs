using asc.Compiler;
using asc.Compiler.Declarations;
using asc.Properties;
using System;
using System.IO;

namespace asc
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-v")
            {
#if UNITY
                Console.WriteLine(Resources.Version_Unity);
#else
                Console.WriteLine(Resources.Version);
#endif
                return;
            }

            if (args.Length < 2)
            {
                ShowHelps();
                return;
            }

            RunMode mode = args[0] switch
            {
                "-t" => RunMode.ToTokens,
                "-d" => RunMode.ToDeclarations,
                "-c" => RunMode.ToFile,
                _ => RunMode.None
            };

            if (mode == RunMode.None)
            {
                ShowHelps();
                return;
            }

            LanguageManager? langManager;
            string[] filePaths;

            if (mode == RunMode.ToTokens)
            {
                langManager = LanguageManager.CSharp;
                filePaths = new string[args.Length - 1];

                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePaths[i] = args[i + 1];
                }
            }
            else
            {
                if (args.Length < 3)
                {
                    ShowHelps();
                    return;
                }

                langManager = args[1] switch
                {
                    "-vb" => LanguageManager.VisualBasic,
                    "-cs" => LanguageManager.CSharp,
                    "-cpp" => LanguageManager.CPP,
                    "-java" => LanguageManager.Java,
                    _ => null,
                };

                if (langManager == null)
                {
                    ShowHelps();
                    return;
                }

                filePaths = new string[args.Length - 2];

                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePaths[i] = args[i + 2];
                }
            }



            Run(mode, filePaths, ErrorWriter.Default, KeywordManager.Default, langManager);
        }

        private static void ShowHelps()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tasc [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-v                      : Show compiler version.");
            Console.WriteLine("\t-t       <file paths...>: Show tokens in files.");

            Console.WriteLine("\t-d -vb   <file paths...>: Show Visual Basic declarations in files.");
            Console.WriteLine("\t-d -cs   <file paths...>: Show C# declarations in files.");
            Console.WriteLine("\t-d -cpp  <file paths...>: Show C++ declarations in files.");
            Console.WriteLine("\t-d -java <file paths...>: Show Java declarations in files.");

            Console.WriteLine("\t-c -vb   <file paths...>: Compile to Visual Basic files.");
            Console.WriteLine("\t-c -cs   <file paths...>: Compile to C# files.");
            Console.WriteLine("\t-c -cpp  <file paths...>: Compile to C++ files.");
            Console.WriteLine("\t-c -java <file paths...>: Compile to Java files.");
        }

        private static void Run(RunMode mode, string[] filePaths, ErrorWriter writer, KeywordManager keywordManager, LanguageManager languageManager)
        {
            foreach (string path in filePaths)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                writer.Reset();

                using Scanner scanner = new Scanner(path, writer, keywordManager);
                Token[] tokens = scanner.ToTokens();

                if (writer.IsError)
                {
                    continue;
                }

                if (mode == RunMode.ToTokens)
                {
                    WriteTokens(tokens);
                    continue;
                }

                Parser parser = new Parser(tokens, writer, keywordManager, languageManager);
                IDeclaration[] declarations = parser.ToDeclaration();

                if (writer.IsError)
                {
                    continue;
                }

                if (mode == RunMode.ToDeclarations)
                {
                    WriteDeclarations(declarations);
                    continue;
                }

                string outputPath = languageManager.ChangeExtension(path);
                languageManager.GenerateCode(declarations, outputPath);
                Console.WriteLine("output: " + outputPath);
            }
        }

        private static void WriteTokens(Token[] tokens)
        {
            if (tokens.Length > 0)
            {
                Console.WriteLine(tokens[0].FilePath);
            }

            foreach (Token token in tokens)
            {
                Console.WriteLine($"\t{{'{token.Raw}', {token.Type}, {token.Line}, {token.Column}}}");
            }
        }

        private static void WriteDeclarations(IDeclaration[] declarations)
        {
            foreach (IDeclaration declaration in declarations)
            {
                switch (declaration)
                {
                    case UsingDeclaration usingDeclaration:
                        Console.WriteLine($"UsingDeclaration: {usingDeclaration.PackageName}");
                        break;
                    case UsingAsDeclaration usingAsDeclaration:
                        Console.WriteLine($"UsingAsDeclaration: {usingAsDeclaration.TypeName}->{usingAsDeclaration.AliasName}");
                        break;
                    case PackageDeclaration packageDeclaration:
                        Console.WriteLine($"PackageDeclaration: {packageDeclaration.PackageName}");
                        break;
                    case FieldDeclaration fieldDeclaration:
                        Console.WriteLine("FieldDeclaration:");
                        Console.WriteLine($"\ttype: {fieldDeclaration.Type}");
                        Console.WriteLine($"\treal_type: {fieldDeclaration.RealType}");
                        Console.WriteLine($"\tname: {fieldDeclaration.Name}");
                        Console.WriteLine($"\tindex: {fieldDeclaration.Index}");
                        Console.WriteLine($"\tis_obsolete: {fieldDeclaration.IsObsolete}");
                        Console.WriteLine($"\tis_neverNull: {fieldDeclaration.IsNeverNull}");
                        Console.WriteLine($"\tdocument: {fieldDeclaration.Doc}");
                        break;
                    case StructDeclaration structDeclaration:
                        Console.WriteLine("StructDeclaration:");
                        Console.WriteLine($"\tvisibility: {structDeclaration.TypeVisibility}");
                        Console.WriteLine($"\tname: {structDeclaration.Name}");
                        Console.WriteLine($"\tis_final: {structDeclaration.IsFinal}");
                        Console.WriteLine($"\tis_ref: {structDeclaration.IsRef}");
                        Console.WriteLine($"\tis_nested: {structDeclaration.IsNested}");
                        Console.WriteLine($"\tis_field_index_continuous: {structDeclaration.IsFieldIndexContinuous}");
                        Console.WriteLine($"\tsize: {structDeclaration.Size}");
                        Console.WriteLine($"\tdocument: {structDeclaration.Doc}");
                        Console.WriteLine("{");
                        WriteDeclarations(structDeclaration.Declarations);
                        Console.WriteLine("}");
                        break;
                }
            }
        }
    }
}
