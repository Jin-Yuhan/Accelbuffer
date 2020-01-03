using UnityEngine;

namespace Accelbuffer
{
    internal sealed class RectSerializeProxy : ISerializeProxy<Rect>, ISerializeProxy<RectInt>
    {
        Rect ISerializeProxy<Rect>.Deserialize(ref UnmanagedReader reader)
        {
            return new Rect(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0));
        }

        RectInt ISerializeProxy<RectInt>.Deserialize(ref UnmanagedReader reader)
        {
            return new RectInt(reader.ReadVariableInt32(0), reader.ReadVariableInt32(0), reader.ReadVariableInt32(0), reader.ReadVariableInt32(0));
        }

        void ISerializeProxy<Rect>.Serialize(Rect obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.x, Number.Var);
            writer.WriteValue(0, obj.y, Number.Var);
            writer.WriteValue(0, obj.width, Number.Var);
            writer.WriteValue(0, obj.height, Number.Var);
        }

        void ISerializeProxy<RectInt>.Serialize(RectInt obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.xMin, Number.Var);
            writer.WriteValue(0, obj.yMin, Number.Var);
            writer.WriteValue(0, obj.width, Number.Var);
            writer.WriteValue(0, obj.height, Number.Var);
        }
    }
}
