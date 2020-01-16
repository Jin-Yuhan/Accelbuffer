namespace Accelbuffer
{
    /// <summary>
    /// 表示一个对象的数据类型（4位）
    /// </summary>
    internal enum ObjectType
    {
        /// <summary>
        /// 8位固定长度类型
        /// </summary>
        Fixed8 = 0,

        /// <summary>
        /// 16位固定长度类型
        /// </summary>
        Fixed16 = 1,

        /// <summary>
        /// 32位固定长度类型
        /// </summary>
        Fixed32 = 2,

        /// <summary>
        /// 64位固定长度类型
        /// </summary>
        Fixed64 = 3,

        /// <summary>
        /// 128位固定长度类型
        /// </summary>
        Fixed128 = 4,

        /// <summary>
        /// 动态长度类型 (1字节长度 + [0, 8]字节数据)
        /// </summary>
        Variant = 5,

        /// <summary>
        /// 前缀长度类型（Variant字节长度 + N字节数据）
        /// </summary>
        LengthPrefixed = 6,

        /// <summary>
        /// 使用ASCII编码的字符类型
        /// </summary>
        ASCIIChar = 7,

        /// <summary>
        /// 使用Unicode编码的字符类型
        /// </summary>
        UnicodeChar = 8,

        /// <summary>
        /// 使用UTF8编码字符类型
        /// </summary>
        UTF8Char = 9,

        /// <summary>
        /// 使用ASCII编码的字符串类型（Variant字节长度 + N字节数据）
        /// </summary>
        ASCIIString = 10,

        /// <summary>
        /// 使用Unicode编码的字符串类型（Variant字节长度 + N字节数据）
        /// </summary>
        UnicodeString = 11,

        /// <summary>
        /// 使用UTF8编码的字符串类型（Variant字节长度 + N字节数据）
        /// </summary>
        UTF8String = 12,

        /// <summary>
        /// 保留类型
        /// </summary>
        Reserved0 = 13,

        /// <summary>
        /// 保留类型
        /// </summary>
        Reserved1 = 14,

        /// <summary>
        /// 保留类型
        /// </summary>
        Reserved2 = 15
    }
}
