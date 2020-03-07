using Accelbuffer.Compiling;
using System;
using System.Collections.Generic;
using System.IO;

namespace asc
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowHelps();
                return;
            }

            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "-h":
                        ShowHelps();
                        return;
                    case "-v":
                        Console.WriteLine(Compiler.Version);
                        return;
                    default:
                        ShowHelps();
                        return;
                }
            }

            List<CompilerTask> tasks = new List<CompilerTask>();
            List<string> paths = new List<string>();
            RunMode mode = RunMode.None;
            bool lastIsOption = true;

            for (int i = 0; i < args.Length; i++)
            {
                string value = args[i];

                if (IsCompileOptions(value, out RunMode runMode))
                {
                    if (!lastIsOption)
                    {
                        CompilerTask task = new CompilerTask(mode, paths.ToArray());
                        tasks.Add(task);
                        paths.Clear();
                        mode = RunMode.None;
                    }

                    mode |= runMode;
                    lastIsOption = true;

                }
                else
                {
                    if (File.Exists(value))
                    {
                        paths.Add(value);
                    }
                    else
                    {
                        ShowHelps();
                        return;
                    }

                    lastIsOption = false;
                }
            }

            if (paths.Count == 0)
            {
                ShowHelps();
                return;
            }

            CompilerTask task1 = new CompilerTask(mode, paths.ToArray());
            tasks.Add(task1);

            Compiler compiler = new Compiler(new ConsoleErrorWriter(), KeywordManager.Default);
            Run(compiler, tasks);
        }

        private static void Run(Compiler compiler, IEnumerable<CompilerTask> tasks)
        {
            foreach (CompilerTask task in tasks)
            {
                task.Execute(compiler);
            }
        }

        private static bool IsCompileOptions(string value, out RunMode mode)
        {
            switch (value)
            {
                case "-b":
                    mode = RunMode.ToBytes;
                    break;
                case "-vb":
                    mode = RunMode.ToVisualBasic;
                    break;
                case "-cs":
                    mode = RunMode.ToCSharp;
                    break;
                default:
                    mode = RunMode.None;
                    return false;
            }

            return true;
        }

        private static void ShowHelps()
        {
            Console.WriteLine("Usage:\tasc <Help Options> | <(<Options...> [File Paths...])...>");
            Console.WriteLine("Help Options:");
            Console.WriteLine("\t-h : Show helps.");
            Console.WriteLine("\t-v : Show compiler version.");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-b : Compile to Accelbuffer Byte Code files.");
            Console.WriteLine("\t-cs: Compile to C# files.");
            Console.WriteLine("\t-vb: Compile to Visual Basic files.");
        }
    }
}
