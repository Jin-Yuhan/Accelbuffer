namespace Accelbuffer.Injection
{
    internal sealed class ArraySerializer<T> : ITypeSerializer<T[]>
    {
        T[] ITypeSerializer<T[]>.Deserialize(ref StreamingIterator iterator)
        {
            int len = iterator.HasNext() ? iterator.NextAsInt32WithoutTag(NumberFormat.Variant) : 0;

            if (len == -1)
            {
                return null;
            }

            T[] result = new T[len];

            for (int i = 0; i < len && iterator.HasNext(); i++)
            {
                result[i] = iterator.NextAsWithoutTag<T>();
            }

            return result;
        }

        void ITypeSerializer<T[]>.Serialize(T[] obj, ref StreamingWriter writer)
        {
            int count = obj == null ? -1 : obj.Length;
            writer.WriteValue(count, NumberFormat.Variant);

            for (int i = 0; i < count; i++)
            {
                writer.WriteValue(obj[i]);
            }
        }
    }
}
