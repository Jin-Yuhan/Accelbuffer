using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class CollectionSerializer<T, TValue> : ITypeSerializer<T> where T : ICollection<TValue>, new()
    {
        T ITypeSerializer<T>.Deserialize(ref AccelReader reader)
        {
            T result = new T();

            while (reader.HasNext())
            {
                TValue value = reader.ReadGeneric<TValue>();
                result.Add(value);
            }

            return result;
        }

        void ITypeSerializer<T>.Serialize(T obj, ref AccelWriter writer)
        {
            foreach (TValue o in obj)
            {
                writer.WriteValue<TValue>(1, o);
            }
        }
    }
}
