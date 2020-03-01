using Accelbuffer.Compiling.Declarations;
using System.CodeDom;
using System.Text;
using System.Collections.Generic;

namespace Accelbuffer.Compiling
{
    internal sealed class CSharpManager : DotNetManager
    {
        private List<IDeclaration> m_Declarations;

        public override string Entension => "cs";

        protected override List<IDeclaration> Predefines => m_Declarations;

        public CSharpManager()
        {
            m_Declarations = new List<IDeclaration>()
            {
                new UsingDeclaration { PackageName = "Accelbuffer" },
                new UsingDeclaration { PackageName = "Accelbuffer.Injection" },
                new UsingDeclaration { PackageName = "Accelbuffer.Memory" },
                new UsingDeclaration { PackageName = "System" },
                new UsingDeclaration { PackageName = "System.Collections.Generic" },
#if UNITY
                new UsingDeclaration { PackageName = "UnityEngine" },
#endif

                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Boolean"), AliasName= "boolean"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.SByte"), AliasName= "int8"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Byte"), AliasName= "uint8"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Int16"), AliasName= "int16"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.UInt16"), AliasName= "uint16"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Int32"), AliasName= "int32"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.UInt32"), AliasName= "uint32"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Int64"), AliasName= "int64"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.UInt64"), AliasName= "uint64"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Single"), AliasName= "float32"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Double"), AliasName= "float64"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Decimal"), AliasName= "float128"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.IntPtr"), AliasName= "nint"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.UIntPtr"), AliasName= "nuint"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("Accelbuffer.VInt"), AliasName= "vint"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("Accelbuffer.VUInt"), AliasName= "vuint"}
            };
        }

        protected override string LanguageName => "CSharp";

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

                        bool isFacadeType = field.RealType != null;
                        string readType = isFacadeType ? field.RealType.RawString : field.Type.RawString;
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
