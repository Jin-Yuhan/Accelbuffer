namespace Accelbuffer.Runtime.Injection
{
    internal sealed class PrimitiveTypeSerializeProxy :
        ISerializeProxy<sbyte>,
        ISerializeProxy<byte>,
        ISerializeProxy<short>,
        ISerializeProxy<ushort>,
        ISerializeProxy<int>,
        ISerializeProxy<uint>,
        ISerializeProxy<long>,
        ISerializeProxy<ulong>,
        ISerializeProxy<float>,
        ISerializeProxy<double>,
        ISerializeProxy<bool>,
        ISerializeProxy<char>,
        ISerializeProxy<string>
    {
        sbyte ISerializeProxy<sbyte>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableInt8(0);
        }

        byte ISerializeProxy<byte>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableUInt8(0);
        }

        short ISerializeProxy<short>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableInt16(0);
        }

        ushort ISerializeProxy<ushort>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableUInt16(0);
        }

        uint ISerializeProxy<uint>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableUInt32(0);
        }

        int ISerializeProxy<int>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableInt32(0);
        }

        ulong ISerializeProxy<ulong>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableUInt64(0);
        }

        double ISerializeProxy<double>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableFloat64(0);
        }

        long ISerializeProxy<long>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableInt64(0);
        }

        float ISerializeProxy<float>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableFloat32(0);
        }

        bool ISerializeProxy<bool>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadBoolean(0);
        }

        char ISerializeProxy<char>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadChar(0, SerializationUtility.DefaultCharEncoding);
        }

        string ISerializeProxy<string>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadString(0, SerializationUtility.DefaultCharEncoding);
        }

        void ISerializeProxy<sbyte>.Serialize(sbyte obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<byte>.Serialize(byte obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<short>.Serialize(short obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<ushort>.Serialize(ushort obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<uint>.Serialize(uint obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<int>.Serialize(int obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<ulong>.Serialize(ulong obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<double>.Serialize(double obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<long>.Serialize(long obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<float>.Serialize(float obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultNumberType);
        }

        void ISerializeProxy<bool>.Serialize(bool obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj);
        }

        void ISerializeProxy<char>.Serialize(char obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultCharEncoding);
        }

        void ISerializeProxy<string>.Serialize(string obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, SerializationUtility.DefaultCharEncoding);
        }
    }
}
