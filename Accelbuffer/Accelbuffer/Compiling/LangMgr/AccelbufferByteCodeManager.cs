using System.IO;

namespace Accelbuffer.Compiling
{
    internal sealed class AccelbufferByteCodeManager : LanguageManager
    {
        public override string Entension => "bytes";

        public override void GenerateCode(DeclarationArray declarations, StreamWriter writer)
        {
            byte[] data = declarations.GetBytes();
            Stream stream = writer.BaseStream;
            stream.Write(data, 0, data.Length);
        }
    }
}
