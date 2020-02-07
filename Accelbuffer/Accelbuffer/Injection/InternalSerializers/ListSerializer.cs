using System.Collections.Generic;

namespace Accelbuffer.Injection
{
    internal sealed class ListSerializer<T> : ITypeSerializer<List<T>>
    {
        List<T> ITypeSerializer<List<T>>.Deserialize(ref AccelReader reader)
        {
            List<T> result = new List<T>();

            while (reader.HasNext())
            {
                T value = reader.ReadGeneric<T>();
                result.Add(value);
            }

            return result;
        }

        void ITypeSerializer<List<T>>.Serialize(List<T> obj, ref AccelWriter writer)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                writer.WriteValue(1, obj[i]);
            }
        }
    }

    internal sealed class ListSerializer<T, TValue> : ITypeSerializer<T> where T : IList<TValue>, new()
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
            for (int i = 0; i < obj.Count; i++)
            {
                writer.WriteValue(1, obj[i]);
            }
        }
    }
}
