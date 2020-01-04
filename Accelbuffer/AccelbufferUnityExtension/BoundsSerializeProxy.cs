using UnityEngine;

namespace Accelbuffer
{
    internal sealed class BoundsSerializeProxy : ISerializeProxy<Bounds>, ISerializeProxy<BoundsInt>
    {
        Bounds ISerializeProxy<Bounds>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return new Bounds
            (
                new Vector3(reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var)),
                new Vector3(reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var))
            );
        }

        BoundsInt ISerializeProxy<BoundsInt>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return new BoundsInt
            (
                new Vector3Int(reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var)),
                new Vector3Int(reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var))
            );
        }

        void ISerializeProxy<Bounds>.Serialize(Bounds obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj.center.x, Number.Var);
            writer.WriteValue(0, obj.center.y, Number.Var);
            writer.WriteValue(0, obj.center.z, Number.Var);

            writer.WriteValue(0, obj.size.x, Number.Var);
            writer.WriteValue(0, obj.size.y, Number.Var);
            writer.WriteValue(0, obj.size.z, Number.Var);
        }

        void ISerializeProxy<BoundsInt>.Serialize(BoundsInt obj, ref UnmanagedWriter writer, SerializationContext context)
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
