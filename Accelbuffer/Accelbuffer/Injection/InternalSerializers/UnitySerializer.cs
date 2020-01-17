#if UNITY
using UnityEngine;

namespace Accelbuffer.Injection
{
    internal sealed class UnitySerializer :
        ITypeSerializer<Vector2>,
        ITypeSerializer<Vector3>,
        ITypeSerializer<Vector4>,
        ITypeSerializer<Vector2Int>,
        ITypeSerializer<Vector3Int>,
        ITypeSerializer<Quaternion>
    {
        Vector2 ITypeSerializer<Vector2>.Deserialize(ref StreamingIterator iterator)
        {
            Vector2 result = new Vector2();
            int index = 0;

            while (iterator.HasNext())
            {
                switch (index++)
                {
                    case 0: result.x = iterator.NextAsFloat32WithoutTag(); break;
                    case 1: result.y = iterator.NextAsFloat32WithoutTag(); break;
                    default: return result;
                }
            }

            return result;
        }

        Vector3 ITypeSerializer<Vector3>.Deserialize(ref StreamingIterator iterator)
        {
            Vector3 result = new Vector3();
            int index = 0;

            while (iterator.HasNext())
            {
                switch (index++)
                {
                    case 0: result.x = iterator.NextAsFloat32WithoutTag(); break;
                    case 1: result.y = iterator.NextAsFloat32WithoutTag(); break;
                    case 2: result.z = iterator.NextAsFloat32WithoutTag(); break;
                    default: return result;
                }
            }

            return result;
        }

        Vector4 ITypeSerializer<Vector4>.Deserialize(ref StreamingIterator iterator)
        {
            Vector4 result = new Vector4();
            int index = 0;

            while (iterator.HasNext())
            {
                switch (index++)
                {
                    case 0: result.x = iterator.NextAsFloat32WithoutTag(); break;
                    case 1: result.y = iterator.NextAsFloat32WithoutTag(); break;
                    case 2: result.z = iterator.NextAsFloat32WithoutTag(); break;
                    case 3: result.w = iterator.NextAsFloat32WithoutTag(); break;
                    default: return result;
                }
            }

            return result;
        }

        Vector2Int ITypeSerializer<Vector2Int>.Deserialize(ref StreamingIterator iterator)
        {
            Vector2Int result = new Vector2Int();
            int index = 0;

            while (iterator.HasNext())
            {
                switch (index++)
                {
                    case 0: result.x = iterator.NextAsInt32WithoutTag(NumberFormat.Variant); break;
                    case 1: result.y = iterator.NextAsInt32WithoutTag(NumberFormat.Variant); break;
                    default: return result;
                }
            }

            return result;
        }

        Vector3Int ITypeSerializer<Vector3Int>.Deserialize(ref StreamingIterator iterator)
        {
            Vector3Int result = new Vector3Int();
            int index = 0;

            while (iterator.HasNext())
            {
                switch (index++)
                {
                    case 0: result.x = iterator.NextAsInt32WithoutTag(NumberFormat.Variant); break;
                    case 1: result.y = iterator.NextAsInt32WithoutTag(NumberFormat.Variant); break;
                    case 2: result.z = iterator.NextAsInt32WithoutTag(NumberFormat.Variant); break;
                    default: return result;
                }
            }

            return result;
        }

        Quaternion ITypeSerializer<Quaternion>.Deserialize(ref StreamingIterator iterator)
        {
            Quaternion result = new Quaternion();
            int index = 0;

            while (iterator.HasNext())
            {
                switch (index++)
                {
                    case 0: result.x = iterator.NextAsFloat32WithoutTag(); break;
                    case 1: result.y = iterator.NextAsFloat32WithoutTag(); break;
                    case 2: result.z = iterator.NextAsFloat32WithoutTag(); break;
                    case 3: result.w = iterator.NextAsFloat32WithoutTag(); break;
                    default: return result;
                }
            }

            return result;
        }

        void ITypeSerializer<Vector2>.Serialize(Vector2 obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.x);
            writer.WriteValue(obj.y);
        }

        void ITypeSerializer<Vector3>.Serialize(Vector3 obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.x);
            writer.WriteValue(obj.y);
            writer.WriteValue(obj.z);
        }

        void ITypeSerializer<Vector4>.Serialize(Vector4 obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.x);
            writer.WriteValue(obj.y);
            writer.WriteValue(obj.z);
            writer.WriteValue(obj.w);
        }

        void ITypeSerializer<Vector2Int>.Serialize(Vector2Int obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.x, NumberFormat.Variant);
            writer.WriteValue(obj.y, NumberFormat.Variant);
        }

        void ITypeSerializer<Vector3Int>.Serialize(Vector3Int obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.x, NumberFormat.Variant);
            writer.WriteValue(obj.y, NumberFormat.Variant);
            writer.WriteValue(obj.z, NumberFormat.Variant);
        }

        void ITypeSerializer<Quaternion>.Serialize(Quaternion obj, ref StreamingWriter writer)
        {
            writer.WriteValue(obj.x);
            writer.WriteValue(obj.y);
            writer.WriteValue(obj.z);
            writer.WriteValue(obj.w);
        }
    }
}
#endif