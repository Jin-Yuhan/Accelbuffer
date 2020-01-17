using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class CollectionSerializer<T, TValue> : ITypeSerializer<T> where T : ICollection<TValue>, new()
    {
        T ITypeSerializer<T>.Deserialize(ref StreamingIterator iterator)
        {
            T result = new T();

            while (iterator.HasNext())
            {
                result.Add(iterator.NextAsWithoutTag<TValue>());
            }

            return result;
        }

        void ITypeSerializer<T>.Serialize(T obj, ref StreamingWriter writer)
        {
            foreach (TValue o in obj)
            {
                writer.WriteValue<TValue>(o);
            }
        }
    }
}
