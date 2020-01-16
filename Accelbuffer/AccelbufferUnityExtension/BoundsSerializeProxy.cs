using UnityEngine;

namespace Accelbuffer
{
    internal sealed class BoundsSerializeProxy : ITypeSerializer<Bounds>, ITypeSerializer<BoundsInt>
    {
        Bounds ITypeSerializer<Bounds>.Deserialize(ref StreamingIterator iterator)
        {
            return new Bounds
            (
                new Vector3(iterator.NextAsFloat32WithoutTag(), iterator.NextAsFloat32WithoutTag(), iterator.NextAsFloat32WithoutTag()),
                new Vector3(iterator.NextAsFloat32WithoutTag(), iterator.NextAsFloat32WithoutTag(), iterator.NextAsFloat32WithoutTag())
            );
        }

        BoundsInt ITypeSerializer<BoundsInt>.Deserialize(ref StreamingIterator iterator)
        {
            return new BoundsInt
            (
                new Vector3Int(iterator.NextAsInt32WithoutTag(NumberFormat.Variant), iterator.NextAsInt32WithoutTag(NumberFormat.Variant), iterator.NextAsInt32WithoutTag(NumberFormat.Variant)),
                new Vector3Int(iterator.NextAsInt32WithoutTag(NumberFormat.Variant), iterator.NextAsInt32WithoutTag(NumberFormat.Variant), iterator.NextAsInt32WithoutTag(NumberFormat.Variant))
            );
        }

        void ITypeSerializer<Bounds>.Serialize(Bounds obj, ref StreamingWriter writer)
        {
            writer.WriteValue(0, obj.center.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.center.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.center.z, NumberFormat.Variant);

            writer.WriteValue(0, obj.size.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.size.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.size.z, NumberFormat.Variant);
        }

        void ITypeSerializer<BoundsInt>.Serialize(BoundsInt obj, ref StreamingWriter writer)
        {
            writer.WriteValue(0, obj.position.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.position.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.position.z, NumberFormat.Variant);

            writer.WriteValue(0, obj.size.x, NumberFormat.Variant);
            writer.WriteValue(0, obj.size.y, NumberFormat.Variant);
            writer.WriteValue(0, obj.size.z, NumberFormat.Variant);
        }
    }
}
