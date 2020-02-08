using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Accelbuffer
{
#pragma warning disable IDE1006
    /// <summary>
    /// 表示一个动态长度的有符号整数
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{m_Value}")]
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct VInt : IComparable, IFormattable, IConvertible, IComparable<VInt>, IEquatable<VInt>, ISerializable
    {
#pragma warning disable CS1591
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly VInt MaxValue = long.MaxValue;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly VInt MinValue = long.MinValue;

#if UNITY
        [UnityEngine.SerializeField]
#endif

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if UNITY
        internal long m_Value;
#else
        internal readonly long m_Value;
#endif

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I1;

        [FieldOffset(4)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I2;

        public VInt(sbyte value) : this() => m_Value = value;

        public VInt(byte value) : this() => m_Value = value;

        public VInt(short value) : this() => m_Value = value;

        public VInt(ushort value) : this() => m_Value = value;

        public VInt(int value) : this() => m_Value = value;

        public VInt(uint value) : this() => m_Value = value;

        public VInt(long value) : this() => m_Value = value;

        public VInt(ulong value) : this() => m_Value = (long)value;

        public VInt(SerializationInfo info, StreamingContext context) : this() => m_Value = info.GetInt64("value");

        public int CompareTo(VInt other) => m_Value.CompareTo(other.m_Value);

        public bool Equals(VInt other) => m_Value.Equals(other.m_Value);

        public override bool Equals(object obj) => obj is VInt v && Equals(v);

        public override int GetHashCode() => m_Value.GetHashCode();

        public override string ToString() => m_Value.ToString();

        public string ToString(string format) => m_Value.ToString(format);

        public string ToString(IFormatProvider provider) => m_Value.ToString(provider);

        public string ToString(string format, IFormatProvider provider) => m_Value.ToString(format, provider);

        public int CompareTo(object obj) => m_Value.CompareTo(obj);

        public static VInt Parse(string s, NumberStyles style, IFormatProvider provider) => long.Parse(s, style, provider);

        public static VInt Parse(string s, IFormatProvider provider) => long.Parse(s, provider);

        public static VInt Parse(string s) => long.Parse(s);

        public static VInt Parse(string s, NumberStyles style) => long.Parse(s, style);

        public static bool TryParse(string s, out VInt result)
        {
            bool u = long.TryParse(s, out long l);
            result = new VInt(l);
            return u;
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out VInt result)
        {
            bool u = long.TryParse(s, style, provider, out long l);
            result = new VInt(l);
            return u;
        }

        public TypeCode GetTypeCode() => m_Value.GetTypeCode();

        bool IConvertible.ToBoolean(IFormatProvider provider) => ((IConvertible)m_Value).ToBoolean(provider);

        char IConvertible.ToChar(IFormatProvider provider) => ((IConvertible)m_Value).ToChar(provider);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => ((IConvertible)m_Value).ToSByte(provider);

        byte IConvertible.ToByte(IFormatProvider provider) => ((IConvertible)m_Value).ToByte(provider);

        short IConvertible.ToInt16(IFormatProvider provider) => ((IConvertible)m_Value).ToInt16(provider);

        ushort IConvertible.ToUInt16(IFormatProvider provider) => ((IConvertible)m_Value).ToUInt16(provider);

        int IConvertible.ToInt32(IFormatProvider provider) => ((IConvertible)m_Value).ToInt32(provider);

        uint IConvertible.ToUInt32(IFormatProvider provider) => ((IConvertible)m_Value).ToUInt32(provider);

        long IConvertible.ToInt64(IFormatProvider provider) => ((IConvertible)m_Value).ToInt64(provider);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => ((IConvertible)m_Value).ToUInt64(provider);

        float IConvertible.ToSingle(IFormatProvider provider) => ((IConvertible)m_Value).ToSingle(provider);

        double IConvertible.ToDouble(IFormatProvider provider) => ((IConvertible)m_Value).ToDouble(provider);

        decimal IConvertible.ToDecimal(IFormatProvider provider) => ((IConvertible)m_Value).ToDecimal(provider);

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => ((IConvertible)m_Value).ToDateTime(provider);

        string IConvertible.ToString(IFormatProvider provider) => m_Value.ToString(provider);

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)m_Value).ToType(conversionType, provider);

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue("value", m_Value);

        public static VInt operator +(VInt value) => value;

        public static VInt operator -(VInt value) => new VInt(-value.m_Value);

        public static VInt operator ~(VInt value) => new VInt(~value.m_Value);

        public static VInt operator ++(VInt value) => new VInt(value.m_Value + 1);

        public static VInt operator --(VInt value) => new VInt(value.m_Value - 1);

        public static VInt operator +(VInt left, VInt right) => new VInt(left.m_Value + right.m_Value);

        public static VInt operator -(VInt left, VInt right) => new VInt(left.m_Value - right.m_Value);

        public static VInt operator *(VInt left, VInt right) => new VInt(left.m_Value * right.m_Value);

        public static VInt operator /(VInt left, VInt right) => new VInt(left.m_Value / right.m_Value);

        public static VInt operator %(VInt left, VInt right) => new VInt(left.m_Value % right.m_Value);

        public static VInt operator <<(VInt left, int right) => new VInt(left.m_Value << right);

        public static VInt operator >>(VInt left, int right) => new VInt(left.m_Value >> right);

        public static VInt operator &(VInt left, VInt right) => new VInt(left.m_Value & right.m_Value);

        public static VInt operator |(VInt left, VInt right) => new VInt(left.m_Value | right.m_Value);

        public static VInt operator ^(VInt left, VInt right) => new VInt(left.m_Value ^ right.m_Value);

        public static bool operator ==(VInt left, VInt right) => left.m_Value == right.m_Value;

        public static bool operator !=(VInt left, VInt right) => left.m_Value != right.m_Value;

        public static bool operator >(VInt left, VInt right) => left.m_Value > right.m_Value;

        public static bool operator <(VInt left, VInt right) => left.m_Value < right.m_Value;

        public static bool operator >=(VInt left, VInt right) => left.m_Value >= right.m_Value;

        public static bool operator <=(VInt left, VInt right) => left.m_Value <= right.m_Value;

        public static implicit operator VInt(sbyte value) => new VInt(value);

        public static implicit operator VInt(byte value) => new VInt(value);

        public static implicit operator VInt(short value) => new VInt(value);

        public static implicit operator VInt(ushort value) => new VInt(value);

        public static implicit operator VInt(int value) => new VInt(value);

        public static implicit operator VInt(uint value) => new VInt(value);

        public static implicit operator VInt(long value) => new VInt(value);

        public static explicit operator VInt(ulong value) => new VInt(value);

        public static explicit operator VInt(VUInt value) => new VInt((long)value);

        public static explicit operator sbyte(VInt value) => (sbyte)value.m_Value;

        public static explicit operator byte(VInt value) => (byte)value.m_Value;

        public static explicit operator short(VInt value) => (short)value.m_Value;

        public static explicit operator ushort(VInt value) => (ushort)value.m_Value;

        public static explicit operator int(VInt value) => (int)value.m_Value;

        public static explicit operator uint(VInt value) => (uint)value.m_Value;

        public static explicit operator long(VInt value) => value.m_Value;

        public static explicit operator ulong(VInt value) => (ulong)value.m_Value;

        public static explicit operator VUInt(VInt value) => new VUInt((ulong)value.m_Value);
#pragma warning restore CS1591
    }
#pragma warning restore IDE1006
}
