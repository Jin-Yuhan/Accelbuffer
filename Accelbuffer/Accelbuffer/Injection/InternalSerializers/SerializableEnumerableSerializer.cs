namespace Accelbuffer.Injection
{
    internal sealed class SerializableEnumerableSerializer<T, TValue> : ITypeSerializer<T> where T : ISerializableEnumerable<TValue>, new()
    {
        T ITypeSerializer<T>.Deserialize(ref StreamingIterator iterator)
        {
            int count = iterator.HasNext() ? iterator.NextAsInt32WithoutTag(NumberFormat.Variant) : 0;

            T result = new T();
            result.Initialize(count);

            while (iterator.HasNext())
            {
                result.Add(iterator.NextAsWithoutTag<TValue>());
            }

            return result;
        }

        void ITypeSerializer<T>.Serialize(T obj, ref StreamingWriter writer)
        {
            int count = obj.Count;
            writer.WriteValue(count, NumberFormat.Variant);

            foreach (TValue o in obj)
            {
                writer.WriteValue(o);
            }
        }
    }
}
