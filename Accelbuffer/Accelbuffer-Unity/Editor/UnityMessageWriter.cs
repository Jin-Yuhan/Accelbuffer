using Accelbuffer.Compiling;
using UnityEditor;
using UnityEngine;

namespace Accelbuffer.Unity
{
    public sealed class UnityMessageWriter : ErrorWriter
    {
        protected override void LogErrorMessage(string msg, string filePath, int line, int column, params object[] args)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            Debug.LogError($"{string.Format(msg, args)}\nfile:{filePath}\nline:{line}\ncolumn:{column}", obj);
        }

        protected override void LogWarning(string msg, string filePath, int line, int column, params object[] args)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            Debug.LogWarning($"{string.Format(msg, args)}\nfile:{filePath}\nline:{line}\ncolumn:{column}", obj);
        }
    }
}
