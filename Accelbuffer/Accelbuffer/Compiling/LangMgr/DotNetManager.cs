using Accelbuffer.Compiling.Declarations;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Accelbuffer.Reflection;

namespace Accelbuffer.Compiling
{
    internal abstract class DotNetManager : LanguageManager
    {
        private static readonly CodeAttributeDeclaration s_NeverNullAttr = new CodeAttributeDeclaration("NeverNull");
        private static readonly CodeAttributeDeclaration s_ObsoleteAttr = new CodeAttributeDeclaration("Obsolete");
#if UNITY
        private static readonly CodeAttributeDeclaration s_SerializeFieldAttr = new CodeAttributeDeclaration("SerializeField");
#endif

        private static readonly CodePropertySetValueReferenceExpression s_PropertyValue = new CodePropertySetValueReferenceExpression();
        private static readonly CodeThisReferenceExpression s_This = new CodeThisReferenceExpression();

        private static readonly CodeParameterDeclarationExpression s_Writer = new CodeParameterDeclarationExpression("AccelWriter", "writer") { Direction = FieldDirection.Ref };
        private static readonly CodeParameterDeclarationExpression s_Reader = new CodeParameterDeclarationExpression("AccelReader", "reader") { Direction = FieldDirection.Ref };

        private static readonly CodeVariableDeclarationStatement s_Index = new CodeVariableDeclarationStatement(typeof(int), "index");

        private static readonly CodeArgumentReferenceExpression s_WriterRef = new CodeArgumentReferenceExpression("writer");
        private static readonly CodeArgumentReferenceExpression s_ObjRef = new CodeArgumentReferenceExpression("obj");

        private static readonly CodeVariableReferenceExpression s_ResultRef = new CodeVariableReferenceExpression("result");
        private static readonly CodeMethodReturnStatement s_ReturnResult = new CodeMethodReturnStatement(s_ResultRef);

        private static readonly CodePrimitiveExpression s_Null = new CodePrimitiveExpression(null); 

        private static readonly CodeMethodInvokeExpression s_OnBeforeSerialization = new CodeMethodInvokeExpression(s_ObjRef, "OnBeforeSerialization"); 
        private static readonly CodeMethodInvokeExpression s_OnAfterDeserialization = new CodeMethodInvokeExpression(s_ResultRef, "OnAfterDeserialization");

        private static readonly CodeVariableDeclarationStatement s_BytesResult = new CodeVariableDeclarationStatement(typeof(byte[]), "result");
        private static readonly CodeMethodInvokeExpression s_Serialize = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("Serializer"), "Serialize", s_This, new CodeDirectionExpression(FieldDirection.Out, s_ResultRef));

        private static readonly CodeTypeReference s_ByteArray = new CodeTypeReference(typeof(byte[]));

        private static readonly CodeParameterDeclarationExpression s_Bytes = new CodeParameterDeclarationExpression(s_ByteArray, "bytes");

        protected abstract string LanguageName { get; }

        protected abstract List<IDeclaration> Predefines { get; }

        protected DotNetManager() { }

        public sealed override void GenerateCode(DeclarationArray array, StreamWriter writer)
        {
            IDeclaration[] declarations = array.Declarations;
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace();
            codeNamespace.Comments.Add(new CodeCommentStatement("扩展类型请使用partial关键字在其他文件扩展，否则更改可能会丢失。", false));
            List<IDeclaration> declarations1 = new List<IDeclaration>(Predefines);
            declarations1.AddRange(declarations);

            foreach (IDeclaration declaration in declarations1)
            {
                switch (declaration)
                {
                    case PackageDeclaration packageDeclaration:
                        codeNamespace.Name = packageDeclaration.PackageName;
                        break;
                    case UsingDeclaration usingDeclaration:
                        CodeNamespaceImport import = new CodeNamespaceImport(usingDeclaration.PackageName);
                        codeNamespace.Imports.Add(import);
                        break;
                    case UsingAsDeclaration usingAsDeclaration:
                        CodeNamespaceImport alias = new CodeNamespaceImport($"{usingAsDeclaration.AliasName} = {usingAsDeclaration.TypeName.RawString}");
                        codeNamespace.Imports.Add(alias);
                        break;
                    case StructDeclaration structDeclaration:
                        CodeTypeDeclaration type = GetTypeDecalaration(structDeclaration);
                        codeNamespace.Types.Add(type);
                        break;
                }
            }

            unit.Namespaces.Add(codeNamespace);

            CodeDomProvider provider = CodeDomProvider.CreateProvider(LanguageName);
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = true
            };

            provider.GenerateCodeFromCompileUnit(unit, writer, options);
        }

        private CodeTypeDeclaration GetTypeDecalaration(StructDeclaration structDeclaration)
        {
            CodeTypeDeclaration type = new CodeTypeDeclaration(structDeclaration.Name)
            {
                IsClass = structDeclaration.IsRef,
                IsStruct = !structDeclaration.IsRef,
                IsPartial = true
            };
            string serializerName = structDeclaration.Name + "Serializer";
            string serializerFullName = $"{structDeclaration.Name}.{serializerName}";

            AddMemorySizeAttribute(type, (int)structDeclaration.Size);
            AddSerializeByAttribute(type, serializerFullName);
            AddMessageMethods(type);
            
            if (structDeclaration.Doc != null)
            {
                type.Comments.Add(GetDocument(structDeclaration.Doc));
            }

            switch (structDeclaration.Visibility)
            {
                case TypeVisibility.Internal when structDeclaration.IsNested:
                    type.TypeAttributes = TypeAttributes.NestedAssembly;
                    break;
                case TypeVisibility.Internal:
                    type.TypeAttributes = TypeAttributes.NotPublic;
                    break;

                case TypeVisibility.Private:
                    type.TypeAttributes = TypeAttributes.NestedPrivate;
                    break;

                case TypeVisibility.PrivateProtected:
                    type.TypeAttributes = TypeAttributes.NestedFamANDAssem;
                    break;

                case TypeVisibility.Protected:
                    type.TypeAttributes = TypeAttributes.NestedFamily;
                    break;

                case TypeVisibility.ProtectedInternal:
                    type.TypeAttributes = TypeAttributes.NestedFamORAssem;
                    break;

                case TypeVisibility.Public:
                    type.TypeAttributes = TypeAttributes.Public;
                    break;
            }

            if (structDeclaration.IsFinal)
            {
                type.TypeAttributes |= TypeAttributes.Sealed;
            }

            CodeTypeReference structRef = new CodeTypeReference(structDeclaration.Name);

            AddShortcutMethod(type, structDeclaration.Name, structRef);

            AddSerializerType(type, structRef, serializerName, structDeclaration.Name, out var serializeMethod, out var deserializeMethod);

            foreach (var declaration in structDeclaration.Declarations.Declarations)
            {
                switch (declaration)
                {
                    case FieldDeclaration fieldDeclaration:
                        string fieldName = "m_" + fieldDeclaration.Name;
                        string typeName = ValidateType(fieldDeclaration.Type.RawString);

                        CodeTypeReference fieldType = GetTypeReference(typeName);
                        CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(s_ObjRef, fieldName);
                        CodePrimitiveExpression index = new CodePrimitiveExpression((int)fieldDeclaration.Index);
                        CodeTypeReferenceExpression realType = fieldDeclaration.RealType == null ? null : new CodeTypeReferenceExpression(GetTypeReference(fieldDeclaration.RealType.RawString));

                        CodeMemberField field = new CodeMemberField()
                        {
                            Attributes = MemberAttributes.Private | MemberAttributes.Final,
                            Name = fieldName,
                            Type = fieldType,
                        };

                        if (fieldDeclaration.IsNeverNull)
                        {
                            field.CustomAttributes.Add(s_NeverNullAttr);
                        }

                        if (fieldDeclaration.IsObsolete)
                        {
                            field.CustomAttributes.Add(s_ObsoleteAttr);
                        }

                        AddSerialIndexAttribute(field, index);

                        if (realType != null)
                        {
                            AddFacadeTypeAttribute(field, realType.Type);
                        }

#if UNITY
                        field.CustomAttributes.Add(s_SerializeFieldAttr);
#endif

                        CodeMemberProperty property = new CodeMemberProperty()
                        {
                            Attributes = MemberAttributes.Public | MemberAttributes.Final,
                            Name = fieldDeclaration.Name,
                            Type = fieldType,
                            HasGet = true,
                            HasSet = true
                        };

                        if (fieldDeclaration.Doc != null)
                        {
                            property.Comments.Add(GetDocument(fieldDeclaration.Doc));
                        }

                        if (fieldDeclaration.IsObsolete)
                        {
                            property.CustomAttributes.Add(s_ObsoleteAttr);
                        }

                        SetPropertyMethods(property, fieldName);

                        type.Members.Add(field);
                        type.Members.Add(property);

                        if (!fieldDeclaration.IsObsolete)
                        {
                            AddSerializedField(serializeMethod, fieldRef, index, realType, fieldDeclaration.IsNeverNull);
                        }

                        break;
                    case StructDeclaration structDeclaration1:
                        CodeTypeDeclaration type1 = GetTypeDecalaration(structDeclaration1);
                        type.Members.Add(type1);
                        break;
                }
            }

            deserializeMethod.Statements.Add(GetDeserializeMethodBody(structDeclaration.Declarations.Declarations));
            deserializeMethod.Statements.Add(s_OnAfterDeserialization);
            deserializeMethod.Statements.Add(s_ReturnResult);

            return type;
        }

        protected abstract CodeSnippetStatement GetDeserializeMethodBody(IDeclaration[] declarations);

        protected abstract CodeSnippetStatement GetFromBytesMethodBody(string typeName);

        protected virtual string ValidateType(string rawType)
        {
            return rawType;
        }

        protected string GetTypeNameInMethod(string name)
        {
            switch (name)
            {
                case "boolean": return "Boolean";

                case "int8": return "Int8";
                case "uint8": return "UInt8";
                case "int16": return "Int16";
                case "uint16": return "UInt16";
                case "int32": return "Int32";
                case "uint32": return "UInt32";
                case "int64": return "Int64";
                case "uint64": return "UInt64";

                case "float32": return "Float32";
                case "float64": return "Float64";
                case "float128": return "Float128";

                case "char": return "Char";
                case "string": return "String";

                case "nint": return "IntPtr";
                case "nuint": return "UIntPtr";

                case "vint": return "VariantInt";
                case "vuint": return "VariantUInt";

#if UNITY
                case "Vector2": return "Vector2";
                case "Vector3": return "Vector3";
                case "Vector4": return "Vector4";
                case "Vector2int": return "Vector2Int";
                case "Vector3int": return "Vector3Int";
                case "Quaternion": return "Quaternion";
                case "Color": return "Color";
                case "Color32": return "Color32";
#endif
                default: return "Generic";
            }
        }

        protected abstract void AddMessageMethods(CodeTypeDeclaration type);

        private void AddSerializerType(CodeTypeDeclaration type, CodeTypeReference structRef, string serializerName, string structName, out CodeMemberMethod serializeMethod, out CodeMemberMethod deserializeMethod)
        {
            CodeTypeReference interfaceTypeRef = new CodeTypeReference(ValidateType($"ITypeSerializer<{structName}>"));
            CodeTypeDeclaration serializerType = new CodeTypeDeclaration(serializerName) { IsClass = true };
            serializerType.TypeAttributes |= TypeAttributes.Sealed;
            serializerType.BaseTypes.Add(new CodeTypeReference("System.Object"));
            serializerType.BaseTypes.Add(interfaceTypeRef);
            serializerType.Comments.Add(GetDocument("对象序列化代理（自动生成）"));

            serializeMethod = new CodeMemberMethod
            {
                Name = "Serialize",
                ReturnType = new CodeTypeReference(typeof(void)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };
            serializeMethod.ImplementationTypes.Add(interfaceTypeRef);
            serializeMethod.Parameters.Add(new CodeParameterDeclarationExpression(structName, "obj"));
            serializeMethod.Parameters.Add(s_Writer);
            serializeMethod.Comments.Add(GetDocument("序列化方法（自动生成）"));
            serializeMethod.Statements.Add(s_OnBeforeSerialization);

            deserializeMethod = new CodeMemberMethod
            {
                Name = "Deserialize",
                ReturnType = new CodeTypeReference(structName),
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            deserializeMethod.ImplementationTypes.Add(interfaceTypeRef);
            deserializeMethod.Parameters.Add(s_Reader);
            deserializeMethod.Statements.Add(new CodeVariableDeclarationStatement(structRef, "result", new CodeObjectCreateExpression(structRef)));
            deserializeMethod.Statements.Add(s_Index);
            deserializeMethod.Comments.Add(GetDocument("反序列化方法（自动生成）"));

            serializerType.Members.Add(serializeMethod);
            serializerType.Members.Add(deserializeMethod);
            type.Members.Add(serializerType);
        }

        private void AddSerializedField(CodeMemberMethod serializeMethod, CodeFieldReferenceExpression fieldRef, CodePrimitiveExpression index, CodeTypeReferenceExpression realType, bool isNeverNull)
        {
            CodeExpression param = realType == null ? fieldRef as CodeExpression : new CodeCastExpression(realType.Type, fieldRef);
            CodeExpression expr = new CodeMethodInvokeExpression(s_WriterRef, "WriteValue", index, param);

            if (isNeverNull)
            {
                serializeMethod.Statements.Add(expr);
            }
            else
            {
                CodeConditionStatement condition = new CodeConditionStatement(new CodeBinaryOperatorExpression(fieldRef, CodeBinaryOperatorType.IdentityInequality, s_Null), new CodeExpressionStatement(expr));
                serializeMethod.Statements.Add(condition);
            }  
        }

        private CodeTypeReference GetTypeReference(string type)
        {
            switch (type)
            {
                case "char": return new CodeTypeReference(typeof(char));
                case "string": return new CodeTypeReference(typeof(string));
                default: return new CodeTypeReference(type);
            }
        }

        private CodeCommentStatement GetDocument(string doc)
        {
            return new CodeCommentStatement($"<summary>{Environment.NewLine} {doc}{Environment.NewLine} </summary>", true);
        }

        private void AddShortcutMethod(CodeTypeDeclaration type, string typeName, CodeTypeReference structRef)
        {
            CodeMemberMethod toBytes = new CodeMemberMethod
            {
                Name = "ToBytes",
                ReturnType = s_ByteArray,
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };
            toBytes.Statements.Add(s_BytesResult);
            toBytes.Statements.Add(s_Serialize);
            toBytes.Statements.Add(s_ReturnResult);
            toBytes.Comments.Add(GetDocument("序列化对象"));
            type.Members.Add(toBytes);

            CodeMemberMethod fromBytes = new CodeMemberMethod
            {
                Name = "FromBytes",
                ReturnType = structRef,
                Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static,
            };
            fromBytes.Parameters.Add(s_Bytes);
            fromBytes.Statements.Add(GetFromBytesMethodBody(typeName));
            fromBytes.Comments.Add(GetDocument("反序列化对象"));
            type.Members.Add(fromBytes);
        }

        private void SetPropertyMethods(CodeMemberProperty property, string fieldName)
        {
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(s_This, fieldName)));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(s_This, fieldName), s_PropertyValue));
        }

        private void AddSerialIndexAttribute(CodeMemberField field, CodePrimitiveExpression index)
        {
            CodeAttributeArgument attrArg = new CodeAttributeArgument(index);
            CodeAttributeDeclaration fieldIndexAttr = new CodeAttributeDeclaration("SerialIndex", attrArg);
            field.CustomAttributes.Add(fieldIndexAttr);
        }

        private void AddFacadeTypeAttribute(CodeMemberField field, CodeTypeReference type)
        {
            CodeAttributeArgument attrArg = new CodeAttributeArgument(new CodeTypeOfExpression(type));
            CodeAttributeDeclaration fieldIndexAttr = new CodeAttributeDeclaration("FacadeType", attrArg);
            field.CustomAttributes.Add(fieldIndexAttr);
        }

        private void AddMemorySizeAttribute(CodeTypeDeclaration type, int size)
        {
            CodeAttributeArgument attrArg = new CodeAttributeArgument(new CodePrimitiveExpression(size));
            CodeAttributeDeclaration memSizeAttr = new CodeAttributeDeclaration("MemorySize", attrArg);
            type.CustomAttributes.Add(memSizeAttr);
        }

        private void AddSerializeByAttribute(CodeTypeDeclaration type, string fullName)
        {
            CodeAttributeArgument attrArg = new CodeAttributeArgument(new CodeTypeOfExpression(fullName));
            CodeAttributeDeclaration serializeByAttr = new CodeAttributeDeclaration("SerializeBy", attrArg);
            type.CustomAttributes.Add(serializeByAttr);
        }
    }
}
