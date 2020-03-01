namespace Accelbuffer.Compiling
{
    public partial class TypeName
    {
        internal static TypeName GetSimpleTypeName(string name)
        {
            return new TypeName
            {
                RawString = name,
                IsGenericType = false,
                GenericTypeDefinitionName = null,
                GenericParameters = null,
                IsNullable =false,
                ArraySuffixCount = 0
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RawString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static implicit operator string(TypeName name)
        {
            return name?.RawString;
        }
    }
}
