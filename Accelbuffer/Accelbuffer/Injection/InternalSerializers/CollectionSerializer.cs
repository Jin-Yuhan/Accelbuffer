using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class CollectionSerializer<T, TValue> : ITypeSerializer<T> where T : ICollection<TValue>, new()
    {
        T ITypeSerializer<T>.Deserialize(ref StreamingIterator iterator)
        {
            int count = iterator.HasNext() ? iterator.NextAsInt32WithoutTag(NumberFormat.Variant) : 0;

            if (count == -1)
            {
                return default;
            }

            T result = new T();

            while (iterator.HasNext())
            {
                result.Add(iterator.NextAsWithoutTag<TValue>());
            }

            return result;
        }

        void ITypeSerializer<T>.Serialize(T obj, ref StreamingWriter writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer.WriteValue(count, NumberFormat.Variant);

            if (count == -1)
            {
                return;
            }

            foreach (TValue o in obj)
            {
                writer.WriteValue<TValue>(o);
            }
        }
    }
}
