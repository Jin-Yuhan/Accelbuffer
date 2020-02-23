#if UNITY
using UnityEngine;
#endif

using Accelbuffer.Unsafe.Text;
using System;
using System.Runtime.CompilerServices;
using Resources = Accelbuffer.Properties.Resources;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对字节数据的读取与转换接口
    /// </summary>
    public unsafe struct AccelReader
    {
        private readonly byte* m_Source;
        private readonly int m_Size;
        private readonly Encoding m_Encoding;
        private readonly bool m_IsLittleEndian;
        private int m_ReadCount;
        private uint m_Tag;

        internal AccelReader(byte* source, int size, Encoding encoding, bool isLittleEndian)
        {
            m_Source = source;
            m_Size = size;
            m_Encoding = encoding;
            m_IsLittleEndian = isLittleEndian;
            m_ReadCount = 0;
            m_Tag = 0u;
        }

        /// <summary>
        /// 获取是否还有下一个值
        /// </summary>
        /// <returns>如果还有值，返回true，否则，false</returns>
        public bool HasNext()
        {
            if (m_ReadCount < m_Size)
            {
                m_Tag = ReadUInt32Variant();
                return true;
            }

            m_Tag = 0u;
            return false;
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
                m_Tag = ReadUInt32Variant();
                index = ReadIndexFromCachedTag();
                return true;
            }

            m_Tag = 0u;
            index = 0;
            return false;
        }

        /// <summary>
        /// 跳过下一个值
        /// </summary>
        /// <exception cref="NotSupportedException">下一个值的类型是非法的</exception>
        public void SkipNext()
        {
            ObjectType type = ReadTypeFromCachedTag();
            int len = ReadLengthByType(type);

            if (len < 0)
            {
                throw new NotSupportedException();
            }

            m_ReadCount += len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public sbyte ReadInt8()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed8)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int8"));
            }

            return (sbyte)ReadByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte ReadUInt8()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed8)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint8"));
            }

            return ReadByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public short ReadInt16()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed16)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int16"));
            }

            short value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 2);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 2);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public ushort ReadUInt16()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed16)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint16"));
            }

            ushort value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 2);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 2);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed32)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int32"));
            }

            int value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 4);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public uint ReadUInt32()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed32)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint32"));
            }

            uint value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 4);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "int64"));
            }

            long value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 8);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 8);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public ulong ReadUInt64()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uint64"));
            }

            ulong value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 8);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 8);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VInt ReadVariantInt()
        {
            ObjectType type = ReadTypeFromCachedTag();
            int len = ReadLengthByType(type);

            if (len < 0)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vint"));
            }

            long value = 0;

            if (BitConverter.IsLittleEndian)
            {
                if (m_IsLittleEndian)
                {
                    ReadBytes((byte*)&value, len);
                }
                else
                {
                    ReadBytesReversed((byte*)&value, len);
                }
            }
            else
            {
                if (m_IsLittleEndian)
                {
                    ReadBytesReversed((byte*)&value, len, 8);
                }
                else
                {
                    ReadBytes(((byte*)&value) + 8 - len, len);
                }
            }

            return Zag(new VInt(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public VUInt ReadVariantUInt()
        {
            ObjectType type = ReadTypeFromCachedTag();
            int len = ReadLengthByType(type);

            if (len < 0)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vuint"));
            }

            ulong value = 0;

            if (BitConverter.IsLittleEndian)
            {
                if (m_IsLittleEndian)
                {
                    ReadBytes((byte*)&value, len);
                }
                else
                {
                    ReadBytesReversed((byte*)&value, len);
                }
            }
            else
            {
                if (m_IsLittleEndian)
                {
                    ReadBytesReversed((byte*)&value, len, 8);
                }
                else
                {
                    ReadBytes(((byte*)&value) + 8 - len, len);
                }
            }

            return new VUInt(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IntPtr ReadIntPtr()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "intptr"));
            }

            long value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 8);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 8);
            }

            return new IntPtr(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public UIntPtr ReadUIntPtr()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "uintptr"));
            }

            ulong value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 8);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 8);
            }

            return new UIntPtr(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ReadFloat32()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed32)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "float32"));
            }

            float value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 4);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ReadFloat64()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "float64"));
            }

            double value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 8);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 8);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal ReadFloat128()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed128)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "float128"));
            }

            decimal value = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 16);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 16);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed8)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "boolean"));
            }

            return ReadByte() == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char ReadChar()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed16)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "char"));
            }

            char value = default;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&value, 2);
            }
            else
            {
                ReadBytesReversed((byte*)&value, 2);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            ObjectType type = ReadTypeFromCachedTag();
            int len = ReadLengthByType(type);

            if (len < 0)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "string"));
            }

            if (m_ReadCount + len > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            byte* src = m_Source + m_ReadCount;
            m_ReadCount += len;

            switch (m_Encoding)
            {
                case Encoding.Unicode:
                    if (m_IsLittleEndian == BitConverter.IsLittleEndian)
                    {
                        return Encodings.Unicode.GetString(src, len);
                    }
                    return Encodings.ReversedUnicode.GetString(src, len);
                case Encoding.ASCII: return Encodings.ASCII.GetString(src, len);
                default: return Encodings.UTF8.GetString(src, len);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadGeneric<T>()
        {
            ITypeSerializer<T> serializer = InternalTypeCache<T>.Serializer;

            if (serializer is IBuiltinTypeSerializer)
            {
                return serializer.Deserialize(ref this);
            }
            else
            {
                ObjectType type = ReadTypeFromCachedTag();
                int len = ReadLengthByType(type);

                if (len < 0)
                {
                    throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, typeof(T).AssemblyQualifiedName));
                }

                if (m_ReadCount + len > m_Size)
                {
                    throw new StreamTooShortException(Resources.StreamTooShort);
                }

                AccelReader iterator = new AccelReader(m_Source + m_ReadCount, len, m_Encoding, m_IsLittleEndian);
                m_ReadCount += len;
                return serializer.Deserialize(ref iterator);
            }
        }


#if UNITY
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Vector2 ReadVector2()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vector2"));
            }

            float x = 0;
            float y = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&x, 4);
                ReadBytes((byte*)&y, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&x, 4);
                ReadBytesReversed((byte*)&y, 4);
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Vector3 ReadVector3()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed96)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vector3"));
            }

            float x = 0;
            float y = 0;
            float z = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&x, 4);
                ReadBytes((byte*)&y, 4);
                ReadBytes((byte*)&z, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&x, 4);
                ReadBytesReversed((byte*)&y, 4);
                ReadBytesReversed((byte*)&z, 4);
            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Vector4 ReadVector4()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed128)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vector4"));
            }

            float x = 0;
            float y = 0;
            float z = 0;
            float w = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&x, 4);
                ReadBytes((byte*)&y, 4);
                ReadBytes((byte*)&z, 4);
                ReadBytes((byte*)&w, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&x, 4);
                ReadBytesReversed((byte*)&y, 4);
                ReadBytesReversed((byte*)&z, 4);
                ReadBytesReversed((byte*)&w, 4);
            }

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Vector2Int ReadVector2Int()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed64)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vector2int"));
            }

            int x = 0;
            int y = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&x, 4);
                ReadBytes((byte*)&y, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&x, 4);
                ReadBytesReversed((byte*)&y, 4);
            }

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Vector3Int ReadVector3Int()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed96)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "vector3int"));
            }

            int x = 0;
            int y = 0;
            int z = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&x, 4);
                ReadBytes((byte*)&y, 4);
                ReadBytes((byte*)&z, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&x, 4);
                ReadBytesReversed((byte*)&y, 4);
                ReadBytesReversed((byte*)&z, 4);
            }

            return new Vector3Int(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Quaternion ReadQuaternion()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed128)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "quaternion"));
            }

            float x = 0;
            float y = 0;
            float z = 0;
            float w = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&x, 4);
                ReadBytes((byte*)&y, 4);
                ReadBytes((byte*)&z, 4);
                ReadBytes((byte*)&w, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&x, 4);
                ReadBytesReversed((byte*)&y, 4);
                ReadBytesReversed((byte*)&z, 4);
                ReadBytesReversed((byte*)&w, 4);
            }

            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Color ReadColor()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed128)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "color"));
            }

            float r = 0;
            float g = 0;
            float b = 0;
            float a = 0;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                ReadBytes((byte*)&r, 4);
                ReadBytes((byte*)&g, 4);
                ReadBytes((byte*)&b, 4);
                ReadBytes((byte*)&a, 4);
            }
            else
            {
                ReadBytesReversed((byte*)&r, 4);
                ReadBytesReversed((byte*)&g, 4);
                ReadBytesReversed((byte*)&b, 4);
                ReadBytesReversed((byte*)&a, 4);
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public Color32 ReadColor32()
        {
            ObjectType type = ReadTypeFromCachedTag();

            if (type != ObjectType.Fixed32)
            {
                throw new InvalidCastException(string.Format(Resources.InvalidCastFormat, type, "color32"));
            }

            return new Color32(ReadByte(), ReadByte(), ReadByte(), ReadByte());
        }
#endif


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
        private void ReadBytesReversed(byte* buffer, int length)
        {
            if (m_ReadCount + length > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            buffer += length - 1;

            while (length-- > 0)
            {
                *buffer-- = *(m_Source + m_ReadCount++);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadBytesReversed(byte* buffer, int length, int rawLength)
        {
            if (m_ReadCount + length > m_Size)
            {
                throw new StreamTooShortException(Resources.StreamTooShort);
            }

            buffer += rawLength - 1;

            while (length-- > 0)
            {
                *buffer-- = *(m_Source + m_ReadCount++);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadUInt32Variant()
        {
            uint value = 0u;
            byte b;
            int count = 0;

            do
            {
                b = ReadByte();
                value |= (uint)((b & 0x7F) << count);
                count += 7;
            } while ((b & 0x80u) == 0x80u);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ReadLengthByType(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Fixed8: return 1;
                case ObjectType.Fixed16: return 2;
                case ObjectType.Fixed24: return 3;
                case ObjectType.Fixed32: return 4;
                case ObjectType.Fixed40: return 5;
                case ObjectType.Fixed48: return 6;
                case ObjectType.Fixed56: return 7;
                case ObjectType.Fixed64: return 8;
                case ObjectType.Fixed72: return 9;
                case ObjectType.Fixed80: return 10;
                case ObjectType.Fixed88: return 11;
                case ObjectType.Fixed96: return 12;
                case ObjectType.Fixed104: return 13;
                case ObjectType.Fixed128: return 16;
                case ObjectType.LengthPrefixed: return (int)ReadUInt32Variant();
                default: return -1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ReadIndexFromCachedTag()
        {
            return (int)(m_Tag >> 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectType ReadTypeFromCachedTag()
        {
            return (ObjectType)(m_Tag & 0xF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static VInt Zag(VInt value)
        {
            return new VInt((long)((ulong)value.m_Value >> 1) ^ -(value.m_Value & 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReadGlobalConfig(byte config, out Encoding encoding, out Endian endian)
        {
            encoding = (Encoding)(config >> 4);
            endian = (Endian)(config & 0xF);
        }
    }
}
