using Accelbuffer.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Accelbuffer.Injection.IL
{
    /// <summary>
    /// 提供在运行时注入类型的接口
    /// </summary>
    public static class RuntimeInjector
    {
        private static readonly MethodAttributes s_MethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        private static readonly CallingConventions s_CallingConventions = CallingConventions.Standard;
        private static readonly ModuleBuilder s_ModuleBuilder;
        private static Type[] s_DeserializeMethodArgs = null;

        static RuntimeInjector()
        {
#if DEBUG
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Resources.RuntimeAssemblyName), AssemblyBuilderAccess.RunAndSave);
            s_ModuleBuilder = builder.DefineDynamicModule(Resources.RuntimeModuleName, Resources.RuntimeAssemblyName);
#else
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Resources.RuntimeAssemblyName), AssemblyBuilderAccess.RunAndCollect);
            s_ModuleBuilder = builder.DefineDynamicModule(Resources.RuntimeModuleName);
#endif
        }

        /// <summary>
        /// 定义一个新的公开类型
        /// </summary>
        /// <param name="name">类型的名称</param>
        /// <param name="isSealed">指示类型是否是密封类型</param>
        /// <param name="declaringTypeBuilder">声明此类型的类型构造器，如果这个值不为null，则类型是嵌套类型</param>
        /// <param name="baseType">类型的基类，如果是值类型，应该传入<see cref="ValueType"/></param>
        /// <param name="interfaces">类型实现的所有接口</param>
        /// <returns>新类型的构造器</returns>
        public static TypeBuilder DefinePublicType(string name, bool isSealed, TypeBuilder declaringTypeBuilder, Type baseType, params Type[] interfaces)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            TypeAttributes attributes = TypeAttributes.Class | TypeAttributes.SequentialLayout;

            if (isSealed)
            {
                attributes |= TypeAttributes.Sealed;
            }

            if (declaringTypeBuilder != null)
            {
                attributes |= TypeAttributes.NestedPublic;
                return declaringTypeBuilder.DefineNestedType(name, attributes, baseType, interfaces);
            }

            attributes |= TypeAttributes.Public;
            return s_ModuleBuilder.DefineType(name, attributes, baseType, interfaces);
        }

        /// <summary>
        /// 创建给定类型的序列化代理类型
        /// </summary>
        /// <param name="objType">该序列化代理序列化的对象类型</param>
        /// <param name="interfaceType">该序列化代理序列化实现的接口类型，为<see cref="ITypeSerializer{T}"/>的封闭类型</param>
        /// <returns>序列化代理类型的构造器</returns>
        public static TypeBuilder DefineSerializerType(Type objType, out Type interfaceType)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            string name = GetSerializerName(objType);
            interfaceType = typeof(ITypeSerializer<>).MakeGenericType(objType);
            return DefinePublicType(name, true, null, typeof(object), interfaceType);
        }

        /// <summary>
        /// 获取给定类型的序列化代理名称
        /// </summary>
        /// <param name="objType">该序列化代理序列化的对象类型</param>
        /// <returns>给定类型的序列化代理名称</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerName(Type objType)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            return objType.FullName + Resources.SerializerTypeSuffix;
        }

        /// <summary>
        /// 为给定的序列化代理创建<see cref="ITypeSerializer{T}.Serialize(T, ref AccelWriter)"/>方法
        /// </summary>
        /// <param name="serializerTypeBuilder">序列化代理类型的构造器</param>
        /// <param name="objType">该序列化代理序列化的对象类型</param>
        /// <param name="interfaceType">该序列化代理序列化实现的接口类型，应该为<see cref="ITypeSerializer{T}"/>的封闭类型</param>
        /// <returns><see cref="ITypeSerializer{T}.Serialize(T, ref AccelWriter)"/>方法的构造器</returns>
        public static MethodBuilder DefineSerializeMethod(TypeBuilder serializerTypeBuilder, Type objType, Type interfaceType)
        {
            if (serializerTypeBuilder == null)
            {
                throw new ArgumentNullException(nameof(serializerTypeBuilder));
            }

            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            string methodName = Resources.SerializeMethodName;
            Type returnType = typeof(void);
            Type[] args = new Type[] { objType, typeof(AccelWriter).MakeByRefType() };
            MethodBuilder builder = serializerTypeBuilder.DefineMethod(methodName, s_MethodAttributes, s_CallingConventions, returnType, args);
            serializerTypeBuilder.DefineMethodOverride(builder, interfaceType.GetMethod(methodName));
            return builder;
        }

        /// <summary>
        /// 为给定的序列化代理创建<see cref="ITypeSerializer{T}.Deserialize(ref AccelReader)"/>方法
        /// </summary>
        /// <param name="serializerTypeBuilder">序列化代理类型的构造器</param>
        /// <param name="objType">该序列化代理序列化的对象类型</param>
        /// <param name="interfaceType">该序列化代理序列化实现的接口类型，应该为<see cref="ITypeSerializer{T}"/>的封闭类型</param>
        /// <returns><see cref="ITypeSerializer{T}.Deserialize(ref AccelReader)"/>方法的构造器</returns>
        public static MethodBuilder DefineDeserializeMethod(TypeBuilder serializerTypeBuilder, Type objType, Type interfaceType)
        {
            if (serializerTypeBuilder == null)
            {
                throw new ArgumentNullException(nameof(serializerTypeBuilder));
            }

            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            string methodName = Resources.DeserializeMethodName;
            Type[] args = s_DeserializeMethodArgs ?? (s_DeserializeMethodArgs = new Type[] { typeof(AccelReader).MakeByRefType() });
            MethodBuilder builder = serializerTypeBuilder.DefineMethod(methodName, s_MethodAttributes, s_CallingConventions, objType, args);
            serializerTypeBuilder.DefineMethodOverride(builder, interfaceType.GetMethod(methodName));
            return builder;
        }

        /// <summary>
        /// 保存程序集至磁盘，名称为DynamicAssembly.dll和RuntimeTypes.dll。
        /// 这个方法只在DEBUG下执行。
        /// </summary>
        [Conditional("DEBUG")]
        public static void SaveAssembly()
        {
            (s_ModuleBuilder.Assembly as AssemblyBuilder).Save(Resources.SavedAssemblyName);
        }
    }
}
