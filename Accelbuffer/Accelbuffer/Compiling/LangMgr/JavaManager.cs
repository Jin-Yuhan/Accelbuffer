using System.IO;

namespace Accelbuffer.Compiling
{
    internal sealed class JavaManager : LanguageManager
    {
        public override string Entension => "java";

        public override void GenerateCode(DeclarationArray declarations, StreamWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
