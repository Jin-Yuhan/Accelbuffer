using Accelbuffer.Text;

namespace Accelbuffer.Injection
{
    internal sealed class PrimitiveTypeSerializer :
        ITypeSerializer<sbyte>,
        ITypeSerializer<byte>,
        ITypeSerializer<short>,
        ITypeSerializer<ushort>,
        ITypeSerializer<int>,
        ITypeSerializer<uint>,
        ITypeSerializer<long>,
        ITypeSerializer<ulong>,
        ITypeSerializer<float>,
        ITypeSerializer<double>,
        ITypeSerializer<decimal>,
        ITypeSerializer<bool>,
        ITypeSerializer<char>,
        ITypeSerializer<string>
    {
        sbyte ITypeSerializer<sbyte>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsInt8WithoutTag(NumberFormat.Variant) : default;
        }

        byte ITypeSerializer<byte>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsUInt8WithoutTag(NumberFormat.Variant) : default;
        }

        short ITypeSerializer<short>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsInt16WithoutTag(NumberFormat.Variant) : default;
        }

        ushort ITypeSerializer<ushort>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsUInt16WithoutTag(NumberFormat.Variant) : default;
        }

        uint ITypeSerializer<uint>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsUInt32WithoutTag(NumberFormat.Variant) : default;
        }

        int ITypeSerializer<int>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsInt32WithoutTag(NumberFormat.Variant) : default;
        }

        ulong ITypeSerializer<ulong>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsUInt64WithoutTag(NumberFormat.Variant) : default;
        }

        double ITypeSerializer<double>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsFloat64WithoutTag(): default;
        }

        long ITypeSerializer<long>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsInt64WithoutTag(NumberFormat.Variant) : default;
        }

        float ITypeSerializer<float>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsFloat32WithoutTag(): default;
        }

        bool ITypeSerializer<bool>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsBooleanWithoutTag(): default;
        }

        char ITypeSerializer<char>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsCharWithoutTag(Encoding.UTF8): default;
        }

        string ITypeSerializer<string>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsStringWithoutTag(Encoding.UTF8) : default;
        }

        decimal ITypeSerializer<decimal>.Deserialize(ref StreamingIterator iterator)
        {
            return iterator.HasNext() ? iterator.NextAsFloat128WithoutTag() : default;
        }

        void ITypeSerializer<sbyte>.Serialize(sbyte obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<byte>.Serialize(byte obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<short>.Serialize(short obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<ushort>.Serialize(ushort obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<uint>.Serialize(uint obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<int>.Serialize(int obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<ulong>.Serialize(ulong obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<double>.Serialize(double obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj);
        }

        void ITypeSerializer<long>.Serialize(long obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, NumberFormat.Variant);
        }

        void ITypeSerializer<float>.Serialize(float obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj);
        }

        void ITypeSerializer<bool>.Serialize(bool obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj);
        }

        void ITypeSerializer<char>.Serialize(char obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, Encoding.UTF8);
        }

        void ITypeSerializer<string>.Serialize(string obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj, Encoding.UTF8);
        }

        void ITypeSerializer<decimal>.Serialize(decimal obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj);
        }
    }
}
