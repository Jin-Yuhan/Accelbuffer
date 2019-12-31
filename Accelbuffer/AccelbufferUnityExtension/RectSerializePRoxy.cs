using UnityEngine;

namespace Accelbuffer
{
    internal sealed class RectSerializeProxy : ISerializeProxy<Rect>, ISerializeProxy<RectInt>
    {
        unsafe Rect ISerializeProxy<Rect>.Deserialize(in UnmanagedReader* reader)
        {
            return new Rect(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0));
        }

        unsafe RectInt ISerializeProxy<RectInt>.Deserialize(in UnmanagedReader* reader)
        {
            return new RectInt(reader->ReadVariableInt32(0), reader->ReadVariableInt32(0), reader->ReadVariableInt32(0), reader->ReadVariableInt32(0));
        }

        unsafe void ISerializeProxy<Rect>.Serialize(in Rect obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.x, Number.Var);
            writer->WriteValue(0, obj.y, Number.Var);
            writer->WriteValue(0, obj.width, Number.Var);
            writer->WriteValue(0, obj.height, Number.Var);
        }

        unsafe void ISerializeProxy<RectInt>.Serialize(in RectInt obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.xMin, Number.Var);
            writer->WriteValue(0, obj.yMin, Number.Var);
            writer->WriteValue(0, obj.width, Number.Var);
            writer->WriteValue(0, obj.height, Number.Var);
        }
    }
}
