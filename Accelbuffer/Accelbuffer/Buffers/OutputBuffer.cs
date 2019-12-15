﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Accelbuffer
{
    /// <summary>
    /// 用于输出数据的缓冲区
    /// </summary>
    public unsafe struct OutputBuffer
    {
        private byte* m_Buffer;
        private long m_ByteCount;

        public long Size { get; private set; }

        internal OutputBuffer(long defaultSize)
        {
            if (defaultSize <= 0)
            {
                throw new ArgumentOutOfRangeException("字节缓冲区的大小不能小于等于0");
            }

            m_Buffer = (byte*)Marshal.AllocHGlobal(new IntPtr(defaultSize)).ToPointer();
            Size = defaultSize;
            m_ByteCount = 0;
        }

        internal void Free()
        {
            if (m_Buffer != null)
            {
                Marshal.FreeHGlobal(new IntPtr(m_Buffer));
                m_Buffer = null;
                Size = 0;
                m_ByteCount = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSize(long min)
        {
            long size = Size << 1;
            Size = size < min ? min : size;
            m_Buffer = (byte*)Marshal.ReAllocHGlobal(new IntPtr(m_Buffer), new IntPtr(Size)).ToPointer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reset()
        {
            m_ByteCount = 0;
        }

        public byte[] ToArray()
        {
            if (m_Buffer == null)
            {
                return null;
            }

            byte[] result = m_ByteCount == 0 ? Array.Empty<byte>() : new byte[m_ByteCount];
            long count = m_ByteCount;
            byte* source = m_Buffer;

            fixed (byte* ptr = result)
            {
                byte* p = ptr;

                while (count-- > 0)
                {
                    *p++ = *source++;
                }
            }

            return result;
        }

        public void WriteToStream(Stream stream)
        {
            if (m_Buffer == null || m_ByteCount == 0)
            {
                return;
            }

            long count = m_ByteCount;
            byte* source = m_Buffer;

            while (count-- > 0)
            {
                stream.WriteByte(*source++);
            }
        }


        public void WriteBytes(byte* bytes, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            long size = m_ByteCount + length;

            if (size >= Size)
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

        public void WriteByte(byte b)
        {
            long size = m_ByteCount + 1;

            if (size >= Size)
            {
                EnsureSize(size);
            }

            m_Buffer[m_ByteCount] = b;
            m_ByteCount++;
        }

        public void WriteValue(sbyte value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(byte value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes(&value, tag.ByteCount);
        }

        public void WriteValue(int value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(uint value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(short value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(ushort value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(long value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(ulong value)
        {
            IntegerTag tag = new IntegerTag(&value);
            WriteByte(tag.GetTagByte());
            WriteBytes((byte*)&value, tag.ByteCount);
        }

        public void WriteValue(bool value)
        {
            BooleanTag tag = new BooleanTag(value);
            WriteByte(tag.GetTagByte());
        }

        public void WriteValue(char value, CharEncoding encoding)
        {
            CharTag tag = new CharTag(value, encoding);
            WriteByte(tag.GetTagByte());

            if (tag.IsDefaultValue)
            {
                return;
            }

            switch (encoding)
            {
                case CharEncoding.Unicode:
                    WriteBytes((byte*)&value, 2);
                    break;

                case CharEncoding.ASCII:
                    byte asciiByte;
                    Encoding.ASCII.GetBytes(&value, 1, &asciiByte, 1);
                    WriteByte(asciiByte);
                    break;

                default://case CharEncoding.UTF8:
                    byte* utf8CharByte = stackalloc byte[4];
                    WriteBytes(utf8CharByte, Encoding.UTF8.GetBytes(&value, 1, utf8CharByte, 4));
                    break;
            }
        }

        public void WriteValue(float value)
        {
            FloatTag tag = new FloatTag(value);
            WriteByte(tag.GetTagByte());

            if (!tag.IsDefaultValue)
            {
                WriteBytes((byte*)&value, 4);
            }
        }

        public void WriteValue(double value)
        {
            FloatTag tag = new FloatTag(value);
            WriteByte(tag.GetTagByte());

            if (!tag.IsDefaultValue)
            {
                WriteBytes((byte*)&value, 8);
            }
        }

        public void WriteValue(string value, CharEncoding encoding)
        {
            CharTag tag = new CharTag(value, encoding);
            WriteByte(tag.GetTagByte());

            if (!tag.IsDefaultValue && !tag.IsEmptyString)
            {
                int byteCount;
                int maxByte = value.Length << 2;
                byte* bytes = stackalloc byte[maxByte];

                fixed (char* p = value)
                {

                    switch (encoding)
                    {
                        case CharEncoding.Unicode:
                            byteCount = Encoding.Unicode.GetBytes(p, value.Length, bytes, maxByte);
                            break;

                        case CharEncoding.ASCII:
                            byteCount = Encoding.ASCII.GetBytes(p, value.Length, bytes, maxByte);
                            break;

                        default://case CharEncoding.UTF8:
                            byteCount = Encoding.UTF8.GetBytes(p, value.Length, bytes, maxByte);
                            break;
                    }
                }

                WriteValue(byteCount);
                WriteBytes(bytes, byteCount);
            }
        }
    }
}
