namespace Accelbuffer
{
    /// <summary>
    /// 数字类型选项
    /// </summary>
    public enum Number : byte
    {
        /// <summary>
        /// 指示数字使用固定长度编码
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// 指示数字使用动态长度编码
        /// </summary>
        Var = 1
    }
}
