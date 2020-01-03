using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Accelbuffer.Runtime.Injection
{
#if DEBUG
    public
#else
    internal
#endif
    static class SerializeProxyPipeline
    {
#if DEBUG
        private static readonly AssemblyBuilder s_AssemblyBuilder;
#endif

        private static readonly ModuleBuilder s_ModuleBuilder;
        private static readonly TypeAttributes s_TypeAttributes;
        private static readonly BindingFlags s_MessageCallbackBindingFlags;
        private static readonly Type[] s_InterfaceTypeArray;
        private static readonly List<FieldData> s_FieldData;
        private static readonly List<MethodData> s_MethodData;

        private static ProxyGenerationProgress[] s_Progress;

        static SerializeProxyPipeline()
        {
#if DEBUG
            s_AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SerializeProxies.dll"), AssemblyBuilderAccess.RunAndSave);
            s_ModuleBuilder = s_AssemblyBuilder.DefineDynamicModule("SerializeProxies", "SerializeProxies.dll");
#else
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SerializeProxies.dll"), AssemblyBuilderAccess.Run);
            s_ModuleBuilder = builder.DefineDynamicModule("SerializeProxies");
#endif

            s_TypeAttributes = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public;
            s_MessageCallbackBindingFlags = BindingFlags.Public | BindingFlags.Instance;
            s_InterfaceTypeArray = new Type[1];
            s_FieldData = new List<FieldData>();
            s_MethodData = new List<MethodData>(2);

            s_Progress = new ProxyGenerationProgress[]
            {
                new CtorDefineProgress(),
                new SerializeMethodDefineProgress(),
                new DeserializeMethodDefineProgress()
            };
        }

        internal static Type InjectType<T>()
        {
            Type objType = typeof(T);
            Type interfaceType = typeof(ISerializeProxy<T>);
            TypeBuilder builder = CreateBuilder(objType, interfaceType);

            GetSerializedFields(objType);
            GetMessageCallbacks(objType);

            for (int i = 0; i < s_Progress.Length; i++)
            {
                s_Progress[i].Execute(objType, interfaceType, builder, s_FieldData, s_MethodData);
            }

            return builder.CreateType();
        }

        private static string GetProxyTypeName(Type objType)
        {
            return objType.FullName + "[SerializeProxy]";
        }

        private static TypeBuilder CreateBuilder(Type objType, Type interfaceType)
        {
            string typeName = GetProxyTypeName(objType);
            s_InterfaceTypeArray[0] = interfaceType;
            return s_ModuleBuilder.DefineType(typeName, s_TypeAttributes, typeof(object), s_InterfaceTypeArray);
        }

        private static void GetSerializedFields(Type objType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo[] allFields = objType.GetFields(flags);
            s_FieldData.Clear();

            for (int i = 0; i < allFields.Length; i++)
            {
                FieldInfo field = allFields[i];

                if (field.GetCustomAttribute<CompilerGeneratedAttribute>() != null || field.IsInitOnly)
                {
                    continue;
                }

                FieldIndexAttribute attribute = field.GetCustomAttribute<FieldIndexAttribute>(true);

                if (attribute != null)
                {
                    s_FieldData.Add(new FieldData(field, attribute.Index));
                }
            }

            s_FieldData.Sort((f1, f2) => f1.Index - f2.Index);
        }

        private static void GetMessageCallbacks(Type objType)
        {
            MethodInfo[] methods = objType.GetMethods(s_MessageCallbackBindingFlags);
            s_MethodData.Clear();

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                SerializationCallbackAttribute attr = method.GetCustomAttribute<SerializationCallbackAttribute>(true);

                if (attr != null && method.GetParameters().Length == 0)
                {
                    s_MethodData.Add(new MethodData(method, attr.CallbackMethodType, attr.Priority));
                }
            }

            s_MethodData.Sort((m1, m2) => m1.Priority - m2.Priority);
        }

#if DEBUG
        public static void SaveAssembly(string fileName)
        {
            s_AssemblyBuilder.Save(fileName);
        }
#endif
    }
}
