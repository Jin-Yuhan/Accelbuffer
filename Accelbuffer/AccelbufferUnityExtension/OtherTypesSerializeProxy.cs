using UnityEngine;

namespace Accelbuffer
{
    internal sealed class OtherTypesSerializeProxy : ITypeSerializer<Quaternion>, ITypeSerializer<Matrix4x4>, ITypeSerializer<LayerMask>
    {
        Quaternion ITypeSerializer<Quaternion>.Deserialize(ref StreamingIterator iterator)
        {
            return new Quaternion(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant));
        }

        Matrix4x4 ITypeSerializer<Matrix4x4>.Deserialize(ref StreamingIterator iterator)
        {
            return new Matrix4x4
            (
                new Vector4(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant)),
                new Vector4(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant)),
                new Vector4(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant)),
                new Vector4(reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant), reader.ReadFloat32(0, NumberFormat.Variant))
            );
        }

        LayerMask ITypeSerializer<LayerMask>.Deserialize(ref StreamingIterator iterator)
        {
            return reader.ReadInt32(0, NumberFormat.Variant);
        }

        void ITypeSerializer<Quaternion>.Serialize(Quaternion obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.z, NumberFormat.Variant);
            writer.WriteValue(0, obj.w, NumberFormat.Variant);
        }

        void ITypeSerializer<Matrix4x4>.Serialize(Matrix4x4 obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj.m00, NumberFormat.Variant);
            writer.WriteValue(0, obj.m10, NumberFormat.Variant);
            writer.WriteValue(0, obj.m20, NumberFormat.Variant);
            writer.WriteValue(0, obj.m30, NumberFormat.Variant);

            writer.WriteValue(0, obj.m01, NumberFormat.Variant);
            writer.WriteValue(0, obj.m11, NumberFormat.Variant);
            writer.WriteValue(0, obj.m21, NumberFormat.Variant);
            writer.WriteValue(0, obj.m31, NumberFormat.Variant);

            writer.WriteValue(0, obj.m02, NumberFormat.Variant);
            writer.WriteValue(0, obj.m12, NumberFormat.Variant);
            writer.WriteValue(0, obj.m22, NumberFormat.Variant);
            writer.WriteValue(0, obj.m32, NumberFormat.Variant);

            writer.WriteValue(0, obj.m03, NumberFormat.Variant);
            writer.WriteValue(0, obj.m13, NumberFormat.Variant);
            writer.WriteValue(0, obj.m23, NumberFormat.Variant);
            writer.WriteValue(0, obj.m33, NumberFormat.Variant);
        }

        void ITypeSerializer<LayerMask>.Serialize(LayerMask obj, ref StreamingWriter writer, StreamingContext context)
        {
            writer.WriteValue(0, obj, NumberFormat.Variant);
        }
    }
}
