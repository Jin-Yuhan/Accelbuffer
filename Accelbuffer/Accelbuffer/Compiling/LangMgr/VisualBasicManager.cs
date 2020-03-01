using Accelbuffer.Compiling.Declarations;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace Accelbuffer.Compiling
{
    internal sealed class VisualBasicManager : DotNetManager
    {
        private List<IDeclaration> m_Declarations;

        public override string Entension => "vb";

        protected override List<IDeclaration> Predefines => m_Declarations;

        public VisualBasicManager()
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

                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.SByte"), AliasName= "int8"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Byte"), AliasName= "uint8"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Single"), AliasName= "float32"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Double"), AliasName= "float64"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.Decimal"), AliasName= "float128"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.IntPtr"), AliasName= "nint"},
                new UsingAsDeclaration { TypeName = TypeName.GetSimpleTypeName("System.UIntPtr"), AliasName= "nuint"},
            };
        }

        protected override string LanguageName => "VisualBasic";

        protected override string ValidateType(string rawType)
        {
            return rawType.Replace("<", "(Of ").Replace('>', ')').Replace('[', '(').Replace(']', ')');
        }

        protected override void AddMessageMethods(CodeTypeDeclaration type)
        {
            type.Members.Add(new CodeSnippetTypeMember($"Partial Private Sub OnBeforeSerialization(){Environment.NewLine}End Sub"));
            type.Members.Add(new CodeSnippetTypeMember($"Partial Private Sub OnAfterDeserialization(){Environment.NewLine}End Sub"));
        }

        protected override CodeSnippetStatement GetDeserializeMethodBody(IDeclaration[] declarations)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t\t\t\tDo While reader.HasNext(index)");
            {
                sb.AppendLine("\t\t\t\t\tSelect Case index");
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
                        string methodName = typeName == "Generic" ? $"Generic(Of {ValidateType(readType)})" : typeName;

                        sb.AppendLine($"\t\t\t\t\t\tCase {field.Index}");

                        if (!field.IsObsolete)
                        {
                            if (isFacadeType)
                            {
                                sb.AppendLine($"\t\t\t\t\t\t\tresult.m_{field.Name} = CType(reader.Read{methodName}(), {field.Type})");
                            }
                            else
                            {
                                sb.AppendLine($"\t\t\t\t\t\t\tresult.m_{field.Name} = reader.Read{methodName}()");
                            }
                        }
                    }

                    sb.AppendLine("\t\t\t\t\t\tCase Else");
                    sb.AppendLine("\t\t\t\t\t\t\treader.SkipNext()");
                }
                sb.AppendLine("\t\t\t\t\tEnd Select");
            }
            sb.AppendLine("\t\t\t\tLoop");
            return new CodeSnippetStatement(sb.ToString());
        }

        protected override CodeSnippetStatement GetFromBytesMethodBody(string typeName)
        {
            return new CodeSnippetStatement($"Return Serializer.Deserialize(Of {typeName})(bytes, 0, bytes.Length)");
        }
    }
}
