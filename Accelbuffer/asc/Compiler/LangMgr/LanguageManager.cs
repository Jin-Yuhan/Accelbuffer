using asc.Compiler.Declarations;

namespace asc.Compiler
{
    public abstract class LanguageManager
    {
        public static LanguageManager VisualBasic { get; } = new VisualBasicManager();

        public static LanguageManager CSharp { get; } = new CSharpManager();

        public static LanguageManager CPP { get; } = new CPPManager();

        public static LanguageManager Java { get; } = new JavaManager();

        public abstract IDeclaration[] Predefines { get; }

        protected LanguageManager() { }

        public abstract string ChangeExtension(string filePath);

        public abstract void GenerateCode(IDeclaration[] declarations, string filePath);

        public abstract bool IsBuiltinPackageName(string name);
    }
}
