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
        sbyte ISerializeProxy<sbyte>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadInt8(0, context.DefaultNumberType);
        }

        byte ISerializeProxy<byte>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadUInt8(0, context.DefaultNumberType);
        }

        short ISerializeProxy<short>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadInt16(0, context.DefaultNumberType);
        }

        ushort ISerializeProxy<ushort>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadUInt16(0, context.DefaultNumberType);
        }

        uint ISerializeProxy<uint>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadUInt32(0, context.DefaultNumberType);
        }

        int ISerializeProxy<int>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadInt32(0, context.DefaultNumberType);
        }

        ulong ISerializeProxy<ulong>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadUInt64(0, context.DefaultNumberType);
        }

        double ISerializeProxy<double>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadFloat64(0, context.DefaultNumberType);
        }

        long ISerializeProxy<long>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadInt64(0, context.DefaultNumberType);
        }

        float ISerializeProxy<float>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadFloat32(0, context.DefaultNumberType);
        }

        bool ISerializeProxy<bool>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadBoolean(0);
        }

        char ISerializeProxy<char>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadChar(0, context.DefaultEncoding);
        }

        string ISerializeProxy<string>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return reader.ReadString(0, context.DefaultEncoding);
        }

        void ISerializeProxy<sbyte>.Serialize(sbyte obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<byte>.Serialize(byte obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<short>.Serialize(short obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<ushort>.Serialize(ushort obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<uint>.Serialize(uint obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<int>.Serialize(int obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<ulong>.Serialize(ulong obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<double>.Serialize(double obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<long>.Serialize(long obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<float>.Serialize(float obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultNumberType);
        }

        void ISerializeProxy<bool>.Serialize(bool obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj);
        }

        void ISerializeProxy<char>.Serialize(char obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultEncoding);
        }

        void ISerializeProxy<string>.Serialize(string obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj, context.DefaultEncoding);
        }
    }
}
