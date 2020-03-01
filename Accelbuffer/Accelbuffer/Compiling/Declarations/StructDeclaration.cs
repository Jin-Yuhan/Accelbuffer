using Accelbuffer.Reflection;

namespace Accelbuffer.Compiling.Declarations
{
    public partial class StructDeclaration : IDeclaration
    {
        private static readonly IDeclaration[] s_Empty = new IDeclaration[0];

        /// <summary>
        /// 初始化StructDeclaration
        /// </summary>
        public StructDeclaration()
        {
            Visibility = TypeVisibility.None;
            IsFinal = false;
            IsRef = false;
            IsNested = false;
            IsFieldIndexContinuous = true;
            Doc = null;
            Declarations = new DeclarationArray { Declarations = s_Empty };
        }
    }
}
