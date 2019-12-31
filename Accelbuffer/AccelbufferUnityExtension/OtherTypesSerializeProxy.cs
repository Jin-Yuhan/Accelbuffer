using UnityEngine;

namespace Accelbuffer
{
    internal sealed class OtherTypesSerializeProxy : ISerializeProxy<Quaternion>, ISerializeProxy<Matrix4x4>, ISerializeProxy<LayerMask>
    {
        unsafe Quaternion ISerializeProxy<Quaternion>.Deserialize(in UnmanagedReader* reader)
        {
            return new Quaternion(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0));
        }

        unsafe Matrix4x4 ISerializeProxy<Matrix4x4>.Deserialize(in UnmanagedReader* reader)
        {
            return new Matrix4x4
            (
                new Vector4(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0)),
                new Vector4(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0)),
                new Vector4(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0)),
                new Vector4(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0))
            );
        }

        unsafe LayerMask ISerializeProxy<LayerMask>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadVariableInt32(0);
        }

        unsafe void ISerializeProxy<Quaternion>.Serialize(in Quaternion obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, Number.Var);
            writer->WriteValue(0, obj.y, Number.Var);
            writer->WriteValue(0, obj.z, Number.Var);
            writer->WriteValue(0, obj.w, Number.Var);
        }

        unsafe void ISerializeProxy<Matrix4x4>.Serialize(in Matrix4x4 obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.m00, Number.Var);
            writer->WriteValue(0, obj.m10, Number.Var);
            writer->WriteValue(0, obj.m20, Number.Var);
            writer->WriteValue(0, obj.m30, Number.Var);

            writer->WriteValue(0, obj.m01, Number.Var);
            writer->WriteValue(0, obj.m11, Number.Var);
            writer->WriteValue(0, obj.m21, Number.Var);
            writer->WriteValue(0, obj.m31, Number.Var);

            writer->WriteValue(0, obj.m02, Number.Var);
            writer->WriteValue(0, obj.m12, Number.Var);
            writer->WriteValue(0, obj.m22, Number.Var);
            writer->WriteValue(0, obj.m32, Number.Var);

            writer->WriteValue(0, obj.m03, Number.Var);
            writer->WriteValue(0, obj.m13, Number.Var);
            writer->WriteValue(0, obj.m23, Number.Var);
            writer->WriteValue(0, obj.m33, Number.Var);
        }

        unsafe void ISerializeProxy<LayerMask>.Serialize(in LayerMask obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, Number.Var);
        }
    }
}
