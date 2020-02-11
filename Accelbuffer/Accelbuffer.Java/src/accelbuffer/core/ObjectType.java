package accelbuffer.core;

/**
 * 表示一个对象的数据类型（4位）
 */
enum ObjectType
{
    /**
     * 丢失的类型
     */
    MISSING(0),
    /**
     * 8位固定长度类型
     */
    FIXED8(1),
    /**
     * 16位固定长度类型
     */
    FIXED16 (2),
    /**
     * 24位固定长度类型
     */
    FIXED24(3),
    /**
     * 32位固定长度类型
     */
    FIXED32(4),
    /**
     * 40位固定长度类型
     */
    FIXED40(5),
    /**
     * 48位固定长度类型
     */
    FIXED48(6),
    /**
     * 56位固定长度类型
     */
    FIXED56(7),
    /**
     * 64位固定长度类型
     */
    FIXED64(8),
    /**
     * 72位固定长度类型
     */
    FIXED72(9),
    /**
     * 80位固定长度类型
     */
    FIXED80(10),
    /**
     * 88位固定长度类型
     */
    FIXED88(11),
    /**
     * 96位固定长度类型
     */
    FIXED96(12),
    /**
     * 104位固定长度类型
     */
    FIXED104(13),
    /**
     * 128位固定长度类型
     */
    FIXED128(14),
    /**
     * 前缀长度类型（Variant字节长度 + N字节数据）
     */
    LENGTH_PREFIXED(15);

    private int value;

    private ObjectType(int value)
    {
        this.value = value;
    }

    public int getValue()
    {
        return value;
    }
}
