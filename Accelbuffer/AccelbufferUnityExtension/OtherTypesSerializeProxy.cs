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
            writer->WriteValue(0, obj.x, NumberOption.VariableLength);
            writer->WriteValue(0, obj.y, NumberOption.VariableLength);
            writer->WriteValue(0, obj.z, NumberOption.VariableLength);
            writer->WriteValue(0, obj.w, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<Matrix4x4>.Serialize(in Matrix4x4 obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.m00, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m10, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m20, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m30, NumberOption.VariableLength);

            writer->WriteValue(0, obj.m01, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m11, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m21, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m31, NumberOption.VariableLength);

            writer->WriteValue(0, obj.m02, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m12, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m22, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m32, NumberOption.VariableLength);

            writer->WriteValue(0, obj.m03, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m13, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m23, NumberOption.VariableLength);
            writer->WriteValue(0, obj.m33, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<LayerMask>.Serialize(in LayerMask obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, NumberOption.VariableLength);
        }
    }
}
