package accelbuffer.core;

/**
 * 表示字符串的编码
 */
public enum Encoding
{
    /**
     *指示字符串使用UTF8进行编码
     */
    UTF8(0),
    /**
     *指示字符串使用Unicode进行编码
     */
    Unicode(1),
    /**
     *指示字符串使用ASCII进行编码
     */
    ASCII(2);

    private int value;

    private Encoding(int value)
    {
        this.value = value;
    }

    public int getValue()
    {
        return value;
    }
}
