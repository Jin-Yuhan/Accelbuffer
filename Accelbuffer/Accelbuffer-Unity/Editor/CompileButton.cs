using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Accelbuffer.Unity
{
    public sealed class CompileButton
    {
        private GUIContent m_ButtonContent0;
        private GUIContent m_ButtonContent1;
        private GUIContent m_ButtonContent2;
        private GUIContent m_ButtonContent3;

        public void OnEnable()
        {
            m_ButtonContent0 = new GUIContent("Compile");
            m_ButtonContent1 = new GUIContent("Compile to C#");
            m_ButtonContent2 = new GUIContent("Compile to bytes");
            m_ButtonContent3 = new GUIContent("Compile to both");
        }

        public void OnGUI(Rect rect, Object[] targets)
        {
            if (EditorGUI.DropdownButton(rect, m_ButtonContent0, FocusType.Passive))
            {
                string[] paths = targets.Select(o => AssetDatabase.GetAssetPath(o)).ToArray();
                GenericMenu menu = new GenericMenu();
                menu.AddItem(m_ButtonContent1, false, ps => UnityCompiler.Compile(true, false, ps as string[]), paths);
                menu.AddItem(m_ButtonContent2, false, ps => UnityCompiler.Compile(true, true, ps as string[]), paths);
                menu.AddItem(m_ButtonContent3, false, ps =>
                {
                    UnityCompiler.Compile(true, false, ps as string[]);
                    UnityCompiler.Compile(true, true, ps as string[]);
                }, paths);
                menu.ShowAsContext();
            }
        }
    }
}
