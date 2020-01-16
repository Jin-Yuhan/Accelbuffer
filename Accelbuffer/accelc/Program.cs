using accelc.Compiler;
using accelc.Properties;
using System;
using System.IO;

namespace accelc
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-v")
            {
                Console.WriteLine(Resources.Version);
                return;
            }

            if (args.Length < 2)
            {
                ShowHelps();
                return;
            }

            switch (args[0])
            {
                case "-n":
                    {
                        string path = args[1];
                        string text = Resources.AccelbufferTemplate;

                        path = Path.ChangeExtension(path, Resources.AccelbufferFileExtension);

                        text = text.Replace(Resources.ReplaceString, Path.GetFileNameWithoutExtension(path));
                        File.WriteAllText(path, text);

                        Console.WriteLine(Resources.GenerateSuccessfully);
                        Console.WriteLine(Resources.FilePath + path);
                        break;
                    }

                case "-t":
                    {
                        string filePath = args[1];

                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine(string.Format(Resources.Error_A1002_FileNotFound, filePath));
                            return;
                        }

                        Token[] tokens = CompileToTokens(filePath);

                        if (tokens != null)
                        {
                            foreach (Token token in tokens)
                            {
                                Console.WriteLine($"raw: '{token.Raw}', type: {token.Type}, line: {token.LineNumber}");
                            }
                        }

                        break;
                    }

                case "-d":
                    {
                        string filePath = args[1];

                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine(string.Format(Resources.Error_A1002_FileNotFound, filePath));
                            return;
                        }

                        Declaration[] declarations = CompileToDeclarations(filePath);

                        if (declarations != null)
                        {
                            ShowDeclarations(declarations);
                        }

                        break;
                    }

                case "-c":
                    {
                        for (int i = 1; i < args.Length; i++)
                        {
                            string filePath = args[i];

                            if (!File.Exists(filePath))
                            {
                                Console.WriteLine(string.Format(Resources.Error_A1002_FileNotFound, filePath));
                                continue;
                            }

                            Declaration[] declarations = CompileToDeclarations(filePath);

                            if (declarations == null)
                            {
                                continue;
                            }

                            string outputPath = filePath + ".cs";
                            Generator generator = new Generator(declarations, outputPath);
                            generator.Generate();

                            Console.WriteLine(Resources.GenerateSuccessfully);
                            Console.WriteLine(Resources.FilePath + outputPath);
                        }

                        break;
                    }

                default:
                    ShowHelps();
                    break;
            }
        }

        private static void ShowHelps()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\taccelc [options] [file paths...]");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-t: Show tokens of an accelbuffer file.");
            Console.WriteLine("\t-d: Show declarations of an accelbuffer file.");
            Console.WriteLine("\t-n: Create a new accelbuffer template file.");
            Console.WriteLine("\t-c: Compile accelbuffer files.");
            Console.WriteLine("\t-v: Show compiler version.");
        }

        private static Token[] CompileToTokens(string filePath)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Scanner scanner = new Scanner(Console.Out, filePath);
            Token[] tokens = scanner.ToTokens();
            Console.ForegroundColor = color;

            if (scanner.IsError)
            {
                return null;
            }

            return tokens;
        }

        private static Declaration[] CompileToDeclarations(string filePath)
        {
            Token[] tokens = CompileToTokens(filePath);

            if (tokens == null)
            {
                return null;
            }

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Parser parser = new Parser(Console.Out, tokens);
            Declaration[] declarations = parser.GetDeclarations();
            Console.ForegroundColor = color;

            if (parser.IsError)
            {
                return null;
            }

            return declarations;
        }

        private static void ShowDeclarations(Declaration[] declarations)
        {
            foreach (Declaration declaration in declarations)
            {
                ShowDeclaration(declaration);
            }
        }

        private static void ShowDeclaration(Declaration declaration)
        {
            Console.WriteLine(declaration.GetType().Name);

            if (declaration is TypeDeclaration typeDeclaration)
            {
                ShowDeclarations(typeDeclaration.Declarations);
            }
        }
    }
}
