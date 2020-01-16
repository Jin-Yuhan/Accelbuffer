using UnityEngine;

namespace Accelbuffer
{
    internal sealed class RectSerializeProxy : ITypeSerializer<Rect>, ITypeSerializer<RectInt>
    {
        Rect ITypeSerializer<Rect>.Deserialize(ref StreamingIterator iterator)
        {
            return new Rect(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant));
        }

        RectInt ITypeSerializer<RectInt>.Deserialize(ref StreamingIterator iterator)
        {
            return new RectInt(reader.ReadInt32(0, NumberFormat.Variant), reader.ReadInt32(0, NumberFormat.Variant), reader.ReadInt32(0, NumberFormat.Variant), reader.ReadInt32(0, NumberFormat.Variant));
        }

        void ITypeSerializer<Rect>.Serialize(Rect obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.width, NumberFormat.Variant);
            writer.WriteValue(0, obj.height, NumberFormat.Variant);
        }

        void ITypeSerializer<RectInt>.Serialize(RectInt obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.xMin, NumberFormat.Variant);
            writer.WriteValue(0, obj.yMin, NumberFormat.Variant);
            writer.WriteValue(0, obj.width, NumberFormat.Variant);
            writer.WriteValue(0, obj.height, NumberFormat.Variant);
        }
    }
}
