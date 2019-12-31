using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static Accelbuffer.TagUtility;

namespace Accelbuffer
{
    /// <summary>
    /// 使用特定编码将可被序列化的基元类型写为二进制值
    /// </summary>
    public unsafe struct UnmanagedWriter
    {
        private byte* m_Buffer;
        private long m_ByteCount;
        internal long m_Size;
        private readonly bool m_CanFreePublicly;

        internal UnmanagedWriter(long defaultSize, bool canFreePublicly)
        {
            m_Buffer = (byte*)Marshal.AllocHGlobal(new IntPtr(defaultSize)).ToPointer();
            m_ByteCount = 0;
            m_Size = defaultSize;
            m_CanFreePublicly = canFreePublicly;
        }

        internal void Free()
        {
            if (m_Buffer != null)
            {
                Marshal.FreeHGlobal(new IntPtr(m_Buffer));
                m_Buffer = null;
                m_Size = 0;
                m_ByteCount = 0;
            }
        }

        internal void Reset()
        {
            m_ByteCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSize(long min)
        {
            long size = m_Size << 1;
            m_Size = size < min ? min : size;
            m_Buffer = (byte*)Marshal.ReAllocHGlobal(new IntPtr(m_Buffer), new IntPtr(m_Size)).ToPointer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteBytesPrivate(byte* bytes, int length)
        {
            long size = m_ByteCount + length;

            if (size >= m_Size)
            {
                EnsureSize(size);
            }

            byte* p = m_Buffer + m_ByteCount;

            while (length > 0)
            {
                *p++ = *bytes++;
                m_ByteCount++;
                length--;
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
        /// 尝试释放对象占用的内存
        /// </summary>
        /// <returns>是否释放成功</returns>
        public bool TryFree()
        {
            if (m_CanFreePublicly)
            {
                Free();
            }

            return m_CanFreePublicly;
        }

        /// <summary>
        /// 尝试重置对象
        /// </summary>
        /// <returns>是否重置成功</returns>
        public bool TryReset()
        {
            if (m_CanFreePublicly)
            {
                Reset();
            }

            return m_CanFreePublicly;
        }

        /// <summary>
        /// 将当前缓冲区内的所有字节都写入托管字节数组并返回
        /// </summary>
        /// <returns>托管字节数组</returns>
        /// <exception cref="ObjectDisposedException">内存已经被释放</exception>
        public byte[] ToArray()
        {
            if (m_Buffer == null)
            {
                throw new ObjectDisposedException("无法将数据写入托管字节数组，因为内存已经被释放");
            }

            byte[] result = m_ByteCount == 0 ? Array.Empty<byte>() : new byte[m_ByteCount];
            CopyToArray(result, 0);
            return result;
        }

        /// <summary>
        /// 将当前缓冲区内的所有字节都拷贝至托管字节数组
        /// </summary>
        /// <param name="array">需要被拷贝到的字节数组</param>
        /// <param name="index"></param>
        /// <exception cref="ArgumentException">字节数组容量不足</exception>
        /// <returns>拷贝的字节数量</returns>
        public long CopyToArray(byte[] array, long index)
        {
            long count = m_ByteCount;

            if (array.LongLength - index < count)
            {
                throw new ArgumentException("字节数组容量不足");
            }

            byte* source = m_Buffer;

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

        /// <summary>
        /// 将<paramref name="b"/>写入缓冲区
        /// </summary>
        /// <param name="b">写入的字节</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte b)
        {
            long size = m_ByteCount + 1;

            if (size >= m_Size)
            {
                EnsureSize(size);
            }

            m_Buffer[m_ByteCount] = b;
            m_ByteCount++;
        }

        /// <summary>
        /// 从<paramref name="bytes"/>中读取<paramref name="length"/>个字节并写入缓冲区
        /// </summary>
        /// <param name="bytes">写入的字节</param>
        /// <param name="length">写入的字节长度</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/>是null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>是负数</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(byte* bytes, int length)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), "输入的字节是空");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "写入的字节数必须是非负数");
            }

            WriteBytesPrivate(bytes, length);
        }

        /// <summary>
        /// 使用指定选项写入一个8位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, sbyte value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个8位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, byte value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate(&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个32位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, int value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个32位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, uint value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个16位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, short value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个16位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, ushort value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个64位有符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, long value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) : 
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个64位无符号整数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, ulong value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedIntegerTag(value, out int byteCount) :
                MakeVariableIntegerTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 写入一个布尔值
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        public void WriteValue(byte index, bool value)
        {
            WriteByte(index);
            WriteByte(MakeBooleanTag(value));
        }

        /// <summary>
        /// 使用指定编码写入一个字符
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="encoding">编码类型</param>
        public void WriteValue(byte index, char value, CharEncoding encoding)
        {
            byte tag = MakeCharTag(value, encoding, out bool isDefaultValue);
            WriteByte(index);
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
        /// 使用指定选项写入一个32位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, float value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedFloatTag(value, out int byteCount) : 
                MakeVariableFloatTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定选项写入一个64位浮点数
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="type">数字序列化类型</param>
        public void WriteValue(byte index, double value, Number type)
        {
            byte tag = type == Number.Fixed ? MakeFixedFloatTag(value, out int byteCount) :
                MakeVariableFloatTag(&value, out byteCount);
            WriteByte(index);
            WriteByte(tag);
            WriteBytesPrivate((byte*)&value, byteCount);
        }

        /// <summary>
        /// 使用指定编码写入一个字符串
        /// </summary>
        /// <param name="index">序列化索引</param>
        /// <param name="value">需要写入的值</param>
        /// <param name="encoding">编码类型</param>
        public void WriteValue(byte index, string value, CharEncoding encoding)
        {
            byte tag = MakeCharTag(value, encoding, out bool isDefaultValue, out bool isEmpty);
            WriteByte(index);
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
