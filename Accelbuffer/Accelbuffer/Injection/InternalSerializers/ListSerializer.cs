using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class ListSerializer<T> : ITypeSerializer<List<T>>
    {
        List<T> ITypeSerializer<List<T>>.Deserialize(ref StreamingIterator iterator)
        {
            int count = iterator.HasNext() ? iterator.NextAsInt32WithoutTag(NumberFormat.Variant) : 0;
            List<T> result = new List<T>(count);

            while (iterator.HasNext())
            {
                result.Add(iterator.NextAsWithoutTag<T>());
            }

            return result;
        }

        void ITypeSerializer<List<T>>.Serialize(List<T> obj, ref StreamingWriter writer)
        {
            int count = obj.Count;
            writer.WriteValue(count, NumberFormat.Variant);

            for (int i = 0; i < count; i++)
            {
                writer.WriteValue(obj[i]);
            }
        }
    }

    internal sealed class ListSerializeProxy<T, TValue> : ITypeSerializer<T> where T : IList<TValue>, new()
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
            for (int i = 0; i < obj.Count; i++)
            {
                writer.WriteValue(obj[i]);
            }
        }
    }
}
