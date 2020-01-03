using System.Collections.Generic;

namespace Accelbuffer.Runtime.Injection
{
    internal sealed class DictionarySerializeProxy<TKey, TValue> : ISerializeProxy<Dictionary<TKey, TValue>>
    {
        Dictionary<TKey, TValue> ISerializeProxy<Dictionary<TKey, TValue>>.Deserialize(ref UnmanagedReader reader)
        {
            int count = reader.ReadVariableInt32(0);

            if (count == -1)
            {
                return null;
            }

            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(count);

            while (count > 0)
            {
                TKey key = Serializer<TKey>.Deserialize(ref reader);
                TValue value = Serializer<TValue>.Deserialize(ref reader);
                result.Add(key, value);
                count--;
            }

            return result;
        }

        void ISerializeProxy<Dictionary<TKey, TValue>>.Serialize(Dictionary<TKey, TValue> obj, ref UnmanagedWriter writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer.WriteValue(0, count, Number.Var);

            if (count == -1)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                Serializer<TKey>.Serialize(pair.Key, ref writer);
                Serializer<TValue>.Serialize(pair.Value, ref writer);
            }
        }
    }

    internal sealed class DictionarySerializeProxy<T, TKey, TValue> : ISerializeProxy<T> where T : IDictionary<TKey, TValue>, new()
    {
        T ISerializeProxy<T>.Deserialize(ref UnmanagedReader reader)
        {
            int count = reader.ReadVariableInt32(0);

            if (count == -1)
            {
                return default;
            }

            T result = new T();

            while (count > 0)
            {
                TKey key = Serializer<TKey>.Deserialize(ref reader);
                TValue value = Serializer<TValue>.Deserialize(ref reader);
                result.Add(key, value);
                count--;
            }

            return result;
        }

        void ISerializeProxy<T>.Serialize(T obj, ref UnmanagedWriter writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer.WriteValue(0, count, Number.Var);

            if (count == -1)
            {
                return;
            }

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                Serializer<TKey>.Serialize(pair.Key, ref writer);
                Serializer<TValue>.Serialize(pair.Value, ref writer);
            }
        }
    }
}
