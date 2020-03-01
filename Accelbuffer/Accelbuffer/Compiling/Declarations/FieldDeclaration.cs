namespace Accelbuffer.Compiling.Declarations
{
    public partial class FieldDeclaration : IDeclaration
    {
        /// <summary>
        /// 初始化 FieldDeclaration
        /// </summary>
        public FieldDeclaration()
        {
            IsObsolete = false;
            IsNeverNull = false;
        }
    }
}
