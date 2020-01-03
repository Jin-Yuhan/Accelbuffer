namespace Accelbuffer.Runtime.Injection
{
    internal sealed class ArraySerializeProxy<T> : ISerializeProxy<T[]>
    {
        T[] ISerializeProxy<T[]>.Deserialize(ref UnmanagedReader reader)
        {
            int len = reader.ReadVariableInt32(0);

            if (len == -1)
            {
                return null;
            }

            T[] result = new T[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Serializer<T>.Deserialize(ref reader);
            }

            return result;
        }

        void ISerializeProxy<T[]>.Serialize(T[] obj, ref UnmanagedWriter writer)
        {
            int count = obj == null ? -1 : obj.Length;
            writer.WriteValue(0, count, Number.Var);

            for (int i = 0; i < count; i++)
            {
                Serializer<T>.Serialize(obj[i], ref writer);
            }
        }
    }
}
