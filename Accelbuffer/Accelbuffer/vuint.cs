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
    public readonly struct vuint : IComparable, IFormattable, IConvertible, IComparable<vuint>, IEquatable<vuint>, ISerializable
    {
#pragma warning disable CS1591
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly vuint MaxValue = ulong.MaxValue;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly vuint MinValue = ulong.MinValue;

#if UNITY
        [UnityEngine.SerializeField]
#endif

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly ulong m_Value;

        [FieldOffset(0)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I1;

        [FieldOffset(4)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly uint m_I2;

        public vuint(sbyte value) : this() => m_Value = (ulong)value;

        public vuint(byte value) : this() => m_Value = value;

        public vuint(short value) : this() => m_Value = (ulong)value;

        public vuint(ushort value) : this() => m_Value = value;

        public vuint(int value) : this() => m_Value = (ulong)value;

        public vuint(uint value) : this() => m_Value = value;

        public vuint(long value) : this() => m_Value = (ulong)value;

        public vuint(ulong value) : this() => m_Value = value;

        public vuint(SerializationInfo info, StreamingContext context) : this() => m_Value = info.GetUInt64("value");

        public int CompareTo(vuint other) => m_Value.CompareTo(other.m_Value);

        public bool Equals(vuint other) => m_Value.Equals(other.m_Value);

        public override bool Equals(object obj) => obj is vuint v && Equals(v);

        public override int GetHashCode() => m_Value.GetHashCode();

        public override string ToString() => m_Value.ToString();

        public string ToString(string format) => m_Value.ToString(format);

        public string ToString(IFormatProvider provider) => m_Value.ToString(provider);

        public string ToString(string format, IFormatProvider provider) => m_Value.ToString(format, provider);

        public int CompareTo(object obj) => m_Value.CompareTo(obj);

        public static vuint Parse(string s, NumberStyles style, IFormatProvider provider) => ulong.Parse(s, style, provider);

        public static vuint Parse(string s, IFormatProvider provider) => ulong.Parse(s, provider);

        public static vuint Parse(string s) => ulong.Parse(s);

        public static vuint Parse(string s, NumberStyles style) => ulong.Parse(s, style);

        public static bool TryParse(string s, out vuint result)
        {
            bool u = ulong.TryParse(s, out ulong l);
            result = new vuint(l);
            return u;
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out vuint result)
        {
            bool u = ulong.TryParse(s, style, provider, out ulong l);
            result = new vuint(l);
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

        public static vuint operator +(vuint value) => value;

        public static vuint operator ~(vuint value) => new vuint(~value.m_Value);

        public static vuint operator ++(vuint value) => new vuint(value.m_Value + 1);

        public static vuint operator --(vuint value) => new vuint(value.m_Value - 1);

        public static vuint operator +(vuint left, vuint right) => new vuint(left.m_Value + right.m_Value);

        public static vuint operator -(vuint left, vuint right) => new vuint(left.m_Value - right.m_Value);

        public static vuint operator *(vuint left, vuint right) => new vuint(left.m_Value * right.m_Value);

        public static vuint operator /(vuint left, vuint right) => new vuint(left.m_Value / right.m_Value);

        public static vuint operator %(vuint left, vuint right) => new vuint(left.m_Value % right.m_Value);

        public static vuint operator <<(vuint left, int right) => new vuint(left.m_Value << right);

        public static vuint operator >>(vuint left, int right) => new vuint(left.m_Value >> right);

        public static vuint operator &(vuint left, vuint right) => new vuint(left.m_Value & right.m_Value);

        public static vuint operator |(vuint left, vuint right) => new vuint(left.m_Value | right.m_Value);

        public static vuint operator ^(vuint left, vuint right) => new vuint(left.m_Value ^ right.m_Value);

        public static bool operator ==(vuint left, vuint right) => left.m_Value == right.m_Value;

        public static bool operator !=(vuint left, vuint right) => left.m_Value != right.m_Value;

        public static bool operator >(vuint left, vuint right) => left.m_Value > right.m_Value;

        public static bool operator <(vuint left, vuint right) => left.m_Value < right.m_Value;

        public static bool operator >=(vuint left, vuint right) => left.m_Value >= right.m_Value;

        public static bool operator <=(vuint left, vuint right) => left.m_Value <= right.m_Value;

        public static explicit operator vuint(sbyte value) => new vuint(value);

        public static implicit operator vuint(byte value) => new vuint(value);

        public static explicit operator vuint(short value) => new vuint(value);

        public static implicit operator vuint(ushort value) => new vuint(value);

        public static explicit operator vuint(int value) => new vuint(value);

        public static implicit operator vuint(uint value) => new vuint(value);

        public static explicit operator vuint(long value) => new vuint(value);

        public static implicit operator vuint(ulong value) => new vuint(value);

        public static explicit operator vuint(vint value) => new vuint((ulong)value);

        public static explicit operator sbyte(vuint value) => (sbyte)value.m_Value;

        public static explicit operator byte(vuint value) => (byte)value.m_Value;

        public static explicit operator short(vuint value) => (short)value.m_Value;

        public static explicit operator ushort(vuint value) => (ushort)value.m_Value;

        public static explicit operator int(vuint value) => (int)value.m_Value;

        public static explicit operator uint(vuint value) => (uint)value.m_Value;

        public static explicit operator long(vuint value) => (long)value.m_Value;

        public static explicit operator ulong(vuint value) => value.m_Value;

        public static explicit operator vint(vuint value) => new vint(value.m_Value);
#pragma warning restore CS1591
    }
#pragma warning restore IDE1006
}
