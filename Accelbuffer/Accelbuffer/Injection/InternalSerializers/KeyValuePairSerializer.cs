using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class KeyValuePairSerializer<TKey, TValue> : ITypeSerializer<KeyValuePair<TKey, TValue>>
    {
        KeyValuePair<TKey, TValue> ITypeSerializer<KeyValuePair<TKey, TValue>>.Deserialize(ref AccelReader reader)
        {
            TKey key = reader.HasNext() ? reader.ReadGeneric<TKey>() : default;
            TValue value = reader.HasNext() ? reader.ReadGeneric<TValue>() : default;
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        void ITypeSerializer<KeyValuePair<TKey, TValue>>.Serialize(KeyValuePair<TKey, TValue> obj, ref AccelWriter writer)
        {
            writer.WriteValue<TKey>(2, obj.Key);
            writer.WriteValue<TValue>(2, obj.Value);
        }
    }
}
