using UnityEngine;

namespace Accelbuffer
{
    internal sealed class ColorSerializeProxy : ISerializeProxy<Color>, ISerializeProxy<Color32>
    {
        Color ISerializeProxy<Color>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return new Color(reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var), reader.ReadFloat32(0, Number.Var));
        }

        Color32 ISerializeProxy<Color32>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
        {
            return new Color32(reader.ReadUInt8(0, Number.Var), reader.ReadUInt8(0, Number.Var), reader.ReadUInt8(0, Number.Var), reader.ReadUInt8(0, Number.Var));
        }

        void ISerializeProxy<Color>.Serialize(Color obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj.r, Number.Var);
            writer.WriteValue(0, obj.g, Number.Var);
            writer.WriteValue(0, obj.b, Number.Var);
            writer.WriteValue(0, obj.a, Number.Var);
        }

        void ISerializeProxy<Color32>.Serialize(Color32 obj, ref UnmanagedWriter writer, SerializationContext context)
        {
            writer.WriteValue(0, obj.r, Number.Var);
            writer.WriteValue(0, obj.g, Number.Var);
            writer.WriteValue(0, obj.b, Number.Var);
            writer.WriteValue(0, obj.a, Number.Var);
        }
    }
}
