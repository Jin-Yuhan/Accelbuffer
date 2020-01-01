namespace Accelbuffer
{
    internal sealed class ArraySerializeProxy<T> : ISerializeProxy<T[]>
    {
        unsafe T[] ISerializeProxy<T[]>.Deserialize(in UnmanagedReader* reader)
        {
            int len = reader->ReadVariableInt32(0);

            if (len == -1)
            {
                return null;
            }

            T[] result = new T[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Serializer<T>.Deserialize(reader);
            }

            return result;
        }

        unsafe void ISerializeProxy<T[]>.Serialize(in T[] obj, in UnmanagedWriter* writer)
        {
            int count = obj == null ? -1 : obj.Length;
            writer->WriteValue(0, count, Number.Var);

            for (int i = 0; i < count; i++)
            {
                Serializer<T>.Serialize(obj[i], writer);
            }
        }
    }
}
