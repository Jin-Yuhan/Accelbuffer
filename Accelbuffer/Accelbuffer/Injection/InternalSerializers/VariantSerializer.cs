using Accelbuffer.Memory;

namespace Accelbuffer.Injection
{
    internal sealed class VariantSerializer :
        IBuiltinTypeSerializer,
        IMemorySizeForType<VInt>,
        IMemorySizeForType<VUInt>,
        ITypeSerializer<VInt>,
        ITypeSerializer<VUInt>
    {
        int IMemorySizeForType<VInt>.ApproximateMemorySize => 8;

        int IMemorySizeForType<VUInt>.ApproximateMemorySize => 8;

        VInt ITypeSerializer<VInt>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVariantInt();
        }

        VUInt ITypeSerializer<VUInt>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVariantUInt();
        }

        void ITypeSerializer<VInt>.Serialize(VInt obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.Index, obj);
        }

        void ITypeSerializer<VUInt>.Serialize(VUInt obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.Index, obj);
        }
    }
}
