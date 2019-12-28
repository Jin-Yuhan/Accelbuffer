namespace Accelbuffer
{
    /// <summary>
    /// 数字序列化选项
    /// </summary>
    public enum NumberOption : byte
    {
        /// <summary>
        /// 指示数字使用固定长度编码
        /// </summary>
        FixedLength = 0,
        /// <summary>
        /// 指示数字使用动态长度编码
        /// </summary>
        VariableLength = 1
    }
}
