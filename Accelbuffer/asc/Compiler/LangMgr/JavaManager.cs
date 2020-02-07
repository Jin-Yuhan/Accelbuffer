using asc.Compiler.Declarations;

namespace asc.Compiler
{
    internal sealed class JavaManager : LanguageManager
    {
        public override IDeclaration[] Predefines => throw new System.NotImplementedException();

        public override string ChangeExtension(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public override void GenerateCode(IDeclaration[] declarations, string filePath)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsBuiltinPackageName(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
