using System;
using System.Runtime.CompilerServices;
using System.Text;
using static Accelbuffer.SerializationUtility;
using static Accelbuffer.TagUtility;

namespace Accelbuffer
{
    /// <summary>
    /// 使用特定编码将二进制值读取为基元类型
    /// </summary>
    public unsafe struct UnmanagedReader
    {
        private readonly byte* m_Buffer;
        private readonly long m_Size;
        private readonly bool m_StrictMode;
        private long m_ReadCount;

        internal UnmanagedReader(byte* source, bool strictMode, long size)
        {
            m_Buffer = source;
            m_Size = size;
            m_StrictMode = strictMode;
            m_ReadCount = 0;
        }

        /// <summary>
        /// 从内部缓冲区中读取一个字节并返回
        /// </summary>
        /// <returns>读取的字节</returns>
        /// <exception cref="IndexOutOfRangeException">缓冲区已经读取至末尾</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            if (m_ReadCount == m_Size)
            {
                throw new IndexOutOfRangeException("缓冲区已经读取至末尾");
            }

            return *(m_Buffer + m_ReadCount++);
        }

        /// <summary>
        /// 从内部缓冲区读取<paramref name="length"/>个字节并写入<paramref name="buffer"/>中
        /// </summary>
        /// <param name="buffer">写入数据的缓冲区</param>
        /// <param name="length">读取的字节长度</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>是负数</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        public void ReadBytes(byte* buffer, long length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "写入数据的缓冲区为null");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "读取字节的长度必须是非负数");
            }

            ReadBytesPrivate(buffer, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadBytesPrivate(byte* buffer, long length)
        {
            if (m_ReadCount + length > m_Size)
            {
                throw new IndexOutOfRangeException("缓冲区长度不足");
            }

            while (length > 0)
            {
                *buffer++ = *(m_Buffer + m_ReadCount++);
                length--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnIndexNotMatch(byte index)
        {
            if (m_StrictMode)
            {
                throw new MissingSerializedValueException(index);
            }

            m_ReadCount--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadLength()
        {
            ToVariableNumberTag(ReadByte(), out _, out _, out int byteCount);
            uint value = default;
            ReadBytesPrivate((byte*)&value, byteCount);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T ReadUVarNumber<T>(byte index, ValueTypeCode expected) where T : unmanaged
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ToVariableNumberTag(ReadByte(), out ValueTypeCode typeCode, out NumberSign sign, out int byteCount);

            if (typeCode != expected)
            {
                throw new TagDismatchException(expected, typeCode);
            }

            if (sign != NumberSign.PositiveOrUnsigned)
            {
                throw new TagDismatchException(NumberSign.PositiveOrUnsigned, sign);
            }

            if (byteCount > sizeof(T))
            {
                throw new TagDismatchException(sizeof(T), byteCount);
            }

            T value = default;

            ReadBytesPrivate((byte*)&value, byteCount);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T ReadSVarNumber<T>(byte index, ValueTypeCode expected) where T : unmanaged
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ToVariableNumberTag(ReadByte(), out ValueTypeCode typeCode, out NumberSign sign, out int byteCount);

            if (typeCode != expected)
            {
                throw new TagDismatchException(expected, typeCode);
            }

            if (byteCount > sizeof(T))
            {
                throw new TagDismatchException(sizeof(T), byteCount);
            }

            T value = default;

            ReadBytesPrivate((byte*)&value, byteCount);

            if (sign == NumberSign.Negative)
            {
                OnesComplement((byte*)&value, sizeof(T));
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T ReadFixedNumber<T>(byte index, ValueTypeCode expected) where T : unmanaged
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ToFixedNumberTag(ReadByte(), out ValueTypeCode typeCode, out int byteCount);

            if (typeCode != expected)
            {
                throw new TagDismatchException(expected, typeCode);
            }

            if (byteCount != sizeof(T) && byteCount != 0)
            {
                throw new TagDismatchException(sizeof(T), byteCount);
            }

            T value = default;

            ReadBytesPrivate((byte*)&value, byteCount);

            return value;
        }

        /// <summary>
        /// 读取一个动态长度的8位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public sbyte ReadVariableInt8(byte index)
        {
            return ReadSVarNumber<sbyte>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的8位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public byte ReadVariableUInt8(byte index)
        {
            return ReadUVarNumber<byte>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的16位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public short ReadVariableInt16(byte index)
        {
            return ReadSVarNumber<short>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的16位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public ushort ReadVariableUInt16(byte index)
        {
            return ReadUVarNumber<ushort>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的32位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public int ReadVariableInt32(byte index)
        {
            return ReadSVarNumber<int>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的32位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public uint ReadVariableUInt32(byte index)
        {
            return ReadUVarNumber<uint>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的64位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public long ReadVariableInt64(byte index)
        {
            return ReadSVarNumber<long>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个动态长度的64位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public ulong ReadVariableUInt64(byte index)
        {
            return ReadUVarNumber<ulong>(index, ValueTypeCode.VariableInteger);
        }

        /// <summary>
        /// 读取一个固定长度的8位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public sbyte ReadFixedInt8(byte index)
        {
            return ReadFixedNumber<sbyte>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的8位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public byte ReadFixedUInt8(byte index)
        {
            return ReadFixedNumber<byte>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的16位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public short ReadFixedInt16(byte index)
        {
            return ReadFixedNumber<short>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的16位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public ushort ReadFixedUInt16(byte index)
        {
            return ReadFixedNumber<ushort>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的32位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public int ReadFixedInt32(byte index)
        {
            return ReadFixedNumber<int>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的32位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public uint ReadFixedUInt32(byte index)
        {
            return ReadFixedNumber<uint>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的64位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public long ReadFixedInt64(byte index)
        {
            return ReadFixedNumber<long>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个固定长度的64位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public ulong ReadFixedUInt64(byte index)
        {
            return ReadFixedNumber<ulong>(index, ValueTypeCode.FixedInteger);
        }

        /// <summary>
        /// 读取一个布尔值
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取的布尔值</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public bool ReadBoolean(byte index)
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ToBooleanTag(ReadByte(), out ValueTypeCode typeCode, out bool value);

            if (typeCode != ValueTypeCode.Boolean)
            {
                throw new TagDismatchException(ValueTypeCode.Boolean, typeCode);
            }

            return value;
        }

        /// <summary>
        /// 使用指定编码读取一个字符
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="encoding">字符编码类型</param>
        /// <returns>读取的字符</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        /// <exception cref="DecoderFallbackException">字符解码发生回退</exception>
        public char ReadChar(byte index, CharEncoding encoding)
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ToCharTag(ReadByte(), out ValueTypeCode typeCode, out CharType valueType, out bool isDefaultValue, out CharEncoding e, out _);

            if (typeCode != ValueTypeCode.Char)
            {
                throw new TagDismatchException(ValueTypeCode.Char, typeCode);
            }

            if (valueType != CharType.SingleChar)
            {
                throw new TagDismatchException(CharType.SingleChar, valueType);
            }

            if (encoding != e)
            {
                throw new TagDismatchException(encoding, e);
            }

            if (isDefaultValue)
            {
                return default;
            }

            char value;

            switch (encoding)
            {
                case CharEncoding.Unicode:
                    ReadBytesPrivate((byte*)&value, 2);
                    break;

                case CharEncoding.ASCII:
                    byte asciiByte;
                    asciiByte = ReadByte();
                    Encoding.ASCII.GetChars(&asciiByte, 1, &value, 1);
                    break;

                default://case CharEncoding.UTF8:
                    byte* utf8Byte = stackalloc byte[4];
                    byte b = *utf8Byte = ReadByte();
                    int byteCount = 1;

                    if ((b >> 3) == 0x1E)
                    {
                        ReadBytesPrivate(utf8Byte + 1, 3);
                        byteCount = 4;
                    }
                    else if ((b >> 4) == 0xE)
                    {
                        ReadBytesPrivate(utf8Byte + 1, 2);
                        byteCount = 3;
                    }
                    else if ((b >> 5) == 0x6)
                    {
                        ReadBytesPrivate(utf8Byte + 1, 1);
                        byteCount = 2;
                    }

                    Encoding.UTF8.GetChars(utf8Byte, byteCount, &value, 1);
                    break;
            }

            return value;
        }

        /// <summary>
        /// 读取一个动态长度的32位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public float ReadVariableFloat32(byte index)
        {
            return ReadUVarNumber<float>(index, ValueTypeCode.VariableFloat);
        }

        /// <summary>
        /// 读取一个动态长度的64位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public double ReadVariableFloat64(byte index)
        {
            return ReadUVarNumber<double>(index, ValueTypeCode.VariableFloat);
        }

        /// <summary>
        /// 读取一个固定长度的32位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public float ReadFixedFloat32(byte index)
        {
            return ReadFixedNumber<float>(index, ValueTypeCode.FixedFloat);
        }

        /// <summary>
        /// 读取一个固定长度的64位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public double ReadFixedFloat64(byte index)
        {
            return ReadFixedNumber<double>(index, ValueTypeCode.FixedFloat);
        }

        /// <summary>
        /// 使用一个编码读取一个字符串
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="encoding">字符编码类型</param>
        /// <returns>读取的字符串</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        /// <exception cref="DecoderFallbackException">字符串解码发生回退</exception>
        public string ReadString(byte index, CharEncoding encoding)
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ToCharTag(ReadByte(), out ValueTypeCode typeCode, out CharType valueType, out bool isDefaultValue, out CharEncoding e, out bool isEmpty);

            if (typeCode != ValueTypeCode.Char)
            {
                throw new TagDismatchException(ValueTypeCode.Char, typeCode);
            }

            if (valueType != CharType.String)
            {
                throw new TagDismatchException(CharType.String, valueType);
            }

            if (encoding != e)
            {
                throw new TagDismatchException(encoding, e);
            }

            if (isDefaultValue)
            {
                return default;
            }

            if (isEmpty)
            {
                return string.Empty;
            }

            string value;

            int len = (int)ReadLength();

            byte* bs = stackalloc byte[len];
            ReadBytesPrivate(bs, len);

            switch (encoding)
            {
                case CharEncoding.Unicode:
                    value = Encoding.Unicode.GetString(bs, len);
                    break;

                case CharEncoding.ASCII:
                    value = Encoding.ASCII.GetString(bs, len);
                    break;

                default://case CharEncoding.UTF32:
                    value = Encoding.UTF8.GetString(bs, len);
                    break;
            }

            return value;
        }
    }
}
