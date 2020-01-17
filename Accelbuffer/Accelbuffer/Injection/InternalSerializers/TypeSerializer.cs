using Accelbuffer.Text;
using System;

namespace Accelbuffer.Injection
{
    internal sealed class TypeSerializer : ITypeSerializer<Type>
    {
        Type ITypeSerializer<Type>.Deserialize(ref StreamingIterator iterator)
        {
            if (!iterator.HasNext())
            {
                return null;
            } 

            string value = iterator.NextAsStringWithoutTag(Encoding.UTF8);
            return Type.GetType(value, false);
        }

        void ITypeSerializer<Type>.Serialize(Type obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.AssemblyQualifiedName, Encoding.UTF8);
        }
    }
}
