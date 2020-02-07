#if UNITY
using UnityEngine;
#endif

using Accelbuffer.Memory;
using Accelbuffer.Text;
using System;
using System.Runtime.CompilerServices;
using Resources = Accelbuffer.Properties.Resources;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对字节数据的写入接口
    /// </summary>
    public unsafe struct AccelWriter
    {
        private readonly NativeMemory* m_Memory;
        private readonly Encoding m_Encoding;
        private readonly bool m_IsLittleEndian;
        private int m_ByteCount;
        internal int m_Index;//builtin-serializer使用，默认是1

        internal AccelWriter(NativeMemory* memory, Encoding encoding, bool isLittleEndian)
        {
            m_Memory = memory;
            m_Encoding = encoding;
            m_IsLittleEndian = isLittleEndian;
            m_ByteCount = 0;
            m_Index = 1;
        }

        internal void Free()
        {
            m_Memory->Dispose();
        }

        internal void WriteGlobalConfig(Encoding encoding, Endian endian)
        {
            WriteByte((byte)(((int)encoding << 4) | (int)endian));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, sbyte value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed8);
            WriteByte((byte)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, byte value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed8);
            WriteByte(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, short value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed16);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1]);
            }
            else
            {
                WriteByte(bytes[1], bytes[0]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, ushort value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed16);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1]);
            }
            else
            {
                WriteByte(bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, int value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed32);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3]);
            }
            else
            {
                WriteByte(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, uint value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed32);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3]);
            }
            else
            {
                WriteByte(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, long value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed64);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
            }
            else
            {
                WriteByte(bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, ulong value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed64);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
            }
            else
            {
                WriteByte(bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, vint value)
        {
            if (value == default)
            {
                return;
            }

            value = Zig(value);

            byte* p = (byte*)&value;

            if (BitConverter.IsLittleEndian)
            {
                WriteInt64LittleEndian(p, index, value.m_I2);
            }
            else
            {
                WriteInt64BigEndian(p, index, value.m_I1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, vuint value)
        {
            if (value == default)
            {
                return;
            }

            byte* p = (byte*)&value;

            if (BitConverter.IsLittleEndian)
            {
                WriteInt64LittleEndian(p, index, value.m_I2);
            }
            else
            {
                WriteInt64BigEndian(p, index, value.m_I1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, IntPtr value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed64);

            long longValue = value.ToInt64();
            byte* bytes = (byte*)&longValue;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
            }
            else
            {
                WriteByte(bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, UIntPtr value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed64);

            ulong ulongValue = value.ToUInt64();
            byte* bytes = (byte*)&ulongValue;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
            }
            else
            {
                WriteByte(bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, float value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed32);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3]);
            }
            else
            {
                WriteByte(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, double value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed64);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
            }
            else
            {
                WriteByte(bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, decimal value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed128);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7], bytes[8], bytes[9], bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15]);
            }
            else
            {
                WriteByte(bytes[15], bytes[14], bytes[13], bytes[12], bytes[11], bytes[10], bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, bool value)
        {
            if (!value)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed8);
            WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, char value)
        {
            if (value == default)
            {
                return;
            }

            WriteTag(index, ObjectType.Fixed16);

            byte* bytes = (byte*)&value;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytes[0], bytes[1]);
            }
            else
            {
                WriteByte(bytes[1], bytes[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, string value)
        {
            switch (m_Encoding)
            {
                case Encoding.Unicode: WriteUnicodeString(index, value); break;
                case Encoding.ASCII: WriteASCIIString(index, value); break;
                default: WriteUTF8String(index, value); break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue<T>(int index, T value)
        {
            ITypeSerializer<T> serializer = InternalTypeCache<T>.Serializer;

            if (serializer is IBuiltinTypeSerializer)
            {
                m_Index = index;
                serializer.Serialize(value, ref this);
            }
            else
            {
                int memSize = InternalTypeCache<T>.ApproximateMemorySize;
                NativeMemory mem = NativeMemory.Allocate(ref memSize);
                AccelWriter writer = new AccelWriter(&mem, m_Encoding, m_IsLittleEndian);

                try
                {
                    serializer.Serialize(value, ref writer);

                    int byteCount = writer.m_ByteCount;
                    ObjectType type = GetObjectTypeByLength(byteCount);

                    WriteTag(index, type);

                    if (type == ObjectType.LengthPrefixed)
                    {
                        WriteUInt32Variant((uint)byteCount);
                    }

                    WriteBytes(writer.m_Memory->GetPointer(), byteCount);
                }
                finally
                {
                    writer.Free();
                }
            }
        }

#if UNITY
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Vector2 value)
        {
            WriteTag(index, ObjectType.Fixed64);

            float x = value.x;
            float y = value.y;

            byte* bytesX = (byte*)&x;
            byte* bytesY = (byte*)&y;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesX[0], bytesX[1], bytesX[2], bytesX[3]);
                WriteByte(bytesY[0], bytesY[1], bytesY[2], bytesY[3]);
            }
            else
            {
                WriteByte(bytesX[3], bytesX[2], bytesX[1], bytesX[0]);
                WriteByte(bytesY[3], bytesY[2], bytesY[1], bytesY[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Vector3 value)
        {
            WriteTag(index, ObjectType.Fixed96);

            float x = value.x;
            float y = value.y;
            float z = value.z;

            byte* bytesX = (byte*)&x;
            byte* bytesY = (byte*)&y;
            byte* bytesZ = (byte*)&z;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesX[0], bytesX[1], bytesX[2], bytesX[3]);
                WriteByte(bytesY[0], bytesY[1], bytesY[2], bytesY[3]);
                WriteByte(bytesZ[0], bytesZ[1], bytesZ[2], bytesZ[3]);
            }
            else
            {
                WriteByte(bytesX[3], bytesX[2], bytesX[1], bytesX[0]);
                WriteByte(bytesY[3], bytesY[2], bytesY[1], bytesY[0]);
                WriteByte(bytesZ[3], bytesZ[2], bytesZ[1], bytesZ[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Vector4 value)
        {
            WriteTag(index, ObjectType.Fixed128);

            float x = value.x;
            float y = value.y;
            float z = value.z;
            float w = value.w;

            byte* bytesX = (byte*)&x;
            byte* bytesY = (byte*)&y;
            byte* bytesZ = (byte*)&z;
            byte* bytesW = (byte*)&w;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesX[0], bytesX[1], bytesX[2], bytesX[3]);
                WriteByte(bytesY[0], bytesY[1], bytesY[2], bytesY[3]);
                WriteByte(bytesZ[0], bytesZ[1], bytesZ[2], bytesZ[3]);
                WriteByte(bytesW[0], bytesW[1], bytesW[2], bytesW[3]);
            }
            else
            {
                WriteByte(bytesX[3], bytesX[2], bytesX[1], bytesX[0]);
                WriteByte(bytesY[3], bytesY[2], bytesY[1], bytesY[0]);
                WriteByte(bytesZ[3], bytesZ[2], bytesZ[1], bytesZ[0]);
                WriteByte(bytesW[3], bytesW[2], bytesW[1], bytesW[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Vector2Int value)
        {
            WriteTag(index, ObjectType.Fixed64);

            int x = value.x;
            int y = value.y;

            byte* bytesX = (byte*)&x;
            byte* bytesY = (byte*)&y;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesX[0], bytesX[1], bytesX[2], bytesX[3]);
                WriteByte(bytesY[0], bytesY[1], bytesY[2], bytesY[3]);
            }
            else
            {
                WriteByte(bytesX[3], bytesX[2], bytesX[1], bytesX[0]);
                WriteByte(bytesY[3], bytesY[2], bytesY[1], bytesY[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Vector3Int value)
        {
            WriteTag(index, ObjectType.Fixed96);

            int x = value.x;
            int y = value.y;
            int z = value.z;

            byte* bytesX = (byte*)&x;
            byte* bytesY = (byte*)&y;
            byte* bytesZ = (byte*)&z;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesX[0], bytesX[1], bytesX[2], bytesX[3]);
                WriteByte(bytesY[0], bytesY[1], bytesY[2], bytesY[3]);
                WriteByte(bytesZ[0], bytesZ[1], bytesZ[2], bytesZ[3]);
            }
            else
            {
                WriteByte(bytesX[3], bytesX[2], bytesX[1], bytesX[0]);
                WriteByte(bytesY[3], bytesY[2], bytesY[1], bytesY[0]);
                WriteByte(bytesZ[3], bytesZ[2], bytesZ[1], bytesZ[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Quaternion value)
        {
            WriteTag(index, ObjectType.Fixed128);

            float x = value.x;
            float y = value.y;
            float z = value.z;
            float w = value.w;

            byte* bytesX = (byte*)&x;
            byte* bytesY = (byte*)&y;
            byte* bytesZ = (byte*)&z;
            byte* bytesW = (byte*)&w;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesX[0], bytesX[1], bytesX[2], bytesX[3]);
                WriteByte(bytesY[0], bytesY[1], bytesY[2], bytesY[3]);
                WriteByte(bytesZ[0], bytesZ[1], bytesZ[2], bytesZ[3]);
                WriteByte(bytesW[0], bytesW[1], bytesW[2], bytesW[3]);
            }
            else
            {
                WriteByte(bytesX[3], bytesX[2], bytesX[1], bytesX[0]);
                WriteByte(bytesY[3], bytesY[2], bytesY[1], bytesY[0]);
                WriteByte(bytesZ[3], bytesZ[2], bytesZ[1], bytesZ[0]);
                WriteByte(bytesW[3], bytesW[2], bytesW[1], bytesW[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void WriteValue(int index, Color value)
        {
            WriteTag(index, ObjectType.Fixed128);

            float r = value.r;
            float g = value.g;
            float b = value.b;
            float a = value.a;

            byte* bytesR = (byte*)&r;
            byte* bytesG = (byte*)&g;
            byte* bytesB = (byte*)&b;
            byte* bytesA = (byte*)&a;

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                WriteByte(bytesR[0], bytesR[1], bytesR[2], bytesR[3]);
                WriteByte(bytesG[0], bytesG[1], bytesG[2], bytesG[3]);
                WriteByte(bytesB[0], bytesB[1], bytesB[2], bytesB[3]);
                WriteByte(bytesA[0], bytesA[1], bytesA[2], bytesA[3]);
            }
            else
            {
                WriteByte(bytesR[3], bytesR[2], bytesR[1], bytesR[0]);
                WriteByte(bytesG[3], bytesG[2], bytesG[1], bytesG[0]);
                WriteByte(bytesB[3], bytesB[2], bytesB[1], bytesB[0]);
                WriteByte(bytesA[3], bytesA[2], bytesA[1], bytesA[0]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="color"></param>
        public void WriteValue(int index, Color32 color)
        {
            WriteTag(index, ObjectType.Fixed32);

            WriteByte(color.r);
            WriteByte(color.g);
            WriteByte(color.b);
            WriteByte(color.a);
        }
#endif


        internal NativeBuffer ToNativeBufferNoCopy()
        {
            return new NativeBuffer(m_Memory->GetPointer(0), m_ByteCount, m_Memory->Size);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b)
        {
            byte* p = Allocate(1);
            m_ByteCount++;
            *p = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b1, byte b2)
        {
            byte* p = Allocate(2);
            m_ByteCount += 2;

            p[0] = b1;
            p[1] = b2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b1, byte b2, byte b3, byte b4)
        {
            byte* p = Allocate(4);
            m_ByteCount += 4;

            p[0] = b1;
            p[1] = b2;
            p[2] = b3;
            p[3] = b4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8)
        {
            byte* p = Allocate(8);
            m_ByteCount += 8;

            p[0] = b1;
            p[1] = b2;
            p[2] = b3;
            p[3] = b4;
            p[4] = b5;
            p[5] = b6;
            p[6] = b7;
            p[7] = b8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11, byte b12, byte b13, byte b14, byte b15, byte b16)
        {
            byte* p = Allocate(16);
            m_ByteCount += 16;

            p[0] = b1;
            p[1] = b2;
            p[2] = b3;
            p[3] = b4;
            p[4] = b5;
            p[5] = b6;
            p[6] = b7;
            p[7] = b8;
            p[8] = b9;
            p[9] = b10;
            p[10] = b11;
            p[11] = b12;
            p[12] = b13;
            p[13] = b14;
            p[14] = b15;
            p[15] = b16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteBytes(byte* bytes, int length)
        {
            byte* p = Allocate(length);
            m_ByteCount += length;

            while (length-- > 0)
            {
                *p++ = *bytes++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteBytesReversed(byte* bytes, int length)
        {
            byte* p = Allocate(length);
            m_ByteCount += length;
            bytes += length - 1;

            while (length-- > 0)
            {
                *p++ = *bytes--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteTag(int index, ObjectType objType)
        {
            if ((index <= 0) || (index > 268435456))
            {
                throw new InvalidFieldIndexException(Resources.InvalidFieldIndex);
            }

            uint tag = (uint)(index << 4 | (int)objType);
            WriteUInt32Variant(tag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectType GetObjectTypeByLength(int length)
        {
            ObjectType type;

            switch (length)
            {
                case 0: type = ObjectType.LengthPrefixed; break;
                case 1: type = ObjectType.Fixed8; break;
                case 2: type = ObjectType.Fixed16; break;
                case 3: type = ObjectType.Fixed24; break;
                case 4: type = ObjectType.Fixed32; break;
                case 5: type = ObjectType.Fixed40; break;
                case 6: type = ObjectType.Fixed48; break;
                case 7: type = ObjectType.Fixed56; break;
                case 8: type = ObjectType.Fixed64; break;
                case 9: type = ObjectType.Fixed72; break;
                case 10: type = ObjectType.Fixed80; break;
                case 11: type = ObjectType.Fixed88; break;
                case 12: type = ObjectType.Fixed96; break;
                case 13: type = ObjectType.Fixed104; break;
                case 14: type = ObjectType.LengthPrefixed; break;//防止生成IL代码是if-else
                case 15: type = ObjectType.LengthPrefixed; break;
                case 16: type = ObjectType.Fixed128; break;
                default: type = ObjectType.LengthPrefixed; break;
            }

            return type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUInt32Variant(uint value)
        {
            byte* p = Allocate(5);

            do
            {
                *p++ = (byte)((value & 0x7Fu) | 0x80u); //'1vvvvvvv'
                m_ByteCount++;
            }
            while ((value >>= 7) != 0u);

            *(p - 1) &= 0x7F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInt64LittleEndian(byte* p, int index, uint i2)
        {
            //system使用little-endian

            int byteCount = 8;

            if (i2 == 0u)
            {
                byteCount = 4;//后4个字节都是0
            }

            while (byteCount > 0)
            {
                if (p[byteCount - 1] != 0)
                {
                    break;
                }

                byteCount--;
            }

            ObjectType type = (ObjectType)byteCount;
            WriteTag(index, type);

            if (m_IsLittleEndian)
            {
                WriteBytes(p, byteCount);
            }
            else
            {
                WriteBytesReversed(p, byteCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteInt64BigEndian(byte* p, int index, uint i1)
        {
            //system使用big-endian

            int byteCount = 8;

            if (i1 == 0u)
            {
                byteCount = 4;//前4个字节都是0
            }

            int cnt = byteCount;

            for (int i = 0; i < cnt; i++)
            {
                if (p[i] != 0)
                {
                    break;
                }

                byteCount--;
            }

            p += cnt - byteCount;

            ObjectType type = (ObjectType)byteCount;
            WriteTag(index, type);

            if (m_IsLittleEndian)
            {
                WriteBytesReversed(p, byteCount);
            }
            else
            {
                WriteBytes(p, byteCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUnicodeString(int index, string value)
        {
            int byteCount = value.Length << 1;
            ObjectType type = GetObjectTypeByLength(byteCount);

            WriteTag(index, type);

            if (type == ObjectType.LengthPrefixed)
            {
                WriteUInt32Variant((uint)byteCount);
            }

            byte* p = Allocate(byteCount);

            if (m_IsLittleEndian == BitConverter.IsLittleEndian)
            {
                Encodings.Unicode.GetBytes(value, p);
            }
            else
            {
                Encodings.ReversedUnicode.GetBytes(value, p);
            }

            m_ByteCount += byteCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteASCIIString(int index, string value)
        {
            IUnsafeEncoding e = Encodings.ASCII;
            ObjectType type = GetObjectTypeByLength(value.Length);

            WriteTag(index, type);

            if (type == ObjectType.LengthPrefixed)
            {
                WriteUInt32Variant((uint)value.Length);
            }

            byte* p = Allocate(value.Length);

            e.GetBytes(value, p);
            m_ByteCount += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUTF8String(int index, string value)
        {
            IUnsafeEncoding e = Encodings.UTF8;
            int byteCount = value.Length * 3;

            byte* buffer = stackalloc byte[byteCount];
            byteCount = e.GetBytes(value, buffer);
            ObjectType type = GetObjectTypeByLength(byteCount);

            WriteTag(index, type);

            if (type == ObjectType.LengthPrefixed)
            {
                WriteUInt32Variant((uint)byteCount);
            }

            WriteBytes(buffer, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte* Allocate(int minSize)
        {
            return m_Memory->GetPointer(minSize + m_ByteCount) + m_ByteCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static vint Zig(vint value)
        {
            return new vint(value.m_Value >> 63 ^ value.m_Value << 1);
        }
    }
}
