using System.Collections.Generic;

namespace Accelbuffer
{
    internal sealed class DictionarySerializeProxy<TKey, TValue> : ISerializeProxy<Dictionary<TKey, TValue>>
    {
        unsafe Dictionary<TKey, TValue> ISerializeProxy<Dictionary<TKey, TValue>>.Deserialize(in UnmanagedReader* reader)
        {
            int count = reader->ReadVariableInt32(0);

            if (count == -1)
            {
                return null;
            }

            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(count);

            while (count > 0)
            {
                TKey key = Serializer<TKey>.Deserialize(reader);
                TValue value = Serializer<TValue>.Deserialize(reader);
                result.Add(key, value);
                count--;
            }

            return result;
        }

        unsafe void ISerializeProxy<Dictionary<TKey, TValue>>.Serialize(in Dictionary<TKey, TValue> obj, in UnmanagedWriter* writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer->WriteValue(0, count, Number.Var);

            if (count == -1)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                Serializer<TKey>.Serialize(pair.Key, writer);
                Serializer<TValue>.Serialize(pair.Value, writer);
            }
        }
    }

    internal sealed class DictionarySerializeProxy<T, TKey, TValue> : ISerializeProxy<T> where T : IDictionary<TKey, TValue>, new()
    {
        unsafe T ISerializeProxy<T>.Deserialize(in UnmanagedReader* reader)
        {
            int count = reader->ReadVariableInt32(0);

            if (count == -1)
            {
                return default;
            }

            T result = new T();

            while (count > 0)
            {
                TKey key = Serializer<TKey>.Deserialize(reader);
                TValue value = Serializer<TValue>.Deserialize(reader);
                result.Add(key, value);
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

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                Serializer<TKey>.Serialize(pair.Key, writer);
                Serializer<TValue>.Serialize(pair.Value, writer);
            }
        }
    }
}
