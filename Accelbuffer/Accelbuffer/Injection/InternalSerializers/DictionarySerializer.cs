using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class DictionarySerializer<TKey, TValue> : ITypeSerializer<Dictionary<TKey, TValue>>
    {
        Dictionary<TKey, TValue> ITypeSerializer<Dictionary<TKey, TValue>>.Deserialize(ref AccelReader reader)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();

            while (reader.HasNext())
            {
                TKey key = reader.ReadGeneric<TKey>();
                TValue value = reader.HasNext() ? reader.ReadGeneric<TValue>() : default;
                result.Add(key, value);
            }

            return result;
        }

        void ITypeSerializer<Dictionary<TKey, TValue>>.Serialize(Dictionary<TKey, TValue> obj, ref AccelWriter writer)
        {
            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                writer.WriteValue<TKey>(1, pair.Key);
                writer.WriteValue<TValue>(1, pair.Value);
            }
        }
    }

    internal sealed class DictionarySerializer<T, TKey, TValue> : ITypeSerializer<T> where T : IDictionary<TKey, TValue>, new()
    {
        T ITypeSerializer<T>.Deserialize(ref AccelReader reader)
        {
            T result = new T();

            while (reader.HasNext())
            {
                TKey key = reader.ReadGeneric<TKey>();
                TValue value = reader.HasNext() ? reader.ReadGeneric<TValue>() : default;
                result.Add(key, value);
            }

            return result;
        }

        void ITypeSerializer<T>.Serialize(T obj, ref AccelWriter writer)
        {
            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                writer.WriteValue<TKey>(1, pair.Key);
                writer.WriteValue<TValue>(1, pair.Value);
            }
        }
    }
}
