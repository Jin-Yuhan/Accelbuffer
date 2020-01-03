using UnityEngine;

namespace Accelbuffer
{
    internal sealed class OtherTypesSerializeProxy : ISerializeProxy<Quaternion>, ISerializeProxy<Matrix4x4>, ISerializeProxy<LayerMask>
    {
        Quaternion ISerializeProxy<Quaternion>.Deserialize(ref UnmanagedReader reader)
        {
            return new Quaternion(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0));
        }

        Matrix4x4 ISerializeProxy<Matrix4x4>.Deserialize(ref UnmanagedReader reader)
        {
            return new Matrix4x4
            (
                new Vector4(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0)),
                new Vector4(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0)),
                new Vector4(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0)),
                new Vector4(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0))
            );
        }

        LayerMask ISerializeProxy<LayerMask>.Deserialize(ref UnmanagedReader reader)
        {
            return reader.ReadVariableInt32(0);
        }

        void ISerializeProxy<Quaternion>.Serialize(Quaternion obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
            writer.WriteValue(0, obj.z, Number.Var);
            writer.WriteValue(0, obj.w, Number.Var);
        }

        void ISerializeProxy<Matrix4x4>.Serialize(Matrix4x4 obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.m00, Number.Var);
            writer.WriteValue(0, obj.m10, Number.Var);
            writer.WriteValue(0, obj.m20, Number.Var);
            writer.WriteValue(0, obj.m30, Number.Var);

            writer.WriteValue(0, obj.m01, Number.Var);
            writer.WriteValue(0, obj.m11, Number.Var);
            writer.WriteValue(0, obj.m21, Number.Var);
            writer.WriteValue(0, obj.m31, Number.Var);

            writer.WriteValue(0, obj.m02, Number.Var);
            writer.WriteValue(0, obj.m12, Number.Var);
            writer.WriteValue(0, obj.m22, Number.Var);
            writer.WriteValue(0, obj.m32, Number.Var);

            writer.WriteValue(0, obj.m03, Number.Var);
            writer.WriteValue(0, obj.m13, Number.Var);
            writer.WriteValue(0, obj.m23, Number.Var);
            writer.WriteValue(0, obj.m33, Number.Var);
        }

        void ISerializeProxy<LayerMask>.Serialize(LayerMask obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj, Number.Var);
        }
    }
}
