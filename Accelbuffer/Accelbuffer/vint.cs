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
    public readonly struct vint : IComparable, IFormattable, IConvertible, IComparable<vint>, IEquatable<vint>, ISerializable
    {
#pragma warning disable CS1591
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly vint MaxValue = long.MaxValue;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly vint MinValue = long.MinValue;

#if UNITY
        [UnityEngine.SerializeField]
#endif

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly long m_Value;

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I1;

        [FieldOffset(4)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I2;

        public vint(sbyte value) : this() => m_Value = value;

        public vint(byte value) : this() => m_Value = value;

        public vint(short value) : this() => m_Value = value;

        public vint(ushort value) : this() => m_Value = value;

        public vint(int value) : this() => m_Value = value;

        public vint(uint value) : this() => m_Value = value;

        public vint(long value) : this() => m_Value = value;

        public vint(ulong value) : this() => m_Value = (long)value;

        public vint(SerializationInfo info, StreamingContext context) : this() => m_Value = info.GetInt64("value");

        public int CompareTo(vint other) => m_Value.CompareTo(other.m_Value);

        public bool Equals(vint other) => m_Value.Equals(other.m_Value);

        public override bool Equals(object obj) => obj is vint v && Equals(v);

        public override int GetHashCode() => m_Value.GetHashCode();

        public override string ToString() => m_Value.ToString();

        public string ToString(string format) => m_Value.ToString(format);

        public string ToString(IFormatProvider provider) => m_Value.ToString(provider);

        public string ToString(string format, IFormatProvider provider) => m_Value.ToString(format, provider);

        public int CompareTo(object obj) => m_Value.CompareTo(obj);

        public static vint Parse(string s, NumberStyles style, IFormatProvider provider) => long.Parse(s, style, provider);

        public static vint Parse(string s, IFormatProvider provider) => long.Parse(s, provider);

        public static vint Parse(string s) => long.Parse(s);

        public static vint Parse(string s, NumberStyles style) => long.Parse(s, style);

        public static bool TryParse(string s, out vint result)
        {
            bool u = long.TryParse(s, out long l);
            result = new vint(l);
            return u;
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out vint result)
        {
            bool u = long.TryParse(s, style, provider, out long l);
            result = new vint(l);
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

        public static vint operator +(vint value) => value;

        public static vint operator -(vint value) => new vint(-value.m_Value);

        public static vint operator ~(vint value) => new vint(~value.m_Value);

        public static vint operator ++(vint value) => new vint(value.m_Value + 1);

        public static vint operator --(vint value) => new vint(value.m_Value - 1);

        public static vint operator +(vint left, vint right) => new vint(left.m_Value + right.m_Value);

        public static vint operator -(vint left, vint right) => new vint(left.m_Value - right.m_Value);

        public static vint operator *(vint left, vint right) => new vint(left.m_Value * right.m_Value);

        public static vint operator /(vint left, vint right) => new vint(left.m_Value / right.m_Value);

        public static vint operator %(vint left, vint right) => new vint(left.m_Value % right.m_Value);

        public static vint operator <<(vint left, int right) => new vint(left.m_Value << right);

        public static vint operator >>(vint left, int right) => new vint(left.m_Value >> right);

        public static vint operator &(vint left, vint right) => new vint(left.m_Value & right.m_Value);

        public static vint operator |(vint left, vint right) => new vint(left.m_Value | right.m_Value);

        public static vint operator ^(vint left, vint right) => new vint(left.m_Value ^ right.m_Value);

        public static bool operator ==(vint left, vint right) => left.m_Value == right.m_Value;

        public static bool operator !=(vint left, vint right) => left.m_Value != right.m_Value;

        public static bool operator >(vint left, vint right) => left.m_Value > right.m_Value;

        public static bool operator <(vint left, vint right) => left.m_Value < right.m_Value;

        public static bool operator >=(vint left, vint right) => left.m_Value >= right.m_Value;

        public static bool operator <=(vint left, vint right) => left.m_Value <= right.m_Value;

        public static implicit operator vint(sbyte value) => new vint(value);

        public static implicit operator vint(byte value) => new vint(value);

        public static implicit operator vint(short value) => new vint(value);

        public static implicit operator vint(ushort value) => new vint(value);

        public static implicit operator vint(int value) => new vint(value);

        public static implicit operator vint(uint value) => new vint(value);

        public static implicit operator vint(long value) => new vint(value);

        public static explicit operator vint(ulong value) => new vint(value);

        public static explicit operator vint(vuint value) => new vint((long)value);

        public static explicit operator sbyte(vint value) => (sbyte)value.m_Value;

        public static explicit operator byte(vint value) => (byte)value.m_Value;

        public static explicit operator short(vint value) => (short)value.m_Value;

        public static explicit operator ushort(vint value) => (ushort)value.m_Value;

        public static explicit operator int(vint value) => (int)value.m_Value;

        public static explicit operator uint(vint value) => (uint)value.m_Value;

        public static explicit operator long(vint value) => value.m_Value;

        public static explicit operator ulong(vint value) => (ulong)value.m_Value;

        public static explicit operator vuint(vint value) => new vuint((ulong)value.m_Value);
#pragma warning restore CS1591
    }
#pragma warning restore IDE1006
}
