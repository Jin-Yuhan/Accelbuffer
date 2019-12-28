using UnityEngine;

namespace Accelbuffer
{
    public static class UnityExtensionRegistry
    {
        private static bool s_Registered = false;

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterIfNot()
        {
            if (!s_Registered)
            {
                s_Registered = true;

                SerializationSettings.AddSerializeProxyBinding<Vector2, VectorSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<Vector3, VectorSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<Vector4, VectorSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<Vector2Int, VectorSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<Vector3Int, VectorSerializeProxy>();

                SerializationSettings.AddSerializeProxyBinding<Rect, RectSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<RectInt, RectSerializeProxy>();

                SerializationSettings.AddSerializeProxyBinding<Quaternion, OtherTypesSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<Matrix4x4, OtherTypesSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<LayerMask, OtherTypesSerializeProxy>();

                SerializationSettings.AddSerializeProxyBinding<Color, ColorSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<Color32, ColorSerializeProxy>();

                SerializationSettings.AddSerializeProxyBinding<Bounds, BoundsSerializeProxy>();
                SerializationSettings.AddSerializeProxyBinding<BoundsInt, BoundsSerializeProxy>();
            }
        }
    }
}
