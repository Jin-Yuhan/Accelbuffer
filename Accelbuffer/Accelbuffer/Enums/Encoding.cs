namespace Accelbuffer
{
    /// <summary>
    /// 表示字符串的编码
    /// </summary>
    public enum Encoding : byte
    {
        /// <summary>
        /// 指示字符串使用UTF8进行编码
        /// </summary>
        UTF8 = 0,

        /// <summary>
        /// 指示字符串使用Unicode进行编码
        /// </summary>
        Unicode = 1,

        /// <summary>
        /// 指示字符串使用ASCII进行编码
        /// </summary>
        ASCII = 2
    }
}
