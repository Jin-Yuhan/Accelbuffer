namespace Accelbuffer.Runtime.Injection
{
    internal sealed class ArraySerializeProxy<T> : ISerializeProxy<T[]>
    {
        T[] ISerializeProxy<T[]>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            int len = reader.ReadInt32(0, Number.Var);

            if (len == -1)
            {
                return null;
            }

            T[] result = new T[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Serializer<T>.Deserialize(ref reader, context);
            }

            return result;
        }

        void ISerializeProxy<T[]>.Serialize(T[] obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            int count = obj == null ? -1 : obj.Length;
            writer.WriteValue(0, count, Number.Var);

            for (int i = 0; i < count; i++)
            {
                Serializer<T>.Serialize(obj[i], ref writer, context);
            }
        }
    }
}
