using UnityEngine;
using static Accelbuffer.SerializeProxyInjector;

namespace Accelbuffer
{
    public static class UnityExtensionRegistry
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Register()
        {
            AddBinding<Vector2, VectorSerializeProxy>();
            AddBinding<Vector3, VectorSerializeProxy>();
            AddBinding<Vector4, VectorSerializeProxy>();
            AddBinding<Vector2Int, VectorSerializeProxy>();
            AddBinding<Vector3Int, VectorSerializeProxy>();

            AddBinding<Rect, RectSerializeProxy>();
            AddBinding<RectInt, RectSerializeProxy>();

            AddBinding<Quaternion, OtherTypesSerializeProxy>();
            AddBinding<Matrix4x4, OtherTypesSerializeProxy>();
            AddBinding<LayerMask, OtherTypesSerializeProxy>();

            AddBinding<Color, ColorSerializeProxy>();
            AddBinding<Color32, ColorSerializeProxy>();

            AddBinding<Bounds, BoundsSerializeProxy>();
            AddBinding<BoundsInt, BoundsSerializeProxy>();
        }
    }
}
