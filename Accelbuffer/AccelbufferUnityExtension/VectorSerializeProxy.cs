﻿using UnityEngine;

namespace Accelbuffer
{
    internal sealed class VectorSerializeProxy : ISerializeProxy<Vector2>, 
                                                 ISerializeProxy<Vector3>, 
                                                 ISerializeProxy<Vector4>, 
                                                 ISerializeProxy<Vector2Int>, 
                                                 ISerializeProxy<Vector3Int>
    {
        Vector2 ISerializeProxy<Vector2>.Deserialize(ref UnmanagedReader reader)
        {
            return new Vector2(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0));
        }

        Vector3 ISerializeProxy<Vector3>.Deserialize(ref UnmanagedReader reader)
        {
            return new Vector3(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0));
        }

        Vector4 ISerializeProxy<Vector4>.Deserialize(ref UnmanagedReader reader)
        {
            return new Vector4(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0));
        }

        Vector2Int ISerializeProxy<Vector2Int>.Deserialize(ref UnmanagedReader reader)
        {
            return new Vector2Int(reader.ReadVariableInt32(0), reader.ReadVariableInt32(0));
        }

        Vector3Int ISerializeProxy<Vector3Int>.Deserialize(ref UnmanagedReader reader)
        {
            return new Vector3Int(reader.ReadVariableInt32(0), reader.ReadVariableInt32(0), reader.ReadVariableInt32(0));
        }

        void ISerializeProxy<Vector2>.Serialize(Vector2 obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
        }

        void ISerializeProxy<Vector3>.Serialize(Vector3 obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
            writer.WriteValue(0, obj.z, Number.Var);
        }

        void ISerializeProxy<Vector4>.Serialize(Vector4 obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
            writer.WriteValue(0, obj.z, Number.Var);
            writer.WriteValue(0, obj.w, Number.Var);
        }

        void ISerializeProxy<Vector2Int>.Serialize(Vector2Int obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
        }

        void ISerializeProxy<Vector3Int>.Serialize(Vector3Int obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
            writer.WriteValue(0, obj.z, Number.Var);
        }
    }
}
