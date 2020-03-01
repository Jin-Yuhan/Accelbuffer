using Accelbuffer.Compiling.Declarations;
using System;
using System.IO;

namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个目标语言管理器
    /// </summary>
    public abstract class LanguageManager
    {
        /// <summary>
        /// AccelbufferByteCode管理器
        /// </summary>
        public static LanguageManager AccelbufferByteCode { get; } = new AccelbufferByteCodeManager();

        /// <summary>
        /// VisualBasic管理器
        /// </summary>
        public static LanguageManager VisualBasic { get; } = new VisualBasicManager();

        /// <summary>
        /// C#管理器
        /// </summary>
        public static LanguageManager CSharp { get; } = new CSharpManager();

        /// <summary>
        /// C++管理器
        /// </summary>
        [Obsolete("not supported", true)]
        public static LanguageManager CPP { get; } = new CPPManager();

        /// <summary>
        /// Java管理器
        /// </summary>
        [Obsolete("not supported", true)]
        public static LanguageManager Java { get; } = new JavaManager();

        /// <summary>
        /// 获取该语言源文件的后缀名
        /// </summary>
        public abstract string Entension { get; }

        /// <summary>
        /// 初始化 LanguageManager
        /// </summary>
        protected LanguageManager() { }

        /// <summary>
        /// 切换文件路径的后缀名为目标语言源文件的后缀名
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>修改后的文件路径</returns>
        public string ChangeExtension(string filePath)
        {
            return Path.ChangeExtension(filePath, Entension);
        }

        /// <summary>
        /// 生成目标语言的代码
        /// </summary>
        /// <param name="declarations">定义列表</param>
        /// <param name="writer">数据写入器</param>
        public abstract void GenerateCode(DeclarationArray declarations, StreamWriter writer);
    }
}
