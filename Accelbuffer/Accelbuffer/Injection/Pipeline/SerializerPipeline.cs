using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Accelbuffer.Injection
{
#if DEBUG
    public
#else
    internal
#endif
    static class SerializerPipeline
    {
#if DEBUG
        private static readonly AssemblyBuilder s_AssemblyBuilder;
#endif

        private static readonly ModuleBuilder s_ModuleBuilder;
        private static readonly TypeAttributes s_TypeAttributes;
        private static readonly BindingFlags s_FieldBindingFlags;
        private static readonly BindingFlags s_MessageCallbackBindingFlags;
        private static readonly Type[] s_InterfaceTypeArray;
        private static readonly List<FieldData> s_FieldData;
        private static readonly List<MethodData> s_MethodData;

        private static readonly SerializerGenerationProgress[] s_Progress;

        private static readonly DataComparer s_Comparer;

        static SerializerPipeline()
        {
#if DEBUG
            s_AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Resources.SerializerAssemblyName), AssemblyBuilderAccess.RunAndSave);
            s_ModuleBuilder = s_AssemblyBuilder.DefineDynamicModule(Resources.SerializerModuleName, Resources.SerializerAssemblyName);
#else
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Resources.SerializerAssemblyName), AssemblyBuilderAccess.Run);
            s_ModuleBuilder = builder.DefineDynamicModule(Resources.SerializerModuleName);
#endif

            s_TypeAttributes = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public;
            s_FieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            s_MessageCallbackBindingFlags = BindingFlags.Public | BindingFlags.Instance;
            s_InterfaceTypeArray = new Type[1];
            s_FieldData = new List<FieldData>(10);
            s_MethodData = new List<MethodData>(2);

            s_Progress = new SerializerGenerationProgress[]
            {
                new CtorDefineProgress(),
                new SerializeMethodDefineProgress(),
                new DeserializeMethodDefineProgress()
            };

            s_Comparer = new DataComparer();
        }

        internal static Type InjectType<T>()
        {
            Type objType = typeof(T);
            Type interfaceType = typeof(ITypeSerializer<T>);
            TypeBuilder builder = CreateBuilder(objType, interfaceType);

            GetSerializedFields(objType);
            GetMessageCallbacks(objType);

            SerializerOption option = objType.GetCustomAttribute<CompactLayoutAttribute>(true) == null 
                ? SerializerOption.Normal : SerializerOption.CompactLayout;

            for (int i = 0; i < s_Progress.Length; i++)
            {
                s_Progress[i].Execute(objType, interfaceType, builder, s_FieldData, s_MethodData, option);
            }

            return builder.CreateType();
        }

        private static string GetProxyTypeName(Type objType)
        {
            return objType.FullName + Resources.SerializerTypeSuffix;
        }

        private static TypeBuilder CreateBuilder(Type objType, Type interfaceType)
        {
            string typeName = GetProxyTypeName(objType);
            s_InterfaceTypeArray[0] = interfaceType;
            return s_ModuleBuilder.DefineType(typeName, s_TypeAttributes, typeof(object), s_InterfaceTypeArray);
        }

        private static void GetSerializedFields(Type objType)
        {
            FieldInfo[] allFields = objType.GetFields(s_FieldBindingFlags);
            s_FieldData.Clear();

            if (allFields == null)
            {
                return;
            }

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
                    CheckRefAttribute checkRef = field.GetCustomAttribute<CheckRefAttribute>(true);
                    s_FieldData.Add(new FieldData(field, attribute.Index, checkRef != null));
                }
            }
            
            s_FieldData.Sort(s_Comparer);
        }

        private static void GetMessageCallbacks(Type objType)
        {
            MethodInfo[] methods = objType.GetMethods(s_MessageCallbackBindingFlags);
            s_MethodData.Clear();

            if (methods == null)
            {
                return;
            }            

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                ParameterInfo[] parameters = method.GetParameters();

                if (parameters != null && parameters.Length != 0)
                {
                    continue;
                }

                OnBeforeSerializationAttribute attr1 = method.GetCustomAttribute<OnBeforeSerializationAttribute>(true);
                OnAfterDeserializationAttribute attr2 = method.GetCustomAttribute<OnAfterDeserializationAttribute>(true);

                if (attr1 != null)
                {
                    s_MethodData.Add(new MethodData(method, AccelbufferCallback.OnBeforeSerialization, attr1.Priority));
                }

                if (attr2 != null)
                {
                    s_MethodData.Add(new MethodData(method, AccelbufferCallback.OnAfterDeserialization, attr2.Priority));
                }
            }

            s_MethodData.Sort(s_Comparer);
        }

#if DEBUG
        public static void SaveAssembly(string fileName)
        {
            s_AssemblyBuilder.Save(fileName);
        }
#endif

        private sealed class DataComparer : IComparer<FieldData>, IComparer<MethodData>
        {
            public int Compare(FieldData x, FieldData y)
            {
                return x.Index - y.Index;
            }

            public int Compare(MethodData x, MethodData y)
            {
                return x.Priority - y.Priority;
            }
        }
    }
}
