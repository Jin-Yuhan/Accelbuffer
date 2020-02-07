using System.Diagnostics;

namespace Accelbuffer
{
    internal static class Debug
    {
        [Conditional("DEBUG")]
        public static void Log(string message)
        {
#if UNITY
            UnityEngine.Debug.Log(message);
#else
            System.Diagnostics.Debug.Print(message);
#endif
        }

        [Conditional("DEBUG")]
        public static void Log(string message, params object[] args)
        {
#if UNITY
            UnityEngine.Debug.LogFormat(message, args);
#else
            System.Diagnostics.Debug.Print(message, args);
#endif
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
#if UNITY
            UnityEngine.Debug.Assert(condition, message);
#else
            System.Diagnostics.Debug.Assert(condition, message);
#endif
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message, params object[] args)
        {
#if UNITY
            UnityEngine.Debug.AssertFormat(condition, message, args);
#else
            System.Diagnostics.Debug.Assert(condition, string.Format(message, args));
#endif
        }
    }
}
