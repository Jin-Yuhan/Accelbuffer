using UnityEditor;
using UnityEngine;

namespace Accelbuffer.Unity
{
    [CustomPropertyDrawer(typeof(VInt))]
    public sealed class VIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative("m_Value");
            value.longValue = EditorGUI.LongField(position, label, value.longValue);
        }
    }
}
