using System;
using System.Runtime.CompilerServices;
using System.Text;
using static Accelbuffer.TagUtility;

namespace Accelbuffer
{
    /// <summary>
    /// 使用特定编码将可被序列化的基元类型写为二进制值
    /// </summary>
    public unsafe ref struct UnmanagedWriter
    {
        private readonly UnmanagedMemoryAllocator m_Allocator;
        private long m_ByteCount;

        internal UnmanagedWriter(UnmanagedMemoryAllocator allocator)
        {
            m_Allocator = allocator;
            m_ByteCount = 0;
        }

        internal bool IsDefault()
        {
            return m_Allocator == null;
        }

        internal byte[] ToArray()
        {
            byte[] result = m_ByteCount == 0 ? Array.Empty<byte>() : new byte[m_ByteCount];
            CopyToArray(result, 0);
            return result;
        }

        internal long CopyToArray(byte[] array, long index)
        {
            long count = m_ByteCount;

            if (array.LongLength - index < count)
            {
                throw new ArgumentException("字节数组容量不足");
            }

            byte* source = m_Allocator.GetMemoryPtr(count, 0L);

            fixed (byte* ptr = array)
            {
                byte* p = ptr + index;

                while (count-- > 0)
                {
                    *p++ = *source++;
                }
            }

            return m_ByteCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteBytesPrivate(byte* bytes, int length)
        {
            byte* p = m_Allocator.GetMemoryPtr(m_ByteCount + length, m_ByteCount);

            while (length-- > 0)
            {
                *p++ = *bytes++;
                m_ByteCount++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteLength(uint value)
        {
            byte tag = MakeVariableIntegerTag(&value, out int byteCount);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 写入一个字节
        /// </summary>
        /// <param name="b">写入的字节</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte b)
        {
            byte* p = m_Allocator.GetMemoryPtr(m_ByteCount + 1, m_ByteCount);
            m_ByteCount++;
            *p = b;
        }

        /// <summary>
        /// 写入<paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes">写入的字节</param>
        /// <param name="index">起始的索引位置</param>
        /// <param name="length">写入的字节长度</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>是null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bytes"/>长度不足</exception>
        public void WriteBytes(byte[] bytes, int index, int length)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), "输入的数组是null");
            }

            if (bytes.Length - index < length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "数组长度不足");
            }

            fixed (byte* p = bytes)
            {
                WriteBytesPrivate(p + index, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteIndex(byte index)
        {
            if (index != 0)
            {
                WriteByte(index);
            }
        }

        /// <summary>
        /// 使用指定类型写入一个8位有符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, sbyte value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个8位无符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, byte value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate(&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个32位有符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, int value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个32位无符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, uint value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个16位有符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, short value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个16位无符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, ushort value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个64位有符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, long value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个64位无符号整数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, ulong value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 写入一个布尔值
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        public void WriteValue(byte index, bool value)
        {
            WriteIndex(index);
            WriteByte(MakeBooleanTag(value));
        }

        /// <summary>
        /// 使用指定编码写入一个字符
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="encoding">编码类型</param>
        public void WriteValue(byte index, char value, CharEncoding encoding)
        {
            byte tag = MakeCharTag(value, encoding, out bool isDefaultValue);
            WriteIndex(index);
            WriteByte(tag);

            if (isDefaultValue)
            {
                return;
            }

            switch (encoding)
            {
                case CharEncoding.Unicode:
                    WriteBytesPrivate((byte*)&value, 2);
                    break;

                case CharEncoding.ASCII:
                    byte asciiByte;
                    Encoding.ASCII.GetBytes(&value, 1, &asciiByte, 1);
                    WriteByte(asciiByte);
                    break;

                default://case CharEncoding.UTF8:
                    byte* utf8CharByte = stackalloc byte[4];
                    WriteBytesPrivate(utf8CharByte, Encoding.UTF8.GetBytes(&value, 1, utf8CharByte, 4));
                    break;
            }
        }

        /// <summary>
        /// 使用指定类型写入一个32位浮点数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, float value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedFloatTag(value, out int byteCount) : 
                MakeVariableFloatTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定类型写入一个64位浮点数
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, double value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedFloatTag(value, out int byteCount) :
                MakeVariableFloatTag(&value, out byteCount);
            WriteIndex(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定编码写入一个字符串
        /// </summary>
        /// <param name="index">序列化索引，0表示不写入索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="encoding">编码类型</param>
        public void WriteValue(byte index, string value, CharEncoding encoding)
        {
            byte tag = MakeCharTag(value, encoding, out bool isDefaultValue, out bool isEmpty);
            WriteIndex(index);
            WriteByte(tag);

            if (isDefaultValue || isEmpty)
            {
                return;
            }

            int byteCount;
            byte* bytes;

            fixed (char* p = value)
            {
                switch (encoding)
                {
                    case CharEncoding.Unicode:
                        int unicodeLen = value.Length << 1;
                        byte* unicodeByte = stackalloc byte[unicodeLen];
                        byteCount = Encoding.Unicode.GetBytes(p, value.Length, unicodeByte, unicodeLen);
                        bytes = unicodeByte;
                        break;

                    case CharEncoding.ASCII:
                        byte* asciiByte = stackalloc byte[value.Length];
                        byteCount = Encoding.ASCII.GetBytes(p, value.Length, asciiByte, value.Length);
                        bytes = asciiByte;
                        break;

                    default://case CharEncoding.UTF8:
                        int utf8Len = value.Length << 2;
                        byte* utf8Byte = stackalloc byte[utf8Len];
                        byteCount = Encoding.UTF8.GetBytes(p, value.Length, utf8Byte, utf8Len);
                        bytes = utf8Byte;
                        break;
                }
            }

            WriteLength((uint)byteCount);
            WriteBytesPrivate(bytes, byteCount);
        }
    }
}
