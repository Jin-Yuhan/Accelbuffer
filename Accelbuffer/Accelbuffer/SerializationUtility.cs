using System;
using System.Runtime.CompilerServices;

namespace Accelbuffer
{
    internal static unsafe class SerializationUtility 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumberSign GetSign(byte* value)
        {
            return (NumberSign)((*value) >> 7 & 0x1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OnesComplement(sbyte* value)
        {
            *value = (sbyte)~*value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OnesComplement(short* value)
        {
            *value = (short)~*value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OnesComplement(int* value)
        {
            *value = ~*value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OnesComplement(long* value)
        {
            *value = ~*value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OnesComplement(byte* value, int size)
        {
            switch (size)
            {
                case 1: OnesComplement((sbyte*)value); break;
                case 2: OnesComplement((short*)value); break;
                case 4: OnesComplement((int*)value); break;
                case 8: OnesComplement((long*)value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUsedByteCount(byte* p, int size)
        {
            p += size - 1;

            while (size > 0)
            {
                if (*p-- != 0)
                {
                    break;
                }

                size--;
            }

            return size;
        }

        public static SerializedType GetSerializedType(Type type, out string name)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                    name = "Int8";
                    return SerializedType.Number;
                case TypeCode.Byte:
                    name = "UInt8";
                    return SerializedType.Number;
                case TypeCode.Int16:
                    name = "Int16";
                    return SerializedType.Number;
                case TypeCode.UInt16:
                    name = "UInt16";
                    return SerializedType.Number;
                case TypeCode.Int32:
                    name = "Int32";
                    return SerializedType.Number;
                case TypeCode.UInt32:
                    name = "UInt32";
                    return SerializedType.Number;
                case TypeCode.Int64:
                    name = "Int64";
                    return SerializedType.Number;
                case TypeCode.UInt64:
                    name = "UInt64";
                    return SerializedType.Number;
                case TypeCode.Single:
                    name = "Float32";
                    return SerializedType.Number;
                case TypeCode.Double:
                    name = "Float64";
                    return SerializedType.Number;
                case TypeCode.Boolean:
                    name = "Boolean";
                    return SerializedType.Boolean;
                case TypeCode.Char:
                    name = "Char";
                    return SerializedType.Char;
                case TypeCode.String:
                    name = "String";
                    return SerializedType.Char;
                default:
                    name = string.Empty;
                    return SerializedType.Complex;
            }
        }

        /// <summary>
        /// 获取是否真正是复杂类型（不包括简单类型组成的一维数组）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTrulyComplex(Type type)
        {
            if (type.IsArray)
            {
                return type.GetArrayRank() > 1 || GetSerializedType(type.GetElementType(), out _) == SerializedType.Complex;
            }

            return GetSerializedType(type, out _) == SerializedType.Complex;
        }
    }
}
