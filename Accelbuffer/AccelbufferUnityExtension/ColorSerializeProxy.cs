using UnityEngine;

namespace Accelbuffer
{
    internal sealed class ColorSerializeProxy : ITypeSerializer<Color>, ITypeSerializer<Color32>
    {
        Color ITypeSerializer<Color>.Deserialize(ref StreamingIterator iterator)
        {
            return new Color(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant));
        }

        Color32 ITypeSerializer<Color32>.Deserialize(ref StreamingIterator iterator)
        {
            return new Color32(reader.ReadUInt8(0, NumberFormat.Variant), reader.ReadUInt8(0, NumberFormat.Variant), reader.ReadUInt8(0, NumberFormat.Variant), reader.ReadUInt8(0, NumberFormat.Variant));
        }

        void ITypeSerializer<Color>.Serialize(Color obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.r, NumberFormat.Variant);
            writer.WriteValue(0, obj.g, NumberFormat.Variant);
            writer.WriteValue(0, obj.b, NumberFormat.Variant);
            writer.WriteValue(0, obj.a, NumberFormat.Variant);
        }

        void ITypeSerializer<Color32>.Serialize(Color32 obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.r, NumberFormat.Variant);
            writer.WriteValue(0, obj.g, NumberFormat.Variant);
            writer.WriteValue(0, obj.b, NumberFormat.Variant);
            writer.WriteValue(0, obj.a, NumberFormat.Variant);
        }
    }
}
