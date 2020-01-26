using Accelbuffer.Memory;
using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 公开对序列化代理绑定的接口
    /// </summary>
    public static class SerializerInjector
    {
        internal static class InternalCache<T>
        {
            public static readonly ITypeSerializer<T> Serializer;
            public static readonly MemoryAllocator Allocator;

            static InternalCache()
            {
                Serializer = Inject<T>();
                Allocator = MemoryAllocator.Alloc<T>(Serializer);
            }

            public static void Initialize() { }
        }

        private static readonly Dictionary<Type, Type> s_TypeMap;

        static SerializerInjector()
        {
            s_TypeMap = new Dictionary<Type, Type>(33)
            {
                [typeof(sbyte)] = typeof(PrimitiveTypeSerializer),
                [typeof(byte)] = typeof(PrimitiveTypeSerializer),
                [typeof(short)] = typeof(PrimitiveTypeSerializer),
                [typeof(ushort)] = typeof(PrimitiveTypeSerializer),
                [typeof(int)] = typeof(PrimitiveTypeSerializer),
                [typeof(uint)] = typeof(PrimitiveTypeSerializer),
                [typeof(long)] = typeof(PrimitiveTypeSerializer),
                [typeof(ulong)] = typeof(PrimitiveTypeSerializer),
                [typeof(float)] = typeof(PrimitiveTypeSerializer),
                [typeof(double)] = typeof(PrimitiveTypeSerializer),
                [typeof(decimal)] = typeof(PrimitiveTypeSerializer),
                [typeof(bool)] = typeof(PrimitiveTypeSerializer),
                [typeof(char)] = typeof(PrimitiveTypeSerializer),
                [typeof(string)] = typeof(PrimitiveTypeSerializer),
                [typeof(Type)] = typeof(TypeSerializer),

                [typeof(List<>)] = typeof(ListSerializer<>),
                [typeof(IList<>)] = typeof(ListSerializer<,>),
                [typeof(ICollection<>)] = typeof(CollectionSerializer<,>),
                [typeof(ISerializableEnumerable<>)] = typeof(SerializableEnumerableSerializer<,>),
                [typeof(Dictionary<,>)] = typeof(DictionarySerializer<,>),
                [typeof(IDictionary<,>)] = typeof(DictionarySerializer<,,>),

#if UNITY
                [typeof(UnityEngine.Vector2)] = typeof(UnitySerializer),
                [typeof(UnityEngine.Vector3)] = typeof(UnitySerializer),
                [typeof(UnityEngine.Vector4)] = typeof(UnitySerializer),
                [typeof(UnityEngine.Vector2Int)] = typeof(UnitySerializer),
                [typeof(UnityEngine.Vector3Int)] = typeof(UnitySerializer),
                [typeof(UnityEngine.Quaternion)] = typeof(UnitySerializer)
#endif
            };
        }

        /// <summary>
        /// 添加一个序列化代理的绑定，如果已经存在，则覆盖
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        /// <typeparam name="TSerializer">被绑定的代理类型</typeparam>
        public static void AddBinding<TObject, TSerializer>() where TSerializer : ITypeSerializer<TObject>
        {
            Type objectType = typeof(TObject);

            if (s_TypeMap.ContainsKey(objectType))
            {
                s_TypeMap[objectType] = typeof(TSerializer);
            }
            else
            {
                s_TypeMap.Add(objectType, typeof(TSerializer));
            }
        }

        /// <summary>
        /// 添加一个序列化代理的绑定，如果已经存在，则覆盖
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        /// <param name="serializerType">被绑定的代理类型</param>
        /// <exception cref="SerializerTypeException"><paramref name="serializerType"/>类型错误</exception>
        public static void AddBinding(Type objectType, Type serializerType)
        {
            Type expectedType = typeof(ITypeSerializer<>).MakeGenericType(objectType);

            if (!expectedType.IsAssignableFrom(serializerType))
            {
                throw new SerializerTypeException(Resources.InvalidSerializerType);
            }

            if (s_TypeMap.ContainsKey(objectType))
            {
                s_TypeMap[objectType] = serializerType;
            }
            else
            {
                s_TypeMap.Add(objectType, serializerType);
            }
        }

        /// <summary>
        /// 获取是否已经存在绑定
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        /// <param name="checkAttribute">指示是否检查类型的<see cref="SerializeByAttribute"/></param>
        /// <returns>如果已经存在，返回true，否则，false</returns>
        public static bool HasBinding<TObject>(bool checkAttribute)
        {
            return s_TypeMap.ContainsKey(typeof(TObject)) 
                || (checkAttribute && typeof(TObject).GetCustomAttribute<SerializeByAttribute>()?.SerializerType != null);
        }

        /// <summary>
        /// 获取是否已经存在绑定
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        /// <param name="checkAttribute">指示是否检查类型的<see cref="SerializeByAttribute"/></param>
        /// <returns>如果已经存在，返回true，否则，false</returns>
        public static bool HasBinding(Type objectType, bool checkAttribute)
        {
            return s_TypeMap.ContainsKey(objectType)
                || (checkAttribute && objectType.GetCustomAttribute<SerializeByAttribute>()?.SerializerType != null);
        }

        internal static ITypeSerializer<T> Inject<T>()
        {
            return (ITypeSerializer<T>)Activator.CreateInstance(GetSerializerType<T>());
        }

        private static Type GetSerializerType<T>()
        {
            Type objectType = typeof(T);

            if (s_TypeMap.TryGetValue(objectType, out Type serializerType))
            {
                return serializerType;
            }

            SerializeByAttribute attr = objectType.GetCustomAttribute<SerializeByAttribute>(true);

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

                if (objectType.IsGenericType && GetGenericSerializerType(objectType, ref serializerType))
                {
                    return serializerType;
                }

                if (GetCollectionSerializerType(objectType, ref serializerType))
                {
                    return serializerType;
                }

                if (!IsInjectable(objectType))
                {
                    throw new NotSupportedException(string.Format(Resources.NotSupportTypeInjection, objectType));
                }

                if (!objectType.IsSerializable)
                {
                    throw new NotSupportedException(string.Format(Resources.NotSerializable, objectType));
                }

                return SerializerPipeline.InjectType<T>();
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

        private static bool GetCollectionSerializerType(Type objectType, ref Type serializerType)
        {
            Type interfaceType;

            if ((interfaceType = objectType.GetInterface(typeof(IList<>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(IDictionary<,>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(ISerializableEnumerable<>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(ICollection<>).Name)) != null)
            {
                serializerType = s_TypeMap[interfaceType.GetGenericTypeDefinition()];
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

        private static bool GetGenericSerializerType(Type objectType, ref Type serializerType)
        {
            if (s_TypeMap.TryGetValue(objectType.GetGenericTypeDefinition(), out serializerType))
            {
                serializerType = serializerType.MakeGenericType(objectType.GenericTypeArguments);
                return true;
            }

            return false;
        }

        private static bool IsInjectable(Type objectType)
        {
            return objectType.IsValueType || HasDefaultCtor(objectType);
        }

        private static bool HasDefaultCtor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
