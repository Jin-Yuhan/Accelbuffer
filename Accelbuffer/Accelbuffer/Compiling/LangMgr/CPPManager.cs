using System.IO;

namespace Accelbuffer.Compiling
{
    internal sealed class CPPManager : LanguageManager
    {
        public override string Entension => "cpp";

        public override void GenerateCode(DeclarationArray declarations, StreamWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
