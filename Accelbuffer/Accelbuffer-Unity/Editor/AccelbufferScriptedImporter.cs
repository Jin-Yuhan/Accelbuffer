#if !ACCELBUFFER_LEGACY
using Accelbuffer.Compiling;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Accelbuffer.Unity
{
    [ScriptedImporter(1, "accel")]
    public class AccelbufferScriptedImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string path = Application.dataPath + ctx.assetPath.Replace("Assets", string.Empty);
            TextAsset asset = new TextAsset(File.ReadAllText(path));

            ctx.AddObjectToAsset("main script", asset);
            ctx.SetMainObject(asset);

            string time = File.GetLastWriteTime(path).ToFileTime().ToString();

            if (time != userData)
            {
                userData = time;
                UnityCompiler.Compile(false, false, ctx.assetPath);

                string binPath = LanguageManager.AccelbufferByteCode.ChangeExtension(ctx.assetPath);
                if (AssetDatabase.LoadAssetAtPath<Object>(binPath) != null)
                {
                    UnityCompiler.Compile(false, true, ctx.assetPath);
                }

                EditorApplication.delayCall -= AssetDatabase.Refresh;//avoid multiple
                EditorApplication.delayCall += AssetDatabase.Refresh;
            }

            string csPath = Path.ChangeExtension(ctx.assetPath, "cs");
            Object cs = AssetDatabase.LoadAssetAtPath<Object>(csPath);           

            if (cs == null)
            {
                EditorApplication.delayCall += SaveAndReimport;
            }
            else
            {
                ctx.AddObjectToAsset("compiled script", cs);
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AccelbufferScriptedImporter))]
    public class AccelbufferScriptedImporterEditor : ScriptedImporterEditor
    {
        private CompileButton m_Button;

        public override void OnEnable()
        {
            base.OnEnable();
            m_Button = new CompileButton();
            m_Button.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Accelbuffer Script", EditorStyles.boldLabel);

            rect.xMin = EditorGUIUtility.currentViewWidth - 88;
            rect.width = 65;

            m_Button.OnGUI(rect, targets);
        }
    }
}
#endif