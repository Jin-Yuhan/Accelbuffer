using UnityEngine;

namespace Accelbuffer
{
    internal sealed class BoundsSerializeProxy : ISerializeProxy<Bounds>, ISerializeProxy<BoundsInt>
    {
        unsafe Bounds ISerializeProxy<Bounds>.Deserialize(in UnmanagedReader* reader)
        {
            return new Bounds
            (
                new Vector3(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0)),
                new Vector3(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0))
            );
        }

        unsafe BoundsInt ISerializeProxy<BoundsInt>.Deserialize(in UnmanagedReader* reader)
        {
            return new BoundsInt
            (
                new Vector3Int(reader->ReadVariableInt32(0), reader->ReadVariableInt32(0), reader->ReadVariableInt32(0)),
                new Vector3Int(reader->ReadVariableInt32(0), reader->ReadVariableInt32(0), reader->ReadVariableInt32(0))
            );
        }

        unsafe void ISerializeProxy<Bounds>.Serialize(in Bounds obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.center.x, Number.Var);
            writer->WriteValue(0, obj.center.y, Number.Var);
            writer->WriteValue(0, obj.center.z, Number.Var);

            writer->WriteValue(0, obj.size.x, Number.Var);
            writer->WriteValue(0, obj.size.y, Number.Var);
            writer->WriteValue(0, obj.size.z, Number.Var);
        }

        unsafe void ISerializeProxy<BoundsInt>.Serialize(in BoundsInt obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.position.x, Number.Var);
            writer->WriteValue(0, obj.position.y, Number.Var);
            writer->WriteValue(0, obj.position.z, Number.Var);

            writer->WriteValue(0, obj.size.x, Number.Var);
            writer->WriteValue(0, obj.size.y, Number.Var);
            writer->WriteValue(0, obj.size.z, Number.Var);
        }
    }
}
