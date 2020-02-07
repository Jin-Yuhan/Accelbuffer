namespace Accelbuffer
{
    /// <summary>
    /// 表示一个对象的数据类型（4位）
    /// </summary>
    internal enum ObjectType
    {
        /// <summary>
        /// 丢失的类型
        /// </summary>
        Missing = 0,

        /// <summary>
        /// 8位固定长度类型
        /// </summary>
        Fixed8 = 1,

        /// <summary>
        /// 16位固定长度类型
        /// </summary>
        Fixed16 = 2,

        /// <summary>
        /// 24位固定长度类型
        /// </summary>
        Fixed24 = 3,

        /// <summary>
        /// 32位固定长度类型
        /// </summary>
        Fixed32 = 4,

        /// <summary>
        /// 40位固定长度类型
        /// </summary>
        Fixed40 = 5,

        /// <summary>
        /// 48位固定长度类型
        /// </summary>
        Fixed48 = 6,

        /// <summary>
        /// 56位固定长度类型
        /// </summary>
        Fixed56 = 7,

        /// <summary>
        /// 64位固定长度类型
        /// </summary>
        Fixed64 = 8,

        /// <summary>
        /// 72位固定长度类型
        /// </summary>
        Fixed72 = 9,

        /// <summary>
        /// 80位固定长度类型
        /// </summary>
        Fixed80 = 10,

        /// <summary>
        /// 88位固定长度类型
        /// </summary>
        Fixed88 = 11,

        /// <summary>
        /// 96位固定长度类型
        /// </summary>
        Fixed96 = 12,

        /// <summary>
        /// 104位固定长度类型
        /// </summary>
        Fixed104 = 13,

        /// <summary>
        /// 128位固定长度类型
        /// </summary>
        Fixed128 = 14,

        /// <summary>
        /// 前缀长度类型（Variant字节长度 + N字节数据）
        /// </summary>
        LengthPrefixed = 15
    }
}
