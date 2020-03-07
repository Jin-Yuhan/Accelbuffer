using Accelbuffer.Compiling.Declarations;

namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个abc文件的格式
    /// </summary>
    [SerializeBy(typeof(DeclarationArraySerializer))]
    public struct DeclarationArray
    {
        /// <summary>
        /// 文件中包含的所有声明
        /// </summary>
        public IDeclaration[] Declarations;

        static DeclarationArray()
        {
            Serializer.InitializeForType<DeclarationArray>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            Serializer.Serialize(this, out byte[] buffer, Encoding.UTF8, Endian.LittleEndian);
            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DeclarationArray FromBytes(byte[] bytes)
        {
            return Serializer.Deserialize<DeclarationArray>(bytes, 0, bytes.Length);
        }
    }
}
