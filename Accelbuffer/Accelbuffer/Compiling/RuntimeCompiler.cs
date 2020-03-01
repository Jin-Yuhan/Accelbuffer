using Accelbuffer.Compiling.Declarations;
using Accelbuffer.Injection;
using Accelbuffer.Injection.IL;
using Accelbuffer.Injection.IL.Serializers;
using Accelbuffer.Properties;
using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个运行时编译器
    /// </summary>
    public sealed class RuntimeCompiler
    {
        private static readonly Type[] s_DynamicSerializeFunctionArgs = new Type[] { typeof(object), typeof(Encoding), typeof(Endian) };
        private static readonly Type[] s_DynamicDeserializeFunctionArgs = new Type[] { typeof(byte[]), typeof(int), typeof(int) };
        private static readonly RuntimeMethodHandle s_SerializeMethod;
        private static readonly RuntimeMethodHandle s_DeserializeMethod;

        static RuntimeCompiler()
        {
            MethodInfo[] methods = typeof(Serializer).GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.Name == Resources.SerializeMethodName && method.ReturnType == typeof(void))
                {
                    s_SerializeMethod = method.MethodHandle;
                }
                else if (method.Name == Resources.DeserializeMethodName)
                {
                    ParameterInfo[] parameters = method.GetParameters();

                    if (parameters != null && parameters.Length == 3 && parameters[0].ParameterType == typeof(byte[]))
                    {
                        s_DeserializeMethod = method.MethodHandle;
                    }
                }
            }
        }

        private readonly IReadOnlyList<string> m_BuiltinPackages;
        private readonly IReadOnlyDictionary<string, Type> m_BuiltinTypeAlias;
        private readonly List<string> m_ImportedPackages;
        private readonly Dictionary<string, Type> m_TypeAlias;
        private readonly StringBuilder m_StringBuilder;
        private readonly StringBuilder m_StringBuilderType2NestName;

        /// <summary>
        /// 初始化RuntimeCompiler
        /// </summary>
        public RuntimeCompiler()
        {
            m_BuiltinPackages = new List<string>()
            {
                "Accelbuffer",
                "Accelbuffer.Injection",
                "Accelbuffer.Memory",
                "System",
                "System.Collections.Generic",
#if UNITY
                "UnityEngine"
#endif
            };
            m_BuiltinTypeAlias = new Dictionary<string, Type>()
            {
                ["boolean"] = typeof(bool),
                ["int8"] = typeof(sbyte),
                ["uint8"] = typeof(byte),
                ["int16"] = typeof(short),
                ["uint16"] = typeof(ushort),
                ["int32"] = typeof(int),
                ["uint32"] = typeof(uint),
                ["int64"] = typeof(long),
                ["uint64"] = typeof(ulong),
                ["float32"] = typeof(float),
                ["float64"] = typeof(double),
                ["float128"] = typeof(decimal),
                ["nint"] = typeof(IntPtr),
                ["nuint"] = typeof(UIntPtr),
                ["vint"] = typeof(VInt),
                ["vuint"] = typeof(VUInt),
                ["char"] = typeof(char),
                ["string"] = typeof(string)
            };

            m_ImportedPackages = new List<string>();
            m_TypeAlias = new Dictionary<string, Type>();
            m_StringBuilder = new StringBuilder();
            m_StringBuilderType2NestName = new StringBuilder();
        }

        /// <summary>
        /// 编译一系列声明
        /// </summary>
        /// <param name="array">
        /// 需要编译的声明列表，
        /// <see cref="PackageDeclaration"/>应该为首元素（如果有），
        /// <see cref="UsingDeclaration"/>和<see cref="UsingAsDeclaration"/>应该在所有结构声明之前
        /// </param>
        /// <returns>编译结果，Key:类型FullName，Value:类型元数据对象</returns>
        public IReadOnlyDictionary<string, AccelTypeInfo> Compile(DeclarationArray array)
        {
            IDeclaration[] declarations = array.Declarations;

            if (declarations == null)
            {
                return null;
            }

            Dictionary<string, AccelTypeInfo> result = new Dictionary<string, AccelTypeInfo>();
            m_ImportedPackages.Clear();
            m_TypeAlias.Clear();

            string packageName = string.Empty;

            for (int i = 0; i < declarations.Length; i++)
            {
                IDeclaration declaration = declarations[i];

                switch (declaration)
                {
                    case PackageDeclaration packageDeclaration:
                        packageName = packageDeclaration.PackageName;
                        m_ImportedPackages.Add(packageDeclaration.PackageName);
                        break;
                    case UsingDeclaration usingDeclaration:
                        m_ImportedPackages.Add(usingDeclaration.PackageName);
                        break;
                    case UsingAsDeclaration usingAsDeclaration:
                        m_TypeAlias.Add(usingAsDeclaration.AliasName, GetType(usingAsDeclaration.TypeName));
                        break;
                    case StructDeclaration structDeclaration:
                        DefineType(packageName, null, null, structDeclaration, result);
                        break;
                    default:
                        continue;
                }
            }

            foreach (AccelTypeInfo typeInfo in result.Values)
            {
                Type type = typeInfo.Info;
                Type serializerType = SerializerInjector.Inject(type, typeInfo);
                SerializerBinder.AddBinding(type, serializerType);

                DefineDynamicSerializeFunction(typeInfo);
                DefineDynamicDeserializeFunction(typeInfo);
            }

            return result;
        }

        private void DefineDynamicSerializeFunction(AccelTypeInfo typeInfo)
        {
            DynamicMethod method = new DynamicMethod(Resources.SerializeMethodName + typeInfo.FullName, typeof(byte[]), s_DynamicSerializeFunctionArgs);
            ILGenerator il = method.GetILGenerator();
            EmitDynamicSerializeFunctionIL(il, typeInfo.Info);
            DynamicSerializeFunction func = method.CreateDelegate(typeof(DynamicSerializeFunction)) as DynamicSerializeFunction;
            typeInfo.SerializeFunction = func;
        }

        private void EmitDynamicSerializeFunctionIL(ILGenerator il, Type objType)
        {
            il.DeclareLocal(typeof(byte[]));
            MethodInfo method = (MethodBase.GetMethodFromHandle(s_SerializeMethod) as MethodInfo).MakeGenericMethod(objType);

            il.Emit(OpCodes.Ldarg_0);

            if (objType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, objType);
            }
            else
            {
                il.Emit(OpCodes.Isinst, objType);
            }

            il.Emit(OpCodes.Ldloca_S, (byte)0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
        }

        private void DefineDynamicDeserializeFunction(AccelTypeInfo typeInfo)
        {
            DynamicMethod method = new DynamicMethod(Resources.DeserializeMethodName + typeInfo.FullName, typeof(object), s_DynamicDeserializeFunctionArgs);
            ILGenerator il = method.GetILGenerator();
            EmitDynamicDeserializeFunctionIL(il, typeInfo.Info);
            DynamicDeserializeFunction func = method.CreateDelegate(typeof(DynamicDeserializeFunction)) as DynamicDeserializeFunction;
            typeInfo.DeserializeFunction = func;
        }

        private void EmitDynamicDeserializeFunctionIL(ILGenerator il, Type objType)
        {
            MethodInfo method = (MethodBase.GetMethodFromHandle(s_DeserializeMethod) as MethodInfo).MakeGenericMethod(objType);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ret);
        }

        private void DefineType(string packageName, AccelTypeInfo declaringType, TypeBuilder declaringTypeBuilder, StructDeclaration def, Dictionary<string, AccelTypeInfo> result)
        {
            string name = def.Name;
            string fullName = def.IsNested ? $"{declaringType.FullName}+{def.Name}" : $"{packageName}.{def.Name}";
            Dictionary<string, AccelFieldInfo> fields = new Dictionary<string, AccelFieldInfo>(def.Declarations.Declarations.Length);
            Type baseType = def.IsRef ? typeof(object) : typeof(ValueType);
            TypeBuilder builder = RuntimeInjector.DefinePublicType(def.IsNested ? name : fullName, def.IsFinal, declaringTypeBuilder, baseType, Type.EmptyTypes);
            AccelTypeInfo info = new AccelTypeInfo(name, fullName, def.Doc, TypeVisibility.Public, def.IsFinal, def.IsRef, def.IsNested, def.IsFieldIndexContinuous, (int)def.Size, declaringType, fields);

            for (int i = 0; i < def.Declarations.Declarations.Length; i++)
            {
                IDeclaration declaration = def.Declarations.Declarations[i];

                switch (declaration)
                {
                    case FieldDeclaration fieldDeclaration:
                        Type fieldType = GetType(fieldDeclaration.Type);
                        Type realType = fieldDeclaration.RealType == null ? null : GetType(fieldDeclaration.RealType);
                        builder.DefineField(fieldDeclaration.Name, fieldType, FieldAttributes.Public);
                        AccelFieldInfo fieldInfo = new AccelFieldInfo(fieldDeclaration.Name, fieldDeclaration.Doc, (int)fieldDeclaration.Index, fieldDeclaration.IsObsolete, fieldDeclaration.IsNeverNull, info, fieldType, realType);
                        fields.Add(fieldDeclaration.Name, fieldInfo);
                        break;
                    case StructDeclaration structDeclaration:
                        DefineType(packageName, info, builder, structDeclaration, result);
                        break;
                    default:
                        continue;
                }
            }

            Type type = builder.CreateType();
            info.TypeHandle = type.TypeHandle;

            foreach (FieldInfo field in type.GetFields())
            {
                fields[field.Name].FieldHandle = field.FieldHandle;
            }

            result.Add(fullName, info);
        }

        private Type GetType(TypeName name, bool isGenericDefinition = false)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Type type;

            if (name.IsGenericType)
            {
                Type genericDefinitionType = GetType(name.GenericTypeDefinitionName, true);
                Type[] args = new Type[name.GenericParameters.Length];

                for (int i = 0; i < name.GenericParameters.Length; i++)
                {
                    args[i] = GetType(name.GenericParameters[i]);
                }

                type = genericDefinitionType.MakeGenericType(args);
            }
            else
            {
                string rawName = isGenericDefinition ? name.RawString : name.RawStringWithoutNullableAndArraySuffix;

                do
                {
                    type = Type.GetType(rawName);

                    if (type != null)
                    {
                        break;
                    }

                    if (m_BuiltinTypeAlias.TryGetValue(rawName, out type))
                    {
                        break;
                    }

                    if (m_TypeAlias.TryGetValue(rawName, out type))
                    {
                        break;
                    }

                    if (TryGetTypeByFullName(m_BuiltinPackages, rawName, out type))
                    {
                        break;
                    }

                    if (TryGetTypeByFullName(m_ImportedPackages, rawName, out type))
                    {
                        break;
                    }

                    if (TryGetTypeInAssemblies(rawName, out type))
                    {
                        break;
                    }

                } while (ChangeToNestedTypeName(ref rawName));
            }

            if (type == null)
            {
                throw new TypeLoadException(name.RawString);
            }

            if (name.IsNullable)
            {
                type = typeof(Nullable<>).MakeGenericType(type);
            }

            for (int i = 0; i < name.ArraySuffixCount; i++)
            {
                type = type.MakeArrayType();
            }
            
            return type;
        }

        private bool TryGetTypeInAssemblies(string name, out Type type)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            Assembly[] assemblies = domain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                type = assembly.GetType(name);

                if (type != null)
                {
                    return true;
                }

                if (TryGetTypeByFullName(m_BuiltinPackages, assembly, name, out type))
                {
                    return true;
                }

                if (TryGetTypeByFullName(m_ImportedPackages, assembly, name, out type))
                {
                    return true;
                }
            }

            type = null;
            return false;
        }

        private bool TryGetTypeByFullName(IReadOnlyList<string> packages, string name, out Type type)
        {
            for (int i = 0; i < packages.Count; i++)
            {
                m_StringBuilder.Clear();
                m_StringBuilder.Append(packages[i]).Append('.').Append(name);
                string newName = m_StringBuilder.ToString();
                type = Type.GetType(newName);

                if (type != null)
                {
                    return true;
                }
            }

            type = null;
            return false;
        }

        private bool TryGetTypeByFullName(IReadOnlyList<string> packages, Assembly assembly, string name, out Type type)
        {
            for (int i = 0; i < packages.Count; i++)
            {
                m_StringBuilder.Clear();
                m_StringBuilder.Append(packages[i]).Append('.').Append(name);
                string newName = m_StringBuilder.ToString();
                type = assembly.GetType(newName);

                if (type != null)
                {
                    return true;
                }
            }

            type = null;
            return false;
        }

        private bool ChangeToNestedTypeName(ref string name)
        {
            int index = name.LastIndexOf('.');

            if (index < 0)
            {
                return false;
            }

            m_StringBuilderType2NestName.Clear();
            m_StringBuilderType2NestName.Append(name, 0, index).Append('+').Append(name, index + 1, name.Length - index - 1);
            name = m_StringBuilderType2NestName.ToString();
            return true;
        }
    }
}
