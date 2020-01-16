using Accelbuffer.Memory;
using Accelbuffer.Properties;
using Accelbuffer.Injection;
using Accelbuffer.Text;
using System;
using System.Runtime.CompilerServices;

namespace Accelbuffer
{
    /// <summary>
    /// 提供将数据写入流的接口
    /// </summary>
    public unsafe ref struct StreamingWriter
    {
        private readonly MemoryAllocator m_Allocator;
        private int m_ByteCount;

        internal StreamingWriter(MemoryAllocator allocator)
        {
            m_Allocator = allocator;
            m_ByteCount = 0;
        }

        internal void BeginWrite()
        {
            m_Allocator.BeginThreadSafeMemoryWriting();
        }

        internal void EndWrite()
        {
            m_Allocator.EndThreadSafeMemoryWriting();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, sbyte value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed8 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(sbyte value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                WriteInt32Variant((uint)value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(sbyte));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, byte value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed8 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(byte value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                WriteInt32Variant(value);
            }
            else
            {
                WriteBytes(&value, sizeof(byte));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, short value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed16 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(short value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                Zig(&value);
                WriteInt32Variant((uint)value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(short));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, ushort value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed16 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(ushort value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                WriteInt32Variant(value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(ushort));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, int value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed32 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                Zig(&value);
                WriteInt32Variant((uint)value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(int));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, uint value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed32 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(uint value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                WriteInt32Variant(value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(uint));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, long value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed64 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(long value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                Zig(&value);
                WriteInt64Variant((ulong)value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(long));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(int index, ulong value, NumberFormat format)
        {
            WriteTag(index, format == NumberFormat.Fixed ? ObjectType.Fixed64 : ObjectType.Variant);
            WriteValue(value, format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void WriteValue(ulong value, NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                WriteInt64Variant(value);
            }
            else
            {
                WriteBytes((byte*)&value, sizeof(ulong));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, float value)
        {
            WriteTag(index, ObjectType.Fixed32);
            WriteValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(float value)
        {
            WriteBytes((byte*)&value, sizeof(float));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, double value)
        {
            WriteTag(index, ObjectType.Fixed64);
            WriteValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(double value)
        {
            WriteBytes((byte*)&value, sizeof(double));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, decimal value)
        {
            WriteTag(index, ObjectType.Fixed128);
            WriteValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(decimal value)
        {
            WriteBytes((byte*)&value, sizeof(decimal));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, bool value)
        {
            WriteTag(index, ObjectType.Fixed8);
            WriteValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        public void WriteValue(int index, char value, Encoding encoding)
        {
            ObjectType type;

            switch (encoding)
            {
                case Encoding.ASCII: type = ObjectType.ASCIIChar; break;
                case Encoding.Unicode: type = ObjectType.UnicodeChar; break;
                default: type = ObjectType.UTF8Char; break;
            }
            
            WriteTag(index, type);
            WriteValue(value, encoding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        public void WriteValue(char value, Encoding encoding)
        {
            ITextEncoding e = Encodings.GetEncoding(encoding);
            byte* p = (byte*)m_Allocator.GetMemoryPtr(m_ByteCount + 3, m_ByteCount).ToPointer();
            m_ByteCount += e.GetBytes(value, p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        public void WriteValue(int index, string value, Encoding encoding)
        {
            ObjectType type;

            switch (encoding)
            {
                case Encoding.ASCII: type = ObjectType.ASCIIString; break;
                case Encoding.Unicode: type = ObjectType.UnicodeString; break;
                default: type = ObjectType.UTF8String; break;
            }

            WriteTag(index, type);
            WriteValue(value, encoding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        public void WriteValue(string value, Encoding encoding)
        {
            ITextEncoding e;
            int byteCount;

            switch (encoding)
            {
                case Encoding.Unicode:
                    e = Encodings.Unicode;
                    byteCount = value.Length << 1;
                    break;

                case Encoding.ASCII:
                    e = Encodings.ASCII;
                    byteCount = value.Length;
                    break;

                default:
                    e = Encodings.UTF8;
                    byteCount = value.Length << 2;
                    break;
            }

            byte* buffer = stackalloc byte[byteCount];
            byteCount = e.GetBytes(value, buffer);

            WriteValue(byteCount, NumberFormat.Variant);
            WriteBytes(buffer, byteCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue<T>(int index, T value)
        {
            WriteTag(index, ObjectType.LengthPrefixed);
            WriteValue<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void WriteValue<T>(T value)
        {
            StreamingWriter writer = new StreamingWriter(SerializerInjector.InternalCache<T>.Allocator);

            writer.BeginWrite();

            try
            {
                SerializerInjector.InternalCache<T>.Serializer.Serialize(value, ref writer);
                WriteValue(writer.m_ByteCount, NumberFormat.Variant);
                WriteBytes((byte*)writer.m_Allocator.GetMemoryPtr(0, 0).ToPointer(), writer.m_ByteCount);
            }
            finally
            {
                writer.EndWrite();
            }
        }

        /// <summary>
        /// 写入一个标签
        /// </summary>
        /// <param name="index">字段的索引，必须是 (0, 268435456] 范围内的整数</param>
        /// <param name="fieldType">字段的类型</param>
        /// <exception cref="InvalidFieldIndexException">index超出范围</exception>
        public void WriteTag(int index, FieldType fieldType)
        {
            ObjectType type;

            switch (fieldType)
            {
                case FieldType.Boolean:
                case FieldType.Int8:
                case FieldType.UInt8: type = ObjectType.Fixed8; break;
                case FieldType.Int16:
                case FieldType.UInt16: type = ObjectType.Fixed16; break;
                case FieldType.Int32:
                case FieldType.UInt32:
                case FieldType.Float32: type = ObjectType.Fixed32; break;
                case FieldType.Int64:
                case FieldType.UInt64:
                case FieldType.Float64: type = ObjectType.Fixed64; break;
                case FieldType.Float128: type = ObjectType.Fixed128; break;
                case FieldType.ASCIIChar: type = ObjectType.ASCIIChar; break;
                case FieldType.UnicodeChar: type = ObjectType.UnicodeChar; break;
                case FieldType.UTF8Char: type = ObjectType.UTF8Char; break;
                case FieldType.ASCIIString: type = ObjectType.ASCIIString; break;
                case FieldType.UnicodeString: type = ObjectType.UnicodeString; break;
                case FieldType.UTF8String: type = ObjectType.UTF8String; break;
                case FieldType.Complex: type = ObjectType.LengthPrefixed; break;
                default: type = ObjectType.Variant; break;
            }

            WriteTag(index, type);
        }


        internal byte[] ToArray()
        {
            byte[] result = new byte[m_ByteCount];
            CopyToArray(result, 0);
            return result;
        }

        internal int CopyToArray(byte[] array, int index)
        {
            int count = m_ByteCount;

            if (array.LongLength - index < count)
            {
                throw new ArgumentException(Resources.ByteArrayTooShort);
            }

            byte* source = (byte*)m_Allocator.GetMemoryPtr(count, 0).ToPointer();

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
        private void WriteBytes(byte* bytes, int length)
        {
            byte* p = (byte*)m_Allocator.GetMemoryPtr(m_ByteCount + length, m_ByteCount).ToPointer();

            while (length-- > 0)
            {
                *p++ = *bytes++;
                m_ByteCount++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b)
        {
            byte* p = (byte*)m_Allocator.GetMemoryPtr(m_ByteCount + 1, m_ByteCount).ToPointer();
            m_ByteCount++;
            *p = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteTag(int index, ObjectType objType)
        {
            if ((index <= 0) || (index > 268435456))
            {
                throw new InvalidFieldIndexException(Resources.InvalidFieldIndex);
            }

            byte* p = (byte*)m_Allocator.GetMemoryPtr(m_ByteCount + 4, m_ByteCount).ToPointer();
            uint tag = (uint)(index << 4 | (int)objType);

            do
            {
                *p++ = (byte)((tag & 0x7Fu) | 0x80u); //'1vvvvvvv'
                m_ByteCount++;
            }
            while ((tag >>= 7) != 0u);

            *(p - 1) &= 0x7F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInt32Variant(uint value)
        {
            byte* p = (byte*)m_Allocator.GetMemoryPtr(m_ByteCount + sizeof(uint) + 1, m_ByteCount).ToPointer() + 1;
            byte byteCount = 0;

            while (value != 0u)
            {
                *p++ = (byte)(value & 0xFF);
                byteCount++;
                value >>= 8;
            }

            *(p - byteCount - 1) = byteCount;
            m_ByteCount += byteCount + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInt64Variant(ulong value)
        {
            byte* p = (byte*)m_Allocator.GetMemoryPtr(m_ByteCount + sizeof(ulong) + 1, m_ByteCount).ToPointer() + 1;
            byte byteCount = 0;

            while (value != 0u)
            {
                *p++ = (byte)(value & 0xFF);
                byteCount++;
                value >>= 8;
            }

            *(p - byteCount - 1) = byteCount;
            m_ByteCount += byteCount + 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Zig(short* p)
        {
            short value = *p;
            *p = (short)(value >> 15 ^ value << 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Zig(int* p)
        {
            int value = *p;
            *p = value >> 31 ^ value << 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Zig(long* p)
        {
            long value = *p;
            *p = value >> 63 ^ value << 1;
        }
    }
}
