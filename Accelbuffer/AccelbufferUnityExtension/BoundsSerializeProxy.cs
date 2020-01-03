using UnityEngine;

namespace Accelbuffer
{
    internal sealed class BoundsSerializeProxy : ISerializeProxy<Bounds>, ISerializeProxy<BoundsInt>
    {
        Bounds ISerializeProxy<Bounds>.Deserialize(ref UnmanagedReader reader)
        {
            return new Bounds
            (
                new Vector3(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0)),
                new Vector3(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0))
            );
        }

        BoundsInt ISerializeProxy<BoundsInt>.Deserialize(ref UnmanagedReader reader)
        {
            return new BoundsInt
            (
                new Vector3Int(reader.ReadVariableInt32(0), reader.ReadVariableInt32(0), reader.ReadVariableInt32(0)),
                new Vector3Int(reader.ReadVariableInt32(0), reader.ReadVariableInt32(0), reader.ReadVariableInt32(0))
            );
        }

        void ISerializeProxy<Bounds>.Serialize(Bounds obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.center.x, Number.Var);
            writer.WriteValue(0, obj.center.y, Number.Var);
            writer.WriteValue(0, obj.center.z, Number.Var);

            writer.WriteValue(0, obj.size.x, Number.Var);
            writer.WriteValue(0, obj.size.y, Number.Var);
            writer.WriteValue(0, obj.size.z, Number.Var);
        }

        void ISerializeProxy<BoundsInt>.Serialize(BoundsInt obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.position.x, Number.Var);
            writer.WriteValue(0, obj.position.y, Number.Var);
            writer.WriteValue(0, obj.position.z, Number.Var);

            writer.WriteValue(0, obj.size.x, Number.Var);
            writer.WriteValue(0, obj.size.y, Number.Var);
            writer.WriteValue(0, obj.size.z, Number.Var);
        }
    }
}
