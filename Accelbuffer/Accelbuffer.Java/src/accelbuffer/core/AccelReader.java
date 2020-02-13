package accelbuffer.core;

/**
 * 公开对字节数据的读取与转换接口
 */
public final class AccelReader {
    private final byte[] source;
    private final Encoding encoding;
    private final boolean isLittleEndian;
    private int readCount;
    private int tag;

    AccelReader(byte[] source, Encoding encoding, boolean isLittleEndian, int startIndex) {
        this.source = source;
        this.encoding = encoding;
        this.isLittleEndian = isLittleEndian;
        this.readCount = startIndex;
    }

    /**
     * 获取是否还有下一个值
     * @return true，如果有下一个值，false，则没有下一个值
     */
    public boolean hasNext() {
        if(readCount < source.length) {
            tag = readInt32Variant();
            return true;
        }

        tag = 0;
        return false;
    }

    /**
     * 获取当前读取的tag中缓存的索引的值
     * @return 当前读取的tag中缓存的索引的值
     */
    public int getCurrentIndex(){
        return tag >> 4;
    }

    private ObjectType getCurrentType() {
        return ObjectType.values()[tag & 0xF];
    }

    /**
     * 跳过下一个值
     * @throws UnsupportedOperationException 下一个值的类型是非法的
     */
    public void skipNext() throws UnsupportedOperationException{
        ObjectType type = getCurrentType();
        int len = readLengthByType(type);

        if(len < 0) {
            throw new UnsupportedOperationException();
        }

        readCount += len;
    }

    public byte readUInt8() throws ClassCastException {
        ObjectType type = getCurrentType();

        if(type != ObjectType.FIXED8){
            throw new ClassCastException("不存在从" + type.name() + "到uint8的隐式转换");
        }

        return source[readCount++];
    }

    public short readInt16() throws ClassCastException {
        ObjectType type = getCurrentType();

        if(type != ObjectType.FIXED16){
            throw new ClassCastException("不存在从" + type.name() + "到int16的隐式转换");
        }

        int b1 = source[readCount++];
        int b2 = source[readCount++];

        if(isLittleEndian) {
            return (short)(b1 | (b2 << 8));
        }
        return (short)((b1 << 8) | b2);
    }

    public int readInt32() throws ClassCastException {
        ObjectType type = getCurrentType();

        if(type != ObjectType.FIXED32){
            throw new ClassCastException("不存在从" + type.name() + "到int32的隐式转换");
        }

        int b1 = source[readCount++];
        int b2 = source[readCount++];
        int b3 = source[readCount++];
        int b4 = source[readCount++];

        if(isLittleEndian) {
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }
        return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
    }

    public long readInt64() throws ClassCastException {
        ObjectType type = getCurrentType();

        if(type != ObjectType.FIXED64){
            throw new ClassCastException("不存在从" + type.name() + "到int64的隐式转换");
        }

        long b1 = source[readCount++];
        long b2 = source[readCount++];
        long b3 = source[readCount++];
        long b4 = source[readCount++];
        long b5 = source[readCount++];
        long b6 = source[readCount++];
        long b7 = source[readCount++];
        long b8 = source[readCount++];

        if(isLittleEndian) {
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24) | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }
        return (b1 << 56) | (b2 << 48) | (b3 << 40) | (b4 << 32) | (b5 << 24) | (b6 << 16) | (b7 << 8) | b8;
    }

    public float readFloat32() throws ClassCastException {
        ObjectType type = getCurrentType();

        if(type != ObjectType.FIXED32){
            throw new ClassCastException("不存在从" + type.name() + "到float32的隐式转换");
        }

        int b1 = source[readCount++];
        int b2 = source[readCount++];
        int b3 = source[readCount++];
        int b4 = source[readCount++];

        if(isLittleEndian) {
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }
        return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
    }

    public double readFloat64() throws ClassCastException {
        ObjectType type = getCurrentType();

        if(type != ObjectType.FIXED64){
            throw new ClassCastException("不存在从" + type.name() + "到float64的隐式转换");
        }

        long b1 = source[readCount++];
        long b2 = source[readCount++];
        long b3 = source[readCount++];
        long b4 = source[readCount++];
        long b5 = source[readCount++];
        long b6 = source[readCount++];
        long b7 = source[readCount++];
        long b8 = source[readCount++];

        if(isLittleEndian) {
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24) | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }
        return (b1 << 56) | (b2 << 48) | (b3 << 40) | (b4 << 32) | (b5 << 24) | (b6 << 16) | (b7 << 8) | b8;
    }


    private int readInt32Variant() {
        int value = 0;
        byte b;
        int count = 0;

        do {
            b = source[readCount++];
            value |= ((b & 0x7F) << count);
            count += 7;
        } while ((b & 0x80) == 0x80);

        return value;
    }

    private int readLengthByType(ObjectType type) {
        switch (type) {
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

    //zag

    static Encoding getEncodingConfig(byte config) {
        return Encoding.values()[config >> 4];
    }

    static Endian getEndianConfig(byte config) {
        return Endian.values()[config & 0xF];
    }
}
