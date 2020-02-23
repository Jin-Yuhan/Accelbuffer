using asc.Compiler.Declarations;
using System.CodeDom;
using System.IO;
using System.Text;

namespace asc.Compiler
{
    internal sealed class CSharpManager : DotNetManager
    {
        public override IDeclaration[] Predefines { get; }

        protected override string LanguageName => "CSharp";

        public CSharpManager()
        {
            Predefines = new IDeclaration[]
            {
                new UsingDeclaration { PackageName = "Accelbuffer" },
                new UsingDeclaration { PackageName = "Accelbuffer.Injection" },
                new UsingDeclaration { PackageName = "Accelbuffer.Memory" },
                new UsingDeclaration { PackageName = "Accelbuffer.Unsafe" },
                new UsingDeclaration { PackageName = "Accelbuffer.Unsafe.Text" },
                new UsingDeclaration { PackageName = "System" },
#if UNITY
                new UsingDeclaration { PackageName = "UnityEngine" },
#endif
                new UsingAsDeclaration{TypeName = "System.Boolean", AliasName= "boolean"},
                new UsingAsDeclaration{TypeName = "System.SByte", AliasName= "int8"},
                new UsingAsDeclaration{TypeName = "System.Byte", AliasName= "uint8"},
                new UsingAsDeclaration{TypeName = "System.Int16", AliasName= "int16"},
                new UsingAsDeclaration{TypeName = "System.UInt16", AliasName= "uint16"},
                new UsingAsDeclaration{TypeName = "System.Int32", AliasName= "int32"},
                new UsingAsDeclaration{TypeName = "System.UInt32", AliasName= "uint32"},
                new UsingAsDeclaration{TypeName = "System.Int64", AliasName= "int64"},
                new UsingAsDeclaration{TypeName = "System.UInt64", AliasName= "uint64"},
                new UsingAsDeclaration{TypeName = "System.Single", AliasName= "float32"},
                new UsingAsDeclaration{TypeName = "System.Double", AliasName= "float64"},
                new UsingAsDeclaration{TypeName = "System.Decimal", AliasName= "float128"},
                new UsingAsDeclaration{TypeName = "System.IntPtr", AliasName= "intptr"},
                new UsingAsDeclaration{TypeName = "System.UIntPtr", AliasName= "uintptr"},
                new UsingAsDeclaration{TypeName = "Accelbuffer.VInt", AliasName= "vint"},
                new UsingAsDeclaration{TypeName = "Accelbuffer.VUInt", AliasName= "vuint"},

#if UNITY
                new UsingAsDeclaration{TypeName = "UnityEngine.Vector2", AliasName= "vector2"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Vector3", AliasName= "vector3"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Vector4", AliasName= "vector4"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Vector2Int", AliasName= "vector2int"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Vector3Int", AliasName= "vector3int"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Quaternion", AliasName= "quaternion"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Color", AliasName= "color"},
                new UsingAsDeclaration{TypeName = "UnityEngine.Color32", AliasName= "color32"},
#endif
            };
        }

        public override string ChangeExtension(string filePath)
        {
            return Path.ChangeExtension(filePath, "cs");
        }

        protected override void AddMessageMethods(CodeTypeDeclaration type)
        {
            type.Members.Add(new CodeSnippetTypeMember("partial void OnBeforeSerialization();"));
            type.Members.Add(new CodeSnippetTypeMember("partial void OnAfterDeserialization();"));
        }

        protected override CodeSnippetStatement GetDeserializeMethodBody(IDeclaration[] declarations)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t\t\twhile (reader.HasNext(out index))");
            sb.AppendLine("\t\t\t{");
            {
                sb.AppendLine("\t\t\t\tswitch (index)");
                sb.AppendLine("\t\t\t\t{");
                {

                    foreach (IDeclaration declaration in declarations)
                    {
                        if (!(declaration is FieldDeclaration field))
                        {
                            continue;
                        }

                        bool isFacadeType = !string.IsNullOrEmpty(field.RealType);
                        string readType = isFacadeType ? field.RealType! : field.Type;
                        string typeName = GetTypeNameInMethod(readType);
                        string methodName = typeName == "Generic" ? $"Generic<{ValidateType(readType)}>" : typeName;

                        sb.AppendLine($"\t\t\t\t\tcase {field.Index}:");

                        if (!field.IsObsolete)
                        {
                            if (isFacadeType)
                            {
                                sb.AppendLine($"\t\t\t\t\t\tresult.m_{field.Name} = ({field.Type})reader.Read{methodName}();");
                            }
                            else
                            {
                                sb.AppendLine($"\t\t\t\t\t\tresult.m_{field.Name} = reader.Read{methodName}();");
                            }
                        }

                        sb.AppendLine("\t\t\t\t\t\tbreak;");
                    }

                    sb.AppendLine("\t\t\t\t\tdefault:");
                    sb.AppendLine("\t\t\t\t\t\treader.SkipNext();");
                    sb.AppendLine("\t\t\t\t\t\tbreak;");
                }
                sb.AppendLine("\t\t\t\t}");
            }
            sb.AppendLine("\t\t\t}");
            return new CodeSnippetStatement(sb.ToString());
        }

        protected override CodeSnippetStatement GetFromBytesMethodBody(string typeName)
        {
            return new CodeSnippetStatement($"return Serializer.Deserialize<{typeName}>(bytes, 0, bytes.Length);");
        }
    }
}
