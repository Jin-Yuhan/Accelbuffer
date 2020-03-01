using System.Diagnostics;

namespace Accelbuffer
{
    internal static class Debug
    {
        [Conditional("DEBUG")]
        public static void Log(this IDebuggable obj, string message)
        {
#if UNITY
            UnityEngine.Debug.Log($"[{obj.FriendlyName}] {message}");
#else
            System.Diagnostics.Debug.Print($"[{obj.FriendlyName}] {message}");
#endif
        }

        [Conditional("DEBUG")]
        public static void Log(this IDebuggable obj, string message, params object[] args)
        {
#if UNITY
            UnityEngine.Debug.LogFormat($"[{obj.FriendlyName}] {message}", args);
#else
            System.Diagnostics.Debug.Print($"[{obj.FriendlyName}] {message}", args);
#endif
        }

        [Conditional("DEBUG")]
        public static void Assert(this IDebuggable obj, bool condition, string message)
        {
#if UNITY
            UnityEngine.Debug.Assert(condition, $"[{obj.FriendlyName}] {message}");
#else
            System.Diagnostics.Debug.Assert(condition, $"[{obj.FriendlyName}] {message}");
#endif
        }

        [Conditional("DEBUG")]
        public static void Assert(this IDebuggable obj, bool condition, string message, params object[] args)
        {
#if UNITY
            UnityEngine.Debug.AssertFormat(condition, $"[{obj.FriendlyName}] {message}", args);
#else
            System.Diagnostics.Debug.Assert(condition, string.Format($"[{obj.FriendlyName}] {message}", args));
#endif
        }
    }
}
