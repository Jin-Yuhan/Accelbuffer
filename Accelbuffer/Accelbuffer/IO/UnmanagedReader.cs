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
    public unsafe ref struct UnmanagedReader
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

        internal bool IsDefault()
        {
            return m_Buffer == null;
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
        /// <param name="bufferIndex">开始写入数据的索引</param>
        /// <param name="length">读取的字节长度</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/>为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>是负数或buffer长度不足</exception>
        /// <exception cref="IndexOutOfRangeException">内部缓冲区长度不足</exception>
        public void ReadBytes(byte[] buffer, int bufferIndex, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "buffer为null");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "读取字节的长度必须是非负数");
            }

            if (buffer.Length - bufferIndex < length)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), "buffer长度不足");
            }

            fixed (byte* p = buffer)
            {
                ReadBytesPrivate(p + bufferIndex, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadBytesPrivate(byte* buffer, int length)
        {
            if (m_ReadCount + length > m_Size)
            {
                throw new IndexOutOfRangeException("内部缓冲区长度不足");
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
        private T ReadNumber<T>(byte index, ValueTypeCode expected, bool unsigned) where T : unmanaged
        {
            if (ReadByte() != index)
            {
                OnIndexNotMatch(index);
                return default;
            }

            ValueTypeCode typeCode;
            NumberSign sign = NumberSign.PositiveOrUnsigned;
            int byteCount;

            if (expected == ValueTypeCode.FixedInteger || expected == ValueTypeCode.FixedFloat)
            {
                ToFixedNumberTag(ReadByte(), out typeCode, out byteCount);

                if (byteCount != sizeof(T) && byteCount != 0)
                {
                    throw new TagDismatchException(sizeof(T), byteCount);
                }
            }
            else
            {
                ToVariableNumberTag(ReadByte(), out typeCode, out sign, out byteCount);

                if (byteCount > sizeof(T))
                {
                    throw new TagDismatchException(sizeof(T), byteCount);
                }

                if (unsigned && sign == NumberSign.Negative)
                {
                    throw new TagDismatchException(NumberSign.PositiveOrUnsigned, sign);
                }
            }

            if (typeCode != expected)
            {
                throw new TagDismatchException(expected, typeCode);
            }

            T value = default;
            ReadBytesPrivate((byte*)&value, byteCount);

            if (sign == NumberSign.Negative)
            {
                OnesComplement((byte*)&value, sizeof(T));
            }

            return value;
        }

        /// <summary>
        /// 读取一个动态长度的8位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public sbyte ReadInt8(byte index, Number numberType)
        {
            return ReadNumber<sbyte>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, false);
        }

        /// <summary>
        /// 读取一个动态长度的8位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public byte ReadUInt8(byte index, Number numberType)
        {
            return ReadNumber<byte>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, true);
        }

        /// <summary>
        /// 读取一个动态长度的16位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public short ReadInt16(byte index, Number numberType)
        {
            return ReadNumber<short>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, false);
        }

        /// <summary>
        /// 读取一个动态长度的16位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public ushort ReadUInt16(byte index, Number numberType)
        {
            return ReadNumber<ushort>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, true);
        }

        /// <summary>
        /// 读取一个动态长度的32位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public int ReadInt32(byte index, Number numberType)
        {
            return ReadNumber<int>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, false);
        }

        /// <summary>
        /// 读取一个动态长度的32位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public uint ReadUInt32(byte index, Number numberType)
        {
            return ReadNumber<uint>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, true);
        }

        /// <summary>
        /// 读取一个动态长度的64位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public long ReadInt64(byte index, Number numberType)
        {
            return ReadNumber<long>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, false);
        }

        /// <summary>
        /// 读取一个动态长度的64位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public ulong ReadUInt64(byte index, Number numberType)
        {
            return ReadNumber<ulong>(index, numberType == Number.Var ? ValueTypeCode.VariableInteger : ValueTypeCode.FixedInteger, true);
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
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public float ReadFloat32(byte index, Number numberType)
        {
            return ReadNumber<float>(index, numberType == Number.Var ? ValueTypeCode.VariableFloat : ValueTypeCode.FixedFloat, true);
        }

        /// <summary>
        /// 读取一个动态长度的64位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="numberType">数字类型</param>
        /// <returns>读取数字</returns>
        /// <exception cref="MissingSerializedValueException">（StrictMode下）序列化数据丢失</exception>
        /// <exception cref="IndexOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="TagDismatchException">序列化标签不匹配</exception>
        public double ReadFloat64(byte index, Number numberType)
        {
            return ReadNumber<double>(index, numberType == Number.Var ? ValueTypeCode.VariableFloat : ValueTypeCode.FixedFloat, true);
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
