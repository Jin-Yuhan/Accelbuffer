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
    /// 表示一个动态长度的无符号整数
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{m_Value}")]
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    [CLSCompliant(false)]
    public struct VUInt : IComparable, IFormattable, IConvertible, IComparable<VUInt>, IEquatable<VUInt>, ISerializable
    {
#pragma warning disable CS1591
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly VUInt MaxValue = ulong.MaxValue;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly VUInt MinValue = ulong.MinValue;

#if UNITY
        [UnityEngine.SerializeField]
#endif

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if UNITY
        internal ulong m_Value;
#else
        internal readonly ulong m_Value;
#endif

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I1;

        [FieldOffset(4)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I2;

        [CLSCompliant(false)]
        public VUInt(sbyte value) : this() => m_Value = (ulong)value;

        public VUInt(byte value) : this() => m_Value = value;

        public VUInt(short value) : this() => m_Value = (ulong)value;

        [CLSCompliant(false)]
        public VUInt(ushort value) : this() => m_Value = value;

        public VUInt(int value) : this() => m_Value = (ulong)value;

        [CLSCompliant(false)]
        public VUInt(uint value) : this() => m_Value = value;

        public VUInt(long value) : this() => m_Value = (ulong)value;

        [CLSCompliant(false)]
        public VUInt(ulong value) : this() => m_Value = value;

        public VUInt(SerializationInfo info, StreamingContext context) : this() => m_Value = info.GetUInt64("value");

        public int CompareTo(VUInt other) => m_Value.CompareTo(other.m_Value);

        public bool Equals(VUInt other) => m_Value.Equals(other.m_Value);

        public override bool Equals(object obj) => obj is VUInt v && Equals(v);

        public override int GetHashCode() => m_Value.GetHashCode();

        public override string ToString() => m_Value.ToString();

        public string ToString(string format) => m_Value.ToString(format);

        public string ToString(IFormatProvider provider) => m_Value.ToString(provider);

        public string ToString(string format, IFormatProvider provider) => m_Value.ToString(format, provider);

        public int CompareTo(object obj) => m_Value.CompareTo(obj);

        public static VUInt Parse(string s, NumberStyles style, IFormatProvider provider) => ulong.Parse(s, style, provider);

        public static VUInt Parse(string s, IFormatProvider provider) => ulong.Parse(s, provider);

        public static VUInt Parse(string s) => ulong.Parse(s);

        public static VUInt Parse(string s, NumberStyles style) => ulong.Parse(s, style);

        public static bool TryParse(string s, out VUInt result)
        {
            bool u = ulong.TryParse(s, out ulong l);
            result = new VUInt(l);
            return u;
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out VUInt result)
        {
            bool u = ulong.TryParse(s, style, provider, out ulong l);
            result = new VUInt(l);
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

        public static VUInt operator +(VUInt value) => value;

        public static VUInt operator ~(VUInt value) => new VUInt(~value.m_Value);

        public static VUInt operator ++(VUInt value) => new VUInt(value.m_Value + 1);

        public static VUInt operator --(VUInt value) => new VUInt(value.m_Value - 1);

        public static VUInt operator +(VUInt left, VUInt right) => new VUInt(left.m_Value + right.m_Value);

        public static VUInt operator -(VUInt left, VUInt right) => new VUInt(left.m_Value - right.m_Value);

        public static VUInt operator *(VUInt left, VUInt right) => new VUInt(left.m_Value * right.m_Value);

        public static VUInt operator /(VUInt left, VUInt right) => new VUInt(left.m_Value / right.m_Value);

        public static VUInt operator %(VUInt left, VUInt right) => new VUInt(left.m_Value % right.m_Value);

        public static VUInt operator <<(VUInt left, int right) => new VUInt(left.m_Value << right);

        public static VUInt operator >>(VUInt left, int right) => new VUInt(left.m_Value >> right);

        public static VUInt operator &(VUInt left, VUInt right) => new VUInt(left.m_Value & right.m_Value);

        public static VUInt operator |(VUInt left, VUInt right) => new VUInt(left.m_Value | right.m_Value);

        public static VUInt operator ^(VUInt left, VUInt right) => new VUInt(left.m_Value ^ right.m_Value);

        public static bool operator ==(VUInt left, VUInt right) => left.m_Value == right.m_Value;

        public static bool operator !=(VUInt left, VUInt right) => left.m_Value != right.m_Value;

        public static bool operator >(VUInt left, VUInt right) => left.m_Value > right.m_Value;

        public static bool operator <(VUInt left, VUInt right) => left.m_Value < right.m_Value;

        public static bool operator >=(VUInt left, VUInt right) => left.m_Value >= right.m_Value;

        public static bool operator <=(VUInt left, VUInt right) => left.m_Value <= right.m_Value;

        [CLSCompliant(false)]
        public static explicit operator VUInt(sbyte value) => new VUInt(value);

        public static implicit operator VUInt(byte value) => new VUInt(value);

        public static explicit operator VUInt(short value) => new VUInt(value);

        [CLSCompliant(false)]
        public static implicit operator VUInt(ushort value) => new VUInt(value);

        public static explicit operator VUInt(int value) => new VUInt(value);

        [CLSCompliant(false)]
        public static implicit operator VUInt(uint value) => new VUInt(value);

        public static explicit operator VUInt(long value) => new VUInt(value);

        [CLSCompliant(false)]
        public static implicit operator VUInt(ulong value) => new VUInt(value);

        public static explicit operator VUInt(VInt value) => new VUInt((ulong)value);

        [CLSCompliant(false)]
        public static explicit operator sbyte(VUInt value) => (sbyte)value.m_Value;

        public static explicit operator byte(VUInt value) => (byte)value.m_Value;

        public static explicit operator short(VUInt value) => (short)value.m_Value;

        [CLSCompliant(false)]
        public static explicit operator ushort(VUInt value) => (ushort)value.m_Value;

        public static explicit operator int(VUInt value) => (int)value.m_Value;

        [CLSCompliant(false)]
        public static explicit operator uint(VUInt value) => (uint)value.m_Value;

        public static explicit operator long(VUInt value) => (long)value.m_Value;

        [CLSCompliant(false)]
        public static explicit operator ulong(VUInt value) => value.m_Value;

        public static explicit operator VInt(VUInt value) => new VInt(value.m_Value);
#pragma warning restore CS1591
    }
#pragma warning restore IDE1006
}
