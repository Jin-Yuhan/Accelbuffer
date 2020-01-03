using UnityEngine;

namespace Accelbuffer
{
    internal sealed class ColorSerializeProxy : ISerializeProxy<Color>, ISerializeProxy<Color32>
    {
        Color ISerializeProxy<Color>.Deserialize(ref UnmanagedReader reader)
        {
            return new Color(reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0), reader.ReadVariableFloat32(0));
        }

        Color32 ISerializeProxy<Color32>.Deserialize(ref UnmanagedReader reader)
        {
            return new Color32(reader.ReadVariableUInt8(0), reader.ReadVariableUInt8(0), reader.ReadVariableUInt8(0), reader.ReadVariableUInt8(0));
        }

        void ISerializeProxy<Color>.Serialize(Color obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.r, Number.Var);
            writer.WriteValue(0, obj.g, Number.Var);
            writer.WriteValue(0, obj.b, Number.Var);
            writer.WriteValue(0, obj.a, Number.Var);
        }

        void ISerializeProxy<Color32>.Serialize(Color32 obj, ref UnmanagedWriter writer)
        {
            writer.WriteValue(0, obj.r, Number.Var);
            writer.WriteValue(0, obj.g, Number.Var);
            writer.WriteValue(0, obj.b, Number.Var);
            writer.WriteValue(0, obj.a, Number.Var);
        }
    }
}
