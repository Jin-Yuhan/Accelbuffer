using Accelbuffer.Memory;

namespace Accelbuffer.Injection
{
    internal sealed class VariantSerializer :
        IBuiltinTypeSerializer,
        IMemorySizeForType<vint>,
        IMemorySizeForType<vuint>,
        ITypeSerializer<vint>,
        ITypeSerializer<vuint>
    {
        int IMemorySizeForType<vint>.ApproximateMemorySize => 8;

        int IMemorySizeForType<vuint>.ApproximateMemorySize => 8;

        vint ITypeSerializer<vint>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVariantInt();
        }

        vuint ITypeSerializer<vuint>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVariantUInt();
        }

        void ITypeSerializer<vint>.Serialize(vint obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<vuint>.Serialize(vuint obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }
    }
}
