using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class DictionarySerializer<TKey, TValue> : ITypeSerializer<Dictionary<TKey, TValue>>
    {
        Dictionary<TKey, TValue> ITypeSerializer<Dictionary<TKey, TValue>>.Deserialize(ref StreamingIterator iterator)
        {
            int count = iterator.HasNext() ? iterator.NextAsInt32WithoutTag(NumberFormat.Variant) : 0;

            if (count == -1)
            {
                return null;
            }

            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(count);

            while (iterator.HasNext())
            {
                TKey key = iterator.NextAsWithoutTag<TKey>();
                TValue value = iterator.HasNext() ? iterator.NextAsWithoutTag<TValue>() : default;
                result.Add(key, value);
            }

            return result;
        }

        void ITypeSerializer<Dictionary<TKey, TValue>>.Serialize(Dictionary<TKey, TValue> obj, ref StreamingWriter writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer.WriteValue(count, NumberFormat.Variant);

            if (count == -1)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                writer.WriteValue<TKey>(pair.Key);
                writer.WriteValue<TValue>(pair.Value);
            }
        }
    }

    internal sealed class DictionarySerializeProxy<T, TKey, TValue> : ITypeSerializer<T> where T : IDictionary<TKey, TValue>, new()
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
                TKey key = iterator.NextAsWithoutTag<TKey>();
                TValue value = iterator.HasNext() ? iterator.NextAsWithoutTag<TValue>() : default;
                result.Add(key, value);
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

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                writer.WriteValue<TKey>(pair.Key);
                writer.WriteValue<TValue>(pair.Value);
            }
        }
    }
}
