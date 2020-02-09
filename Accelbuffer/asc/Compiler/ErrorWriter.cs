using System;
using System.IO;

namespace asc.Compiler
{
    public sealed class ErrorWriter
    {
        public static ErrorWriter Default { get; }

        static ErrorWriter()
        {
            Default = new ErrorWriter(Console.Out);
            Default.OnBeforeLogError += () => Console.ForegroundColor = ConsoleColor.Red;
            Default.OnAfterLogError += () => Console.ResetColor();
            Default.OnBeforeLogWarning += () => Console.ForegroundColor = ConsoleColor.Yellow;
            Default.OnAfterLogWarning += () => Console.ResetColor();
        }

        public TextWriter Writer { get; }

        public bool IsError { get; private set; }

        public event Action OnBeforeLogError;

        public event Action OnAfterLogError;

        public event Action OnBeforeLogWarning;

        public event Action OnAfterLogWarning;

        public ErrorWriter(TextWriter writer)
        {
            Writer = writer;
            IsError = false;
        }

        public void Reset()
        {
            IsError = false;
        }

        public void LogError(string msg, Token token)
        {
            LogError(msg, token.FilePath, token.Line, token.Column, Array.Empty<object>());
        }

        public void LogError(string msg, string filePath, int line, int column, params object[] args)
        {
            if (!IsError)
            {
                IsError = true;
                OnBeforeLogError?.Invoke();
                Console.WriteLine($"{msg}\nfile: {filePath}\nline: {line}\ncolumn: {column}", args);
                OnAfterLogError?.Invoke();
            }
        }

        public void LogWarning(string msg, Token token)
        {
            LogWarning(msg, token.FilePath, token.Line, token.Column, Array.Empty<object>());
        }

        public void LogWarning(string msg, Token token, params object[] args)
        {
            LogWarning(msg, token.FilePath, token.Line, token.Column, args);
        }

        public void LogWarning(string msg, string filePath, int line, int column, params object[] args)
        {
            OnBeforeLogWarning?.Invoke();
            Console.WriteLine($"{msg}\nfile: {filePath}\nline: {line}\ncolumn: {column}", args);
            OnAfterLogWarning?.Invoke();
        }
    }
}
