namespace Accelbuffer.Injection
{
    internal sealed class NullableSerializer<T> : ITypeSerializer<T?> where T : struct
    {
        T? ITypeSerializer<T?>.Deserialize(ref AccelReader reader)
        {
            if (reader.HasNext() && reader.ReadBoolean())
            {
                return reader.HasNext() ? reader.ReadGeneric<T>() : default;
            }

            return null;
        }

        void ITypeSerializer<T?>.Serialize(T? obj, ref AccelWriter writer)
        {
            writer.WriteValue(1, obj.HasValue);
            writer.WriteValue(2, obj.GetValueOrDefault());
        }
    }
}
