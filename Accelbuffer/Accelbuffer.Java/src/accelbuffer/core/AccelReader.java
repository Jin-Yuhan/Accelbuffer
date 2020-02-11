package accelbuffer.core;

/**
 * 公开对字节数据的读取与转换接口
 */
public final class AccelReader
{
    private final byte[] source;
    private final int startIndex;
    private final Encoding encoding;
    private final boolean isLittleEndian;
    private int readCount;
    private int tag;

    AccelReader(byte[] source, int startIndex, Encoding encoding, boolean isLittleEndian)
    {
        this.source = source;
        this.startIndex = startIndex;
        this.encoding = encoding;
        this.isLittleEndian = isLittleEndian;
    }




    private int readInt32Variant()
    {
        int value = 0;
        byte b;
        int count = 0;

        do
        {
            b = 0;//ReadByte();
            value |= ((b & 0x7F) << count);
            count += 7;
        } while ((b & 0x80) == 0x80);

        return value;
    }

    private int readLengthByType(ObjectType type)
    {
        switch (type)
        {
            case FIXED8: return 1;
            case FIXED16: return 2;
            case FIXED24: return 3;
            case FIXED32: return 4;
            case FIXED40: return 5;
            case FIXED48: return 6;
            case FIXED56: return 7;
            case FIXED64: return 8;
            case FIXED72: return 9;
            case FIXED80: return 10;
            case FIXED88: return 11;
            case FIXED96: return 12;
            case FIXED104: return 13;
            case FIXED128: return 16;
            case LENGTH_PREFIXED: return readInt32Variant();
            default: return -1;
        }
    }

    private int readIndexFromCachedTag()
    {
        return (int)(tag >> 4);
    }

    private ObjectType readTypeFromCachedTag()
    {
        return ObjectType.values()[tag & 0xF];
    }

    //zag

    static Encoding getEncodingConfig(byte config)
    {
        return Encoding.values()[config >> 4];
    }

    static Endian getEndianConfig(byte config)
    {
        return Endian.values()[config & 0xF];
    }
}
