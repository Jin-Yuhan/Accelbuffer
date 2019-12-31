using System;

namespace Accelbuffer
{
    internal sealed class SerializableEnumerableSerializeProxy<T, TValue> : ISerializeProxy<T> where T : ISerializableEnumerable<TValue>
    {
        unsafe T ISerializeProxy<T>.Deserialize(in UnmanagedReader* reader)
        {
            int count = reader->ReadVariableInt32(0);

            if (count == -1)
            {
                return default;
            }

            Type type = Type.GetType(reader->ReadString(1, CharEncoding.ASCII));
            T result = (T)Activator.CreateInstance(type);

            while (count > 0)
            {
                result.Add(Serializer<TValue>.Deserialize(reader));
                count--;
            }

            return result;
        }

        unsafe void ISerializeProxy<T>.Serialize(in T obj, in UnmanagedWriter* writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer->WriteValue(0, count, Number.Var);

            if (count == -1)
            {
                return;
            }

            writer->WriteValue(1, obj.GetType().AssemblyQualifiedName, CharEncoding.ASCII);

            foreach (TValue o in obj)
            {
                Serializer<TValue>.Serialize(o, writer);
            }
        }
    }
}
