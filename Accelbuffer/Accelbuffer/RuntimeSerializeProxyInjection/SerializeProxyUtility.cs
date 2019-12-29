using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Accelbuffer
{
    internal static class SerializeProxyUtility
    {
        private static readonly ModuleBuilder s_ModuleBuilder;

        private static readonly TypeAttributes s_TypeAttributes;

        private static readonly string s_ParameterName_Obj;
        private static readonly string s_ParameterName_Reader;
        private static readonly string s_ParameterName_Writer;

        private static readonly string s_SerializeMethodName;
        private static readonly string s_DeserializeMethodName;

        private static readonly Type s_IsReadOnlyAttributeType;
        private static readonly ConstructorInfo s_IsReadOnlyAttributeCtor;
        private static readonly byte[] s_IsReadOnlyAttributeBytes;

        private static readonly Type[] s_ReaderPtrTypes;

        private static readonly Type[][] s_EmptyTypes1;
        private static readonly Type[][] s_EmptyTypes2;
        private static readonly Type[][] s_InAttr1;
        private static readonly Type[][] s_InAttr2;

        private static readonly MethodAttributes s_OverrideMethodAttributes;

        static SerializeProxyUtility()
        {
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SerializeProxies.dll"), AssemblyBuilderAccess.Run);
            s_ModuleBuilder = builder.DefineDynamicModule("SerializeProxies");

            s_TypeAttributes = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

            s_ParameterName_Obj = "obj";
            s_ParameterName_Reader = "reader";
            s_ParameterName_Writer = "writer";

            s_SerializeMethodName = "Serialize";
            s_DeserializeMethodName = "Deserialize";

            s_IsReadOnlyAttributeType = Type.GetType("System.Runtime.CompilerServices.IsReadOnlyAttribute");
            s_IsReadOnlyAttributeCtor = s_IsReadOnlyAttributeType.GetConstructor(Type.EmptyTypes);
            s_IsReadOnlyAttributeBytes = new byte[] { 1, 0, 0, 0 };

            s_ReaderPtrTypes = new Type[] { typeof(UnmanagedReader*).MakeByRefType() };

            s_EmptyTypes1 = new Type[][] { Type.EmptyTypes };
            s_EmptyTypes2 = new Type[][] { Type.EmptyTypes, Type.EmptyTypes };
            s_InAttr1 = new Type[][] { new Type[] { typeof(InAttribute) } };
            s_InAttr2 = new Type[][] { new Type[] { typeof(InAttribute) }, new Type[] { typeof(InAttribute) } };

            s_OverrideMethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        }

        public static Type GenerateProxy(Type objType)
        {
            string typeName = GetProxyTypeName(objType);

            Type interfaceType = typeof(ISerializeProxy<>).MakeGenericType(objType);

            TypeBuilder builder = s_ModuleBuilder.DefineType(typeName, s_TypeAttributes, typeof(object), new Type[] { interfaceType });

            builder.DefineDefaultConstructor(MethodAttributes.Public);

            DefineSerializeMethod(builder, objType, interfaceType);

            DefineDeserializeMethod(builder, objType, interfaceType);

            return builder.CreateType();
        }

        private static string GetProxyTypeName(Type objType)
        {
            return objType.FullName + "SerializeProxy";
        }

        private static void DefineSerializeMethod(TypeBuilder builder, Type objType, Type interfaceType)
        {
            MethodBuilder method = builder.DefineMethod(s_SerializeMethodName,
                                                        s_OverrideMethodAttributes,
                                                        CallingConventions.Standard,
                                                        typeof(void),
                                                        Type.EmptyTypes,
                                                        Type.EmptyTypes,
                                                        new Type[] { objType.MakeByRefType(), typeof(UnmanagedWriter*).MakeByRefType() },
                                                        s_InAttr2, s_EmptyTypes2);

            method.DefineParameter(1, ParameterAttributes.In, s_ParameterName_Obj).SetCustomAttribute(s_IsReadOnlyAttributeCtor, s_IsReadOnlyAttributeBytes);
            method.DefineParameter(2, ParameterAttributes.In, s_ParameterName_Writer).SetCustomAttribute(s_IsReadOnlyAttributeCtor, s_IsReadOnlyAttributeBytes);

            ILGenerator il = method.GetILGenerator();

            il.EmitTypeSerialize(objType, 0);

            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(s_SerializeMethodName));
        }

        private static void DefineDeserializeMethod(TypeBuilder builder, Type objType, Type interfaceType)
        {
            MethodBuilder method = builder.DefineMethod(s_DeserializeMethodName,
                                                        s_OverrideMethodAttributes,
                                                        CallingConventions.Standard,
                                                        objType,
                                                        Type.EmptyTypes,
                                                        Type.EmptyTypes,
                                                        s_ReaderPtrTypes,
                                                        s_InAttr1, s_EmptyTypes1);

            method.DefineParameter(1, ParameterAttributes.In, s_ParameterName_Reader).SetCustomAttribute(s_IsReadOnlyAttributeCtor, s_IsReadOnlyAttributeBytes);

            ILGenerator il = method.GetILGenerator();

            bool complex = SerializationUtility.IsTrulyComplex(objType);

            if (complex)
            {
                il.DeclareLocal(objType);

                if (objType.IsArray)
                {
                    il.Emit(OpCodes.Newarr, objType.GetElementType());
                    il.Emit(OpCodes.Stloc_0);
                }
                else if (objType.IsValueType)
                {
                    il.Emit(OpCodes.Ldloca_S, (byte)0);
                    il.Emit(OpCodes.Initobj, objType);
                }
                else
                {
                    il.Emit(OpCodes.Newobj, objType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc_0);
                }
            }

            il.EmitTypeDeserialize(objType, complex, 0);

            if (complex)
            {
                il.Emit(OpCodes.Ldloc_0);
            }

            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(s_DeserializeMethodName));
        }
    }
}
