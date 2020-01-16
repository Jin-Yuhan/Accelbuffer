namespace Accelbuffer.Text
{
    /// <summary>
    /// 公开对字符串编码的接口
    /// </summary>
    public static class Encodings
    {
        /// <summary>
        /// ASCII编码
        /// </summary>
        public static ITextEncoding ASCII { get; } = new ASCIIEncoding();

        /// <summary>
        /// Unicode编码
        /// </summary>
        public static ITextEncoding Unicode { get; } = new UnicodeEncoding();

        /// <summary>
        /// UTF8编码
        /// </summary>
        public static ITextEncoding UTF8 { get; } = new UTF8Encoding();

        /// <summary>
        /// 获取指定的文本编码对象
        /// </summary>
        /// <param name="encoding">编码类型</param>
        /// <returns>文本编码</returns>
        public static ITextEncoding GetEncoding(Encoding encoding)
        {
            switch (encoding)
            {
                case Encoding.ASCII: return ASCII;
                case Encoding.Unicode: return Unicode;
                case Encoding.UTF8: return UTF8;
                default: return null;
            }
        }
    }
}
