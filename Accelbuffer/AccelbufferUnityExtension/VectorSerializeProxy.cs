using UnityEngine;

namespace Accelbuffer
{
    internal sealed class VectorSerializeProxy : ISerializeProxy<Vector2>, 
                                                 ISerializeProxy<Vector3>, 
                                                 ISerializeProxy<Vector4>, 
                                                 ISerializeProxy<Vector2Int>, 
                                                 ISerializeProxy<Vector3Int>
    {
        unsafe Vector2 ISerializeProxy<Vector2>.Deserialize(in UnmanagedReader* reader)
        {
            return new Vector2(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0));
        }

        unsafe Vector3 ISerializeProxy<Vector3>.Deserialize(in UnmanagedReader* reader)
        {
            return new Vector3(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0));
        }

        unsafe Vector4 ISerializeProxy<Vector4>.Deserialize(in UnmanagedReader* reader)
        {
            return new Vector4(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0));
        }

        unsafe Vector2Int ISerializeProxy<Vector2Int>.Deserialize(in UnmanagedReader* reader)
        {
            return new Vector2Int(reader->ReadVariableInt32(0), reader->ReadVariableInt32(0));
        }

        unsafe Vector3Int ISerializeProxy<Vector3Int>.Deserialize(in UnmanagedReader* reader)
        {
            return new Vector3Int(reader->ReadVariableInt32(0), reader->ReadVariableInt32(0), reader->ReadVariableInt32(0));
        }

        unsafe void ISerializeProxy<Vector2>.Serialize(in Vector2 obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, NumberOption.VariableLength);
            writer->WriteValue(0, obj.y, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<Vector3>.Serialize(in Vector3 obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, NumberOption.VariableLength);
            writer->WriteValue(0, obj.y, NumberOption.VariableLength);
            writer->WriteValue(0, obj.z, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<Vector4>.Serialize(in Vector4 obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, NumberOption.VariableLength);
            writer->WriteValue(0, obj.y, NumberOption.VariableLength);
            writer->WriteValue(0, obj.z, NumberOption.VariableLength);
            writer->WriteValue(0, obj.w, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<Vector2Int>.Serialize(in Vector2Int obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, NumberOption.VariableLength);
            writer->WriteValue(0, obj.y, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<Vector3Int>.Serialize(in Vector3Int obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, NumberOption.VariableLength);
            writer->WriteValue(0, obj.y, NumberOption.VariableLength);
            writer->WriteValue(0, obj.z, NumberOption.VariableLength);
        }
    }
}
