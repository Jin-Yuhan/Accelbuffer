using System;

namespace Accelbuffer.Injection
{
    internal sealed class TypeSerializer : ITypeSerializer<Type>
    {
        Type ITypeSerializer<Type>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext())
            {
                return null;
            } 

            string value = reader.ReadString();
            return Type.GetType(value, false);
        }

        void ITypeSerializer<Type>.Serialize(Type obj, ref AccelWriter writer)
        {
            writer.WriteValue(1, obj.AssemblyQualifiedName);
        }
    }
}
