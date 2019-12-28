using System.Runtime.CompilerServices;

namespace Accelbuffer
{
    /// <summary>
    /// 提供对数据标签创建和读取的接口
    /// </summary>
    internal static unsafe class TagUtility
    {
        // tags (1 byte)

        //boolean          tag; [type_code(3b) + value(1b) + reserved(4b)]

        //char             tag; [type_code(3b) + char_type(1b) + is_default_value(1b) + char_encoding(2b) + is_empty_string(1b)]

        //variable float   tag; [type_code(3b) + number_sign(1b) + byte_count(4b)]

        //fixed float      tag; [type_code(3b) + byte_count(4b) + reserved(1b)]

        //variable integer tag; [type_code(3b) + number_sign(1b) + byte_count(4b)]

        //fixed integer    tag; [type_code(3b) + byte_count(4b) + reserved(1b)]


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeBooleanTag(bool value)
        {
            return (byte)(((int)ValueTypeCode.Boolean << 5) | ((value ? 1 : 0) << 4));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeCharTag(char value, CharEncoding encoding, out bool isDefaultValue)
        {
            isDefaultValue = value == default;
            return (byte)(((int)ValueTypeCode.Char << 5)
                          | ((int)CharType.SingleChar << 4)
                          | ((isDefaultValue ? 1 : 0) << 3)
                          | ((int)encoding << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeCharTag(string value, CharEncoding encoding, out bool isDefaultValue, out bool isEmpty)
        {
            isDefaultValue = value == default;
            isEmpty = value == string.Empty;
            return (byte)(((int)ValueTypeCode.Char << 5)
                          | ((int)CharType.String << 4)
                          | ((isDefaultValue ? 1 : 0) << 3)
                          | ((int)encoding << 1)
                          | (isEmpty ? 1 : 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableFloatTag(float* value, out int byteCount)
        {
            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 4);
            return (byte)(((int)ValueTypeCode.VariableFloat << 5) | ((int)NumberSign.PositiveOrUnsigned << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableFloatTag(double* value, out int byteCount)
        {
            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 8);
            return (byte)(((int)ValueTypeCode.VariableFloat << 5) | ((int)NumberSign.PositiveOrUnsigned << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedFloatTag(float value, out int byteCount)
        {
            byteCount = value == default ? 0 : 4;
            return (byte)(((int)ValueTypeCode.FixedFloat << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedFloatTag(double value, out int byteCount)
        {
            byteCount = value == default ? 0 : 8;
            return (byte)(((int)ValueTypeCode.FixedFloat << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(sbyte* value, out int byteCount)
        {
            NumberSign sgn = SerializationUtility.GetSign((byte*)value);

            if (sgn == NumberSign.Negative)
            {
                SerializationUtility.OnesComplement(value);
            }

            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 1);

            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)sgn << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(byte* value, out int byteCount)
        {
            byteCount = SerializationUtility.GetUsedByteCount(value, 1);
            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)NumberSign.PositiveOrUnsigned << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(short* value, out int byteCount)
        {
            NumberSign sgn = SerializationUtility.GetSign((byte*)value);

            if (sgn == NumberSign.Negative)
            {
                SerializationUtility.OnesComplement(value);
            }

            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 2);

            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)sgn << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(ushort* value, out int byteCount)
        {
            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 2);
            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)NumberSign.PositiveOrUnsigned << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(int* value, out int byteCount)
        {
            NumberSign sgn = SerializationUtility.GetSign((byte*)value);

            if (sgn == NumberSign.Negative)
            {
                SerializationUtility.OnesComplement(value);
            }

            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 4);

            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)sgn << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(uint* value, out int byteCount)
        {
            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 4);
            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)NumberSign.PositiveOrUnsigned << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(long* value, out int byteCount)
        {
            NumberSign sgn = SerializationUtility.GetSign((byte*)value);

            if (sgn == NumberSign.Negative)
            {
                SerializationUtility.OnesComplement(value);
            }

            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 8);

            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)sgn << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeVariableIntegerTag(ulong* value, out int byteCount)
        {
            byteCount = SerializationUtility.GetUsedByteCount((byte*)value, 8);
            return (byte)(((int)ValueTypeCode.VariableInteger << 5) | ((int)NumberSign.PositiveOrUnsigned << 4) | byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(sbyte value, out int byteCount)
        {
            byteCount = value == default ? 0 : 1;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(byte value, out int byteCount)
        {
            byteCount = value == default ? 0 : 1;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(short value, out int byteCount)
        {
            byteCount = value == default ? 0 : 2;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(ushort value, out int byteCount)
        {
            byteCount = value == default ? 0 : 2;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(int value, out int byteCount)
        {
            byteCount = value == default ? 0 : 4;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(uint value, out int byteCount)
        {
            byteCount = value == default ? 0 : 4;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(long value, out int byteCount)
        {
            byteCount = value == default ? 0 : 8;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte MakeFixedIntegerTag(ulong value, out int byteCount)
        {
            byteCount = value == default ? 0 : 8;
            return (byte)(((int)ValueTypeCode.FixedInteger << 5) | (byteCount << 1));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBooleanTag(byte tag, out ValueTypeCode typeCode, out bool value)
        {
            typeCode = (ValueTypeCode)(tag >> 5);
            value = ((tag >> 4) & 0x1) == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToCharTag(byte tag, out ValueTypeCode typeCode, out CharType valueType, out bool isDefaultValue, out CharEncoding encoding, out bool isEmptyString)
        {
            typeCode = (ValueTypeCode)(tag >> 5);
            valueType = (CharType)((tag >> 4) & 0x1);
            isDefaultValue = ((tag >> 3) & 0x1) == 1;
            encoding = (CharEncoding)((tag >> 1) & 0x3);
            isEmptyString = (tag & 0x1) == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToVariableNumberTag(byte tag, out ValueTypeCode typeCode, out NumberSign sign, out int byteCount)
        {
            typeCode = (ValueTypeCode)(tag >> 5);
            sign = (NumberSign)((tag >> 4) & 0x1);
            byteCount = tag & 0xF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToFixedNumberTag(byte tag, out ValueTypeCode typeCode, out int byteCount)
        {
            typeCode = (ValueTypeCode)(tag >> 5);
            byteCount = (tag >> 1) & 0xF;
        }
    }
}
