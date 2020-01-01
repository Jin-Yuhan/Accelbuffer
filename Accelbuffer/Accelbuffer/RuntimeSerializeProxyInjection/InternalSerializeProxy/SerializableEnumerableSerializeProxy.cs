namespace Accelbuffer
{
    internal sealed class SerializableEnumerableSerializeProxy<T, TValue> : ISerializeProxy<T> where T : ISerializableEnumerable<TValue>, new()
    {
        unsafe T ISerializeProxy<T>.Deserialize(in UnmanagedReader* reader)
        {
            int count = reader->ReadVariableInt32(0);

            if (count == -1)
            {
                return default;
            }

            T result = new T();
            result.Initialize(count);

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

            foreach (TValue o in obj)
            {
                Serializer<TValue>.Serialize(o, writer);
            }
        }
    }
}
