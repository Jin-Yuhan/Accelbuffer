namespace Accelbuffer.Injection
{
    internal sealed class ArraySerializer<T> : ITypeSerializer<T[]>
    {
        T[] ITypeSerializer<T[]>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext(out int index))
            {
                return null;
            }

            int len = (int)reader.ReadVariantUInt();
            T[] result = new T[len];

            for (int i = 0; reader.HasNext(); i++)
            {
                result[i] = reader.ReadGeneric<T>();
            }

            return result;
        }

        void ITypeSerializer<T[]>.Serialize(T[] obj, ref AccelWriter writer)
        {
            writer.WriteValue(1, (VUInt)obj.Length);

            for (int i = 0; i < obj.Length; i++)
            {
                writer.WriteValue<T>(2, obj[i]);
            }
        }
    }
}
