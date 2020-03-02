using Accelbuffer.Compiling;
using UnityEditor;
using UnityEngine;

namespace Accelbuffer.Unity
{
    public static class UnityCompiler
    {
        private static readonly Compiler s_Compiler = new Compiler(new UnityMessageWriter(), KeywordManager.Default);

        /// <summary>
        /// 编译AccelbufferScript文件
        /// </summary>
        /// <param name="refresh">指示编译完成后是否刷新资源编辑器</param>
        /// <param name="binary">指示是否编译为AccelbufferByteCode</param>
        /// <param name="rawPaths">AccelbufferScript文件的路径</param>
        public static void Compile(bool refresh, bool binary, params string[] rawPaths)
        {
            for (int i = 0; i < rawPaths.Length; i++)
            {
                string path = rawPaths[i];
                LanguageManager manager = binary ? LanguageManager.AccelbufferByteCode : LanguageManager.CSharp;
                string outputPath = manager.ChangeExtension(path);
                s_Compiler.CompileToFile(path, outputPath, manager);

                if (!s_Compiler.HasError)
                {
                    Debug.Log("output: " + outputPath);
                }
            }

            if (refresh)
            {
                AssetDatabase.Refresh();
            }            
        }
    }
}
