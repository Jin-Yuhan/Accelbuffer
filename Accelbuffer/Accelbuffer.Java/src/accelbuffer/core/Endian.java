package accelbuffer.core;

/**
 *指示字节序
 */
public enum Endian {
    /**
     *指示使用大端字节序（网络字节序）
     */
    BIG_ENDIAN(0),
    /**
     *指示使用小端字节序
     */
    LITTLE_ENDIAN(1);

    private int value;

    private Endian(int value) {
        this.value = value;
    }

    public int getValue() {
        return value;
    }
}
