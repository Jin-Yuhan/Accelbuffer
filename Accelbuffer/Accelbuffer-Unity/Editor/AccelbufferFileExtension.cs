using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Accelbuffer.Unity
{
    public class AccelbufferFileExtension
    {
        [MenuItem("Assets/Create/Accelbuffer Script", false, 80)]
        public static void CreatNew()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateFileAction>(), "NewAccelbufferScript.accel",null,null);
        }
    }

    public class CreateFileAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            pathName = pathName.Replace(" ", "");

            using (StreamWriter writer = File.CreateText(pathName))
            {
                writer.WriteLine($"struct {Path.GetFileNameWithoutExtension(pathName)}");
                writer.WriteLine("{");
                writer.WriteLine();
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
    }
}
