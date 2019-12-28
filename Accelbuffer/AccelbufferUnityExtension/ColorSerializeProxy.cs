using UnityEngine;

namespace Accelbuffer
{
    internal sealed class ColorSerializeProxy : ISerializeProxy<Color>, ISerializeProxy<Color32>
    {
        unsafe Color ISerializeProxy<Color>.Deserialize(in UnmanagedReader* reader)
        {
            return new Color(reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0), reader->ReadVariableFloat32(0));
        }

        unsafe Color32 ISerializeProxy<Color32>.Deserialize(in UnmanagedReader* reader)
        {
            return new Color32(reader->ReadVariableUInt8(0), reader->ReadVariableUInt8(0), reader->ReadVariableUInt8(0), reader->ReadVariableUInt8(0));
        }

        unsafe void ISerializeProxy<Color>.Serialize(in Color obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.r, NumberOption.VariableLength);
            writer->WriteValue(0, obj.g, NumberOption.VariableLength);
            writer->WriteValue(0, obj.b, NumberOption.VariableLength);
            writer->WriteValue(0, obj.a, NumberOption.VariableLength);
        }

        unsafe void ISerializeProxy<Color32>.Serialize(in Color32 obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj.r, NumberOption.VariableLength);
            writer->WriteValue(0, obj.g, NumberOption.VariableLength);
            writer->WriteValue(0, obj.b, NumberOption.VariableLength);
            writer->WriteValue(0, obj.a, NumberOption.VariableLength);
        }
    }
}
