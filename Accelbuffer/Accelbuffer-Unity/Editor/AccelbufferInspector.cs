#if ACCELBUFFER_LEGACY
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Accelbuffer.Unity
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(DefaultAsset))]
    public class AccelbufferInspector : Editor
    {
        private GUIStyle m_TextStyle;
        private MethodInfo m_DrawHeader;
        private CompileButton m_CompileButton;

        private void OnEnable()
        {
            m_DrawHeader = typeof(Editor).GetMethod("DrawHeaderGUI", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(Editor), typeof(string) }, null);
            m_CompileButton = new CompileButton();
            m_CompileButton.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            if (m_TextStyle == null)
            {
                m_TextStyle = "ScriptText";
            }

            bool enabled = GUI.enabled;
            GUI.enabled = true;

            string[] assetPaths = targets.Select(o => AssetDatabase.GetAssetPath(o)).ToArray();

            if (assetPaths.All(s => s.EndsWith(".accel")))
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 3);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, "Accelbuffer Script", EditorStyles.boldLabel);

                rect.xMin = EditorGUIUtility.currentViewWidth - 88;
                rect.width = 65;

                m_CompileButton.OnGUI(rect, targets);

                if (targets.Length == 1)
                {
                    string text = File.ReadAllText(assetPaths[0]);

                    if (text.Length > 7000)
                    {
                        text = text.Substring(0, 7000) + "...\n\n<...etc...>";
                    }

                    rect = GUILayoutUtility.GetRect(new GUIContent(text), m_TextStyle);
                    rect.x = 0f;
                    rect.y -= 3f;
                    rect.width = EditorGUIUtility.currentViewWidth + 1f;
                    GUI.Box(rect, text, m_TextStyle);
                }
            }

            GUI.enabled = enabled;
        }

        protected override void OnHeaderGUI()
        {
            string path = AssetDatabase.GetAssetPath(target);

            if (path.EndsWith(".accel"))
            {
                string name = targets.Length == 1 ? "Accelbuffer Script" : targets.Length + " Accelbuffer Scripts";
                m_DrawHeader.Invoke(null, new object[] { this, name });
            }
            else
            {
                base.OnHeaderGUI();
            }
        }
    }
}
#endif