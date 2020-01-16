using UnityEngine;

namespace Accelbuffer
{
    internal sealed class VectorSerializeProxy : ITypeSerializer<Vector2>, 
                                                 ITypeSerializer<Vector3>, 
                                                 ITypeSerializer<Vector4>, 
                                                 ITypeSerializer<Vector2Int>, 
                                                 ITypeSerializer<Vector3Int>
    {
        Vector2 ITypeSerializer<Vector2>.Deserialize(ref StreamingIterator iterator)
        {
            return new Vector2(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant));
        }

        Vector3 ITypeSerializer<Vector3>.Deserialize(ref StreamingIterator iterator)
        {
            return new Vector3(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant));
        }

        Vector4 ITypeSerializer<Vector4>.Deserialize(ref StreamingIterator iterator)
        {
            return new Vector4(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant));
        }

        Vector2Int ITypeSerializer<Vector2Int>.Deserialize(ref StreamingIterator iterator)
        {
            return new Vector2Int(reader.ReadInt32(0, NumberFormat.Variant), reader.ReadInt32(0, NumberFormat.Variant));
        }

        Vector3Int ITypeSerializer<Vector3Int>.Deserialize(ref StreamingIterator iterator)
        {
            return new Vector3Int(reader.ReadInt32(0, NumberFormat.Variant), reader.ReadInt32(0, NumberFormat.Variant), reader.ReadInt32(0, NumberFormat.Variant));
        }

        void ITypeSerializer<Vector2>.Serialize(Vector2 obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
        }

        void ITypeSerializer<Vector3>.Serialize(Vector3 obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.z, NumberFormat.Variant);
        }

        void ITypeSerializer<Vector4>.Serialize(Vector4 obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.z, NumberFormat.Variant);
            writer.WriteValue(0, obj.w, NumberFormat.Variant);
        }

        void ITypeSerializer<Vector2Int>.Serialize(Vector2Int obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
        }

        void ITypeSerializer<Vector3Int>.Serialize(Vector3Int obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.z, NumberFormat.Variant);
        }
    }
}
