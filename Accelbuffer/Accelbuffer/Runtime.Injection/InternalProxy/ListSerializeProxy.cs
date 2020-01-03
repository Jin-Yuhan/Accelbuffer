using System.Collections.Generic;

namespace Accelbuffer.Runtime.Injection
{
    internal sealed class ListSerializeProxy<T> : ISerializeProxy<List<T>>
    {
        List<T> ISerializeProxy<List<T>>.Deserialize(ref UnmanagedReader reader)
        {
            int count = reader.ReadVariableInt32(0);

            if (count == -1)
            {
                return null;
            }

            List<T> result = new List<T>(count);

            while (count > 0)
            {
                result.Add(Serializer<T>.Deserialize(ref reader));
                count--;
            }

            return result;
        }

        void ISerializeProxy<List<T>>.Serialize(List<T> obj, ref UnmanagedWriter writer)
        {
            int count = obj == null ? -1 : obj.Count;
            writer.WriteValue(0, count, Number.Var);

            for (int i = 0; i < count; i++)
            {
                Serializer<T>.Serialize(obj[i], ref writer);
            }
        }
    }

    internal sealed class ListSerializeProxy<T, TValue> : ISerializeProxy<T> where T : IList<TValue>, new()
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
                result.Add(Serializer<TValue>.Deserialize(ref reader));
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

            for (int i = 0; i < obj.Count; i++)
            {
                Serializer<TValue>.Serialize(obj[i], ref writer);
            }
        }
    }
}
