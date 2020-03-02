using UnityEditor;
using UnityEngine;

namespace Accelbuffer.Unity
{
    [CustomPropertyDrawer(typeof(VIntRangeAttribute))]
    public class VIntRangeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type != typeof(VInt).Name && property.type != typeof(VUInt).Name)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Not Supported"));
            }

            VIntRangeAttribute range = attribute as VIntRangeAttribute;
            SerializedProperty value = property.FindPropertyRelative("m_Value");
            value.longValue = EditorGUI.IntSlider(position, label, value.intValue, range.Min, range.Max);
        }
    }
}
