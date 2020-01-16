namespace Accelbuffer.Text
{
    /// <summary>
    /// 表示字符编码
    /// </summary>
    public enum Encoding : byte
    {
        /// <summary>
        /// 指示字符使用 <see cref="Encodings.Unicode"/> 进行编码
        /// </summary>
        Unicode = 0,

        /// <summary>
        /// 指示字符使用 <see cref="Encodings.ASCII"/> 进行编码
        /// </summary>
        ASCII = 1,

        /// <summary>
        /// 指示字符使用 <see cref="Encodings.UTF8"/> 进行编码
        /// </summary>
        UTF8 = 2,
    }
}
