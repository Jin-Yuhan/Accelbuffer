using Accelbuffer.Memory;
using System;

namespace Accelbuffer.Injection
{
    internal sealed class IntPtrSerializer : 
        IBuiltinTypeSerializer, 
        IMemorySizeForType<IntPtr>, 
        IMemorySizeForType<UIntPtr>, 
        ITypeSerializer<IntPtr>, 
        ITypeSerializer<UIntPtr>
    {
        int IMemorySizeForType<IntPtr>.ApproximateMemorySize => 8;

        int IMemorySizeForType<UIntPtr>.ApproximateMemorySize => 8;

        IntPtr ITypeSerializer<IntPtr>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadIntPtr();
        }

        UIntPtr ITypeSerializer<UIntPtr>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadUIntPtr();
        }

        void ITypeSerializer<IntPtr>.Serialize(IntPtr obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.Index, obj);
        }

        void ITypeSerializer<UIntPtr>.Serialize(UIntPtr obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.Index, obj);
        }
    }
}
