using Accelbuffer.Compiling;
using System;

namespace asc
{
    public sealed class ConsoleErrorWriter : ErrorWriter
    {
        protected override void LogErrorMessage(string msg, string filePath, int line, int column, params object[] args)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{msg}\nfile: {filePath}\nline: {line}\ncolumn: {column}", args);
            Console.ForegroundColor = color;
        }

        protected override void LogWarning(string msg, string filePath, int line, int column, params object[] args)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{msg}\nfile: {filePath}\nline: {line}\ncolumn: {column}", args);
            Console.ForegroundColor = color;
        }
    }
}
