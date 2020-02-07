namespace Accelbuffer
{
    /// <summary>
    /// 指示字节序
    /// </summary>
    public enum Endian : byte
    {
        /// <summary>
        /// 指示使用大端字节序（网络字节序）
        /// </summary>
        BigEndian = 0,

        /// <summary>
        /// 指示使用小端字节序
        /// </summary>
        LittleEndian = 1
    }
}
