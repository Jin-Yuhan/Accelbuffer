using UnityEngine;

namespace Accelbuffer
{
    internal sealed class RectSerializeProxy : ISerializeProxy<Rect>, ISerializeProxy<RectInt>
    {
        Rect ISerializeProxy<Rect>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return new Rect(reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var));
        }

        RectInt ISerializeProxy<RectInt>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return new RectInt(reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var), reader.ReadInt32(0, Number.Var));
        }

        void ISerializeProxy<Rect>.Serialize(Rect obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
            writer.WriteValue(0, obj.width, Number.Var);
            writer.WriteValue(0, obj.height, Number.Var);
        }

        void ISerializeProxy<RectInt>.Serialize(RectInt obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj.xMin, Number.Var);
            writer.WriteValue(0, obj.yMin, Number.Var);
            writer.WriteValue(0, obj.width, Number.Var);
            writer.WriteValue(0, obj.height, Number.Var);
        }
    }
}
