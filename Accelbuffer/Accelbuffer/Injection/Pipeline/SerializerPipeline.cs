using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

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
            GetMessageCallbacks(objType, out MethodInfo beforeMethod, out MethodInfo afterMethod);

            for (int i = 0; i < s_Progress.Length; i++)
            {
                s_Progress[i].Execute(objType, interfaceType, builder, s_FieldData, beforeMethod, afterMethod);
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

                if (field.IsInitOnly)
                {
                    continue;
                }

                FieldIndexAttribute attribute = field.GetCustomAttribute<FieldIndexAttribute>(true);

                if (attribute != null)
                {
                    NeverNullAttribute neverNull = field.GetCustomAttribute<NeverNullAttribute>(true);
                    s_FieldData.Add(new FieldData(field, attribute.Index, neverNull != null));
                }
            }
            
            s_FieldData.Sort(s_Comparer);
        }

        private static void GetMessageCallbacks(Type objType, out MethodInfo beforeMethod, out MethodInfo afterMethod)
        {
            MethodInfo[] methods = objType.GetMethods(s_MessageCallbackBindingFlags);
            beforeMethod = null;
            afterMethod = null;

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
                    beforeMethod = method;
                }

                if (attr2 != null)
                {
                    afterMethod = method;
                }

                if (beforeMethod != null && afterMethod != null)
                {
                    return;
                }
            }
        }

#if DEBUG
        public static void SaveAssembly(string fileName)
        {
            s_AssemblyBuilder.Save(fileName);
        }
#endif

        private sealed class DataComparer : IComparer<FieldData>
        {
            public int Compare(FieldData x, FieldData y)
            {
                return x.Index - y.Index;
            }
        }
    }
}
