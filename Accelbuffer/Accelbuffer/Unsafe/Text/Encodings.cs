using System;

namespace Accelbuffer.Unsafe.Text
{
    /// <summary>
    /// 提供目前支持的字符串编码
    /// </summary>
    [CLSCompliant(false)]
    public static class Encodings
    {
        /// <summary>
        /// ASCII编码
        /// </summary>
        public static IUnsafeEncoding ASCII { get; } = new ASCIIEncoding();

        /// <summary>
        /// 使用系统字节序的Unicode编码
        /// </summary>
        public static IUnsafeEncoding Unicode { get; } = new UnicodeEncoding();

        /// <summary>
        /// 与系统字节序相反的Unicode编码
        /// </summary>
        public static IUnsafeEncoding ReversedUnicode { get; } = new ReversedUnicodeEncoding();

        /// <summary>
        /// UTF8编码
        /// </summary>
        public static IUnsafeEncoding UTF8 { get; } = new UTF8Encoding();
    }
}
