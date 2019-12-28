namespace Accelbuffer
{
    /// <summary>
    /// 字符编码
    /// </summary>
    public enum CharEncoding : byte
    {
        /// <summary>
        /// 指示字符使用 <see cref="System.Text.Encoding.Unicode"/> 进行编码
        /// </summary>
        Unicode = 0,

        /// <summary>
        /// 指示字符使用 <see cref="System.Text.Encoding.ASCII"/> 进行编码
        /// </summary>
        ASCII = 1,

        /// <summary>
        /// 指示字符使用 <see cref="System.Text.Encoding.UTF8"/> 进行编码
        /// </summary>
        UTF8 = 2,
    }
}
