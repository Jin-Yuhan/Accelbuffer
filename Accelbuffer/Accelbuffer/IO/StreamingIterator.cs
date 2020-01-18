using Accelbuffer.Properties;
using Accelbuffer.Injection;
using Accelbuffer.Text;
using System;
using System.Runtime.CompilerServices;

namespace Accelbuffer
{
    /// <summary>
    /// 提供对流数据的迭代支持
    /// </summary>
    public unsafe ref struct StreamingIterator
    {
        private readonly byte* m_Source;
        private readonly int m_Size;
        private int m_ReadCount;
        private uint m_Tag;

        internal StreamingIterator(byte* source, int size)
        {
            m_Source = source;
            m_Size = size;
            m_ReadCount = 0;
            m_Tag = 0u;
        }

        /// <summary>
        /// 将迭代器的指针移动指定字节
        /// </summary>
        /// <param name="byteCount">移动字节数量，这个值是一维矢量</param>
        
        public void Move(int byteCount = -1)
        {
            m_ReadCount += byteCount;
        }

        /// <summary>
        /// 获取是否还有下一个值
        /// </summary>
        /// <returns>如果还有值，返回true，否则，false</returns>
        public bool HasNext()
        {
            return m_ReadCount < m_Size;
        }

        /// <summary>
        /// 获取是否还有下一个值
        /// </summary>
        /// <param name="index">下一个值的索引</param>
        /// <returns>如果还有值，返回true，否则，false</returns>
        public bool HasNext(out int index)
        {
            if (m_ReadCount < m_Size)
            {
                ReadTag();
                index = GetIndexFromCachedTag();
                return true;
            }

            index = 0;
            return false;
        }

        /// <summary>
        /// 跳过下一个值
        /// </summary>
        public void SkipNext()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.ASCIIChar:
                case ObjectType.Fixed8:         Move(1);                                            break;
                case ObjectType.UnicodeChar:
                case ObjectType.Fixed16:        Move(2);                                            break;
                case ObjectType.Fixed32:        Move(4);                                            break;
                case ObjectType.Fixed64:        Move(8);                                            break;
                case ObjectType.Fixed128:       Move(16);                                           break;
                case ObjectType.Variant:        Move(ReadByte());                                   break;
                case ObjectType.ASCIIString:
                case ObjectType.UnicodeString:
                case ObjectType.UTF8String:
                case ObjectType.LengthPrefixed: Move(NextAsInt32WithoutTag(NumberFormat.Variant));  break;
                case ObjectType.UTF8Char:       NextAsCharWithoutTag(Encoding.UTF8);                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sbyte NextAsInt8()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsInt8WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed8: return NextAsInt8WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int8"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte NextAsUInt8()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsUInt8WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed8: return NextAsUInt8WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint8"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public short NextAsInt16()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsInt16WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed16: return NextAsInt16WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int16"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ushort NextAsUInt16()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsUInt16WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed16: return NextAsUInt16WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint16"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int NextAsInt32()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsInt32WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed32: return NextAsInt32WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int32"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint NextAsUInt32()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsUInt32WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed32: return NextAsUInt32WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint32"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long NextAsInt64()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsInt64WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed64: return NextAsInt64WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int64"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ulong NextAsUInt64()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.Variant: return NextAsUInt64WithoutTag(NumberFormat.Variant);
                case ObjectType.Fixed64: return NextAsUInt64WithoutTag(NumberFormat.Fixed);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint64"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float NextAsFloat32()
        {
            ObjectType type = GetTypeFromCachedTag();

            if (type != ObjectType.Fixed32)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "float32"));
            }

            return NextAsFloat32WithoutTag();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double NextAsFloat64()
        {
            ObjectType type = GetTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "float64"));
            }

            return NextAsFloat64WithoutTag();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal NextAsFloat128()
        {
            ObjectType type = GetTypeFromCachedTag();

            if (type != ObjectType.Fixed128)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "float128"));
            }

            return NextAsFloat128WithoutTag();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool NextAsBoolean()
        {
            ObjectType type = GetTypeFromCachedTag();

            if (type != ObjectType.Fixed8)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "boolean"));
            }

            return NextAsBooleanWithoutTag();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char NextAsChar()
        {
            ObjectType type = GetTypeFromCachedTag();

            switch (type)
            {
                case ObjectType.ASCIIChar: return NextAsCharWithoutTag(Encoding.ASCII);
                case ObjectType.UnicodeChar: return NextAsCharWithoutTag(Encoding.Unicode);
                case ObjectType.UTF8Char: return NextAsCharWithoutTag(Encoding.UTF8);
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "char"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string NextAsString()
        {
            ObjectType type = GetTypeFromCachedTag();
            Encoding encoding;

            switch (type)
            {
                case ObjectType.ASCIIString: encoding = Encoding.ASCII; break;
                case ObjectType.UnicodeString: encoding = Encoding.Unicode; break;
                case ObjectType.UTF8String: encoding = Encoding.UTF8; break;
                default: throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "string"));
            }

            return NextAsStringWithoutTag(encoding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NextAs<T>()
        {
            ObjectType type = GetTypeFromCachedTag();

            if (type != ObjectType.LengthPrefixed)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, typeof(T).AssemblyQualifiedName));
            }

            return NextAsWithoutTag<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public sbyte NextAsInt8WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                return (sbyte)ReadInt32Variant();
            }
            else
            {
                sbyte value = 0;
                ReadBytes((byte*)&value, sizeof(sbyte));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public byte NextAsUInt8WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                return (byte)ReadInt32Variant();
            }
            else
            {
                byte value = 0;
                ReadBytes(&value, sizeof(byte));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public short NextAsInt16WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                short value = (short)ReadInt32Variant();
                Zag(&value);
                return value;
            }
            else
            {
                short value = 0;
                ReadBytes((byte*)&value, sizeof(short));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public ushort NextAsUInt16WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                ushort value = (ushort)ReadInt32Variant();
                return value;
            }
            else
            {
                ushort value = 0;
                ReadBytes((byte*)&value, sizeof(ushort));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public int NextAsInt32WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                int value = (int)ReadInt32Variant();
                Zag(&value);
                return value;
            }
            else
            {
                int value = 0;
                ReadBytes((byte*)&value, sizeof(int));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public uint NextAsUInt32WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                uint value = ReadInt32Variant();
                return value;
            }
            else
            {
                uint value = 0;
                ReadBytes((byte*)&value, sizeof(uint));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public long NextAsInt64WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                long value = (long)ReadInt64Variant();
                Zag(&value);
                return value;
            }
            else
            {
                long value = 0;
                ReadBytes((byte*)&value, sizeof(long));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public ulong NextAsUInt64WithoutTag(NumberFormat format)
        {
            if (format == NumberFormat.Variant)
            {
                ulong value = ReadInt64Variant();
                return value;
            }
            else
            {
                ulong value = 0;
                ReadBytes((byte*)&value, sizeof(ulong));
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float NextAsFloat32WithoutTag()
        {
            float value = 0;
            ReadBytes((byte*)&value, sizeof(float));
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double NextAsFloat64WithoutTag()
        {
            double value = 0;
            ReadBytes((byte*)&value, sizeof(double));
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal NextAsFloat128WithoutTag()
        {
            decimal value = 0;
            ReadBytes((byte*)&value, sizeof(decimal));
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool NextAsBooleanWithoutTag()
        {
            return ReadByte() == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public char NextAsCharWithoutTag(Encoding encoding)
        {
            ITextEncoding e = Encodings.GetEncoding(encoding);
            char value = e.GetChar(m_Source + m_ReadCount, out int readCount);

            if (m_ReadCount + readCount > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            m_ReadCount += readCount;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string NextAsStringWithoutTag(Encoding encoding)
        {
            int len = NextAsInt32WithoutTag(NumberFormat.Variant);

            if (m_ReadCount + len > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            ITextEncoding e = Encodings.GetEncoding(encoding);
            string value = e.GetString(m_Source + m_ReadCount, len);
            m_ReadCount += len;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NextAsWithoutTag<T>()
        {
            int byteCount = NextAsInt32WithoutTag(NumberFormat.Variant);

            if (m_ReadCount + byteCount > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            StreamingIterator iterator = new StreamingIterator(m_Source + m_ReadCount, byteCount);
            m_ReadCount += byteCount;
            return SerializerInjector.InternalCache<T>.Serializer.Deserialize(ref iterator);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ReadByte()
        {
            if (m_ReadCount + 1 > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            return m_Source[m_ReadCount++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadBytes(byte* buffer, int length)
        {
            if (m_ReadCount + length > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            while (length-- > 0)
            {
                *buffer++ = *(m_Source + m_ReadCount++);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadInt32Variant()
        {
            byte byteCount = ReadByte();

            if (m_ReadCount + byteCount > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            uint value = 0u;
            int count = 0;

            while (byteCount-- > 0)
            {
                value |= (uint)(m_Source[m_ReadCount++] << count);
                count += 8;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong ReadInt64Variant()
        {
            byte byteCount = ReadByte();

            if (m_ReadCount + byteCount > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            ulong value = 0u;
            int count = 0;

            while (byteCount-- > 0)
            {
                value |= (uint)(m_Source[m_ReadCount++] << count);
                count += 8;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadTag()
        {
            uint tag = 0u;
            byte b;
            int count = 0;

            do
            {
                b = ReadByte();
                tag |= (uint)((b & 0x7F) << count);
                count += 7;
            } while ((b & 0x80u) == 0x80u);

            m_Tag = tag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetIndexFromCachedTag()
        {
            return (int)(m_Tag >> 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectType GetTypeFromCachedTag()
        {
            return (ObjectType)(m_Tag & 0xF);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Zag(short* p)
        {
            short value = *p;
            *p = (short)((short)((ushort)value >> 1) ^ -(value & 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Zag(int* p)
        {
            int value = *p;
            *p = (int)((uint)value >> 1) ^ -(value & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Zag(long* p)
        {
            long value = *p;
            *p = (long)((ulong)value >> 1) ^ -(value & 1);
        }
    }
}
