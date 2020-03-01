using Accelbuffer.Injection.IL.Serializers.Progress;
using Accelbuffer.Properties;
using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Accelbuffer.Injection.IL.Serializers
{
    internal static class SerializerInjector
    {
        private static readonly SerializerGenerationProgress s_Progress0 = new CtorDefineProgress();
        private static readonly SerializerGenerationProgress s_Progress1 = new SerializeMethodDefineProgress();
        private static readonly SerializerGenerationProgress s_Progress2 = new DeserializeMethodDefineProgress();

        public static ITypeSerializer<T> Inject<T>()
        {
            return (ITypeSerializer<T>)Activator.CreateInstance(GetSerializerType<T>());
        }

        public static Type Inject(Type objectType, AccelTypeInfo typeInfo)
        {
            return InjectType(objectType, typeInfo);
        }

        private static Type InjectType<T>()
        {
            Type objType = typeof(T);
            TypeBuilder builder = RuntimeInjector.DefineSerializerType(objType, out Type interfaceType);
            AccelTypeInfo.GetTypeInfo(objType, out var fields, out var before, out var after, out var fieldCount, out var hasContinuousSerialIndexes);
            
            s_Progress0.Execute(objType, interfaceType, builder, fields, fieldCount, hasContinuousSerialIndexes, before, after);
            s_Progress1.Execute(objType, interfaceType, builder, fields, fieldCount, hasContinuousSerialIndexes, before, after);
            s_Progress2.Execute(objType, interfaceType, builder, fields, fieldCount, hasContinuousSerialIndexes, before, after);

            return builder.CreateType();
        }

        private static Type InjectType(Type objType, AccelTypeInfo typeInfo)
        {
            TypeBuilder builder = RuntimeInjector.DefineSerializerType(objType, out Type interfaceType);
            AccelTypeInfo.GetTypeInfo(typeInfo, out var fields, out var before, out var after, out var fieldCount, out var hasContinuousSerialIndexes);

            s_Progress0.Execute(objType, interfaceType, builder, fields, fieldCount, hasContinuousSerialIndexes, before, after);
            s_Progress1.Execute(objType, interfaceType, builder, fields, fieldCount, hasContinuousSerialIndexes, before, after);
            s_Progress2.Execute(objType, interfaceType, builder, fields, fieldCount, hasContinuousSerialIndexes, before, after);

            return builder.CreateType();
        }

        private static Type GetSerializerType<T>()
        {
            Type objectType = typeof(T);
            IReadOnlyDictionary<RuntimeTypeHandle, RuntimeTypeHandle> typeMap = SerializerBinder.GetTypeMap();

            if (typeMap.TryGetValue(objectType.TypeHandle, out RuntimeTypeHandle handle))
            {
                return Type.GetTypeFromHandle(handle);
            }

            SerializeByAttribute attr = objectType.GetCustomAttribute<SerializeByAttribute>(true);

            Type serializerType = null;

            if (attr == null)
            {
                if (objectType.IsArray)
                {
                    if (objectType.GetArrayRank() > 1)
                    {
                        throw new NotSupportedException(Resources.NotSupportHighRankArray);
                    }

                    return typeof(ArraySerializer<>).MakeGenericType(objectType.GetElementType());
                }

                if (objectType.IsGenericType && GetGenericSerializerType(objectType, typeMap, ref serializerType))
                {
                    return serializerType;
                }

                if (GetCollectionSerializerType(objectType, typeMap, ref serializerType))
                {
                    return serializerType;
                }

                if (!IsInjectable(objectType))
                {
                    throw new NotSupportedException(string.Format(Resources.NotSupportTypeInjection, objectType));
                }

                return InjectType<T>();
            }
            else
            {
                serializerType = attr.SerializerType;

                if (serializerType == null)
                {
                    throw new ArgumentNullException(nameof(SerializeByAttribute.SerializerType), Resources.SerializerTypeIsNull);
                }

                if (serializerType.IsGenericTypeDefinition)
                {
                    serializerType = serializerType.MakeGenericType(objectType.GenericTypeArguments);
                }

                if (!typeof(ITypeSerializer<T>).IsAssignableFrom(serializerType))
                {
                    throw new NotSupportedException(Resources.InvalidSerializerType);
                }

                return serializerType;
            }
        }

        private static bool GetCollectionSerializerType(Type objectType, IReadOnlyDictionary<RuntimeTypeHandle, RuntimeTypeHandle> typeMap, ref Type serializerType)
        {
            Type interfaceType;

            if ((interfaceType = objectType.GetInterface(typeof(IList<>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(IDictionary<,>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(ICollection<>).Name)) != null)
            {
                serializerType = Type.GetTypeFromHandle(typeMap[interfaceType.GetGenericTypeDefinition().TypeHandle]);
                Type[] args = new Type[interfaceType.GenericTypeArguments.Length + 1];

                args[0] = objectType;
                args[1] = interfaceType.GenericTypeArguments[0];

                if (args.Length == 3)
                {
                    args[2] = interfaceType.GenericTypeArguments[1];
                }

                serializerType = serializerType.MakeGenericType(args);
                return true;
            }

            return false;
        }

        private static bool GetGenericSerializerType(Type objectType, IReadOnlyDictionary<RuntimeTypeHandle, RuntimeTypeHandle> typeMap, ref Type serializerType)
        {
            if (typeMap.TryGetValue(objectType.GetGenericTypeDefinition().TypeHandle, out RuntimeTypeHandle handle))
            {
                serializerType = Type.GetTypeFromHandle(handle).MakeGenericType(objectType.GenericTypeArguments);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInjectable(Type objectType)
        {
            return objectType.IsValueType || HasDefaultCtor(objectType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasDefaultCtor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
