#if UNITY
using UnityEngine;
using Accelbuffer.Memory;

namespace Accelbuffer.Injection
{
    internal sealed class UnitySerializer :
        IBuiltinTypeSerializer,

        IMemorySizeForType<Color>,
        IMemorySizeForType<Color32>,
        IMemorySizeForType<Vector2>,
        IMemorySizeForType<Vector3>,
        IMemorySizeForType<Vector4>,
        IMemorySizeForType<Vector2Int>,
        IMemorySizeForType<Vector3Int>,
        IMemorySizeForType<Quaternion>,

        ITypeSerializer<Color>,
        ITypeSerializer<Color32>,
        ITypeSerializer<Vector2>,
        ITypeSerializer<Vector3>,
        ITypeSerializer<Vector4>,
        ITypeSerializer<Vector2Int>,
        ITypeSerializer<Vector3Int>,
        ITypeSerializer<Quaternion>
    {
        int IMemorySizeForType<Vector2>.ApproximateMemorySize => 8;

        int IMemorySizeForType<Vector3>.ApproximateMemorySize => 12;

        int IMemorySizeForType<Vector2Int>.ApproximateMemorySize => 8;

        int IMemorySizeForType<Vector3Int>.ApproximateMemorySize => 12;

        int IMemorySizeForType<Quaternion>.ApproximateMemorySize => 16;

        int IMemorySizeForType<Vector4>.ApproximateMemorySize => 16;

        int IMemorySizeForType<Color>.ApproximateMemorySize => 16;

        int IMemorySizeForType<Color32>.ApproximateMemorySize => 4;

        Vector2 ITypeSerializer<Vector2>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVector2();
        }

        Vector3 ITypeSerializer<Vector3>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVector3();
        }

        Vector4 ITypeSerializer<Vector4>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVector4();
        }

        Vector2Int ITypeSerializer<Vector2Int>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVector2Int();
        }

        Vector3Int ITypeSerializer<Vector3Int>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadVector3Int();
        }

        Quaternion ITypeSerializer<Quaternion>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadQuaternion();
        }

        Color ITypeSerializer<Color>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadColor();
        }

        Color32 ITypeSerializer<Color32>.Deserialize(ref AccelReader reader)
        {
            return reader.ReadColor32();
        }

        void ITypeSerializer<Vector2>.Serialize(Vector2 obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Vector3>.Serialize(Vector3 obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Vector4>.Serialize(Vector4 obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Vector2Int>.Serialize(Vector2Int obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Vector3Int>.Serialize(Vector3Int obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Quaternion>.Serialize(Quaternion obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Color>.Serialize(Color obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }

        void ITypeSerializer<Color32>.Serialize(Color32 obj, ref AccelWriter writer)
        {
            writer.WriteValue(writer.m_Index, obj);
        }
    }
}
#endif