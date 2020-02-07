using Accelbuffer.Memory;

namespace Accelbuffer.Injection
{
    internal sealed class PrimitiveTypeSerializer :
        IBuiltinTypeSerializer,

        IMemorySizeForType<sbyte>,
        IMemorySizeForType<byte>,
        IMemorySizeForType<short>,
        IMemorySizeForType<ushort>,
        IMemorySizeForType<int>,
        IMemorySizeForType<uint>,
        IMemorySizeForType<long>,
        IMemorySizeForType<ulong>,
        IMemorySizeForType<float>,
        IMemorySizeForType<double>,
        IMemorySizeForType<decimal>,
        IMemorySizeForType<bool>,
        IMemorySizeForType<char>,

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
        int IMemorySizeForType<sbyte>.ApproximateMemorySize => 1;

        int IMemorySizeForType<byte>.ApproximateMemorySize => 1;

        int IMemorySizeForType<short>.ApproximateMemorySize => 2;

        int IMemorySizeForType<ushort>.ApproximateMemorySize => 2;

        int IMemorySizeForType<int>.ApproximateMemorySize => 4;

        int IMemorySizeForType<uint>.ApproximateMemorySize => 4;

        int IMemorySizeForType<long>.ApproximateMemorySize => 8;

        int IMemorySizeForType<ulong>.ApproximateMemorySize => 8;

        int IMemorySizeForType<float>.ApproximateMemorySize => 4;

        int IMemorySizeForType<double>.ApproximateMemorySize => 8;

        int IMemorySizeForType<decimal>.ApproximateMemorySize => 16;

        int IMemorySizeForType<bool>.ApproximateMemorySize => 1;

        int IMemorySizeForType<char>.ApproximateMemorySize => 2;

        sbyte ITypeSerializer<sbyte>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadInt8();
        }

        byte ITypeSerializer<byte>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadUInt8();
        }

        short ITypeSerializer<short>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadInt16();
        }

        ushort ITypeSerializer<ushort>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadUInt16();
        }

        int ITypeSerializer<int>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadInt32();
        }

        uint ITypeSerializer<uint>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadUInt32();
        }

        long ITypeSerializer<long>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadInt64();
        }

        ulong ITypeSerializer<ulong>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadUInt64();
        }

        float ITypeSerializer<float>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadFloat32();
        }

        double ITypeSerializer<double>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadFloat64();
        }

        decimal ITypeSerializer<decimal>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadFloat128();
        }

        bool ITypeSerializer<bool>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadBoolean();
        }

        char ITypeSerializer<char>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadChar();
        }

        string ITypeSerializer<string>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadString();
        }

        void ITypeSerializer<sbyte>.Serialize(sbyte obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<byte>.Serialize(byte obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<short>.Serialize(short obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<ushort>.Serialize(ushort obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<int>.Serialize(int obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<uint>.Serialize(uint obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<long>.Serialize(long obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<ulong>.Serialize(ulong obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<float>.Serialize(float obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<double>.Serialize(double obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<decimal>.Serialize(decimal obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<bool>.Serialize(bool obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<char>.Serialize(char obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<string>.Serialize(string obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }
    }
}
