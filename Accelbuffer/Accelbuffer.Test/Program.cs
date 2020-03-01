using System;
using Accelbuffer.Compiling;
using Accelbuffer.Reflection;

namespace Accelbuffer.Test
{
    static class Program
    {
        private static void Main()
        {
            FieldTypeTestProgram.ProgramMain();
            //SpeedTestProgram.ProgramMain();

            string path = @"C:\Users\Administrator\Desktop\AccelbufferScriptExamples\msg.bytes";
            byte[] vs = System.IO.File.ReadAllBytes(path);
            DeclarationArray array = DeclarationArray.FromBytes(vs);
            RuntimeCompiler compiler = new RuntimeCompiler();
            var data = compiler.Compile(array);

            AccelTypeInfo type = data["Accelbuffer.Test.Msg"];
            dynamic obj = type.CreateInstance();
            obj.Name = "A";
            obj.Friends = "B";
            obj.Id = 10;//typeinfo.serialize/deserialize

            type.GetField("Name").SetValue(obj, "P");

            byte[] bytes = Serializer.Serialize(obj, type);
            object o1 = Serializer.Deserialize(type, bytes, 0, bytes.Length);


            Accelbuffer.Injection.IL.RuntimeInjector.SaveAssembly();
            Console.ReadKey();
        }
    }
}
