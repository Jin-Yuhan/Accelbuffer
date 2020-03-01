using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 公开对序列化代理的绑定接口
    /// </summary>
    public static class SerializerBinder
    {
        private static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> s_TypeMap;

        static SerializerBinder()
        {
            s_TypeMap = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>(45, new TypeHandleComparer())
            {
                [typeof(object).TypeHandle] = typeof(ObjectSerializer).TypeHandle,
                [typeof(sbyte).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(byte).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(short).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(ushort).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(int).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(uint).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(long).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(ulong).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(float).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(double).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(decimal).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(bool).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(char).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(string).TypeHandle] = typeof(PrimitiveTypeSerializer).TypeHandle,
                [typeof(VInt).TypeHandle] = typeof(VariantSerializer).TypeHandle,
                [typeof(VUInt).TypeHandle] = typeof(VariantSerializer).TypeHandle,
                [typeof(IntPtr).TypeHandle] = typeof(IntPtrSerializer).TypeHandle,
                [typeof(UIntPtr).TypeHandle] = typeof(IntPtrSerializer).TypeHandle,
                [typeof(Nullable<>).TypeHandle] = typeof(NullableSerializer<>).TypeHandle,
                [typeof(Type).TypeHandle] = typeof(TypeSerializer).TypeHandle,

                [typeof(Guid).TypeHandle] = typeof(OtherTypeSerializer).TypeHandle,
                [typeof(TimeSpan).TypeHandle] = typeof(OtherTypeSerializer).TypeHandle,
                [typeof(DateTime).TypeHandle] = typeof(OtherTypeSerializer).TypeHandle,
                [typeof(DateTimeOffset).TypeHandle] = typeof(OtherTypeSerializer).TypeHandle,

                [typeof(KeyValuePair<,>).TypeHandle] = typeof(KeyValuePairSerializer<,>).TypeHandle,
                [typeof(List<>).TypeHandle] = typeof(ListSerializer<>).TypeHandle,
                [typeof(IList<>).TypeHandle] = typeof(ListSerializer<,>).TypeHandle,
                [typeof(ICollection<>).TypeHandle] = typeof(CollectionSerializer<,>).TypeHandle,
                [typeof(Dictionary<,>).TypeHandle] = typeof(DictionarySerializer<,>).TypeHandle,
                [typeof(IDictionary<,>).TypeHandle] = typeof(DictionarySerializer<,,>).TypeHandle,

#if UNITY
                [typeof(UnityEngine.Vector2).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Vector3).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Vector4).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Vector2Int).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Vector3Int).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Quaternion).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Color).TypeHandle] = typeof(UnitySerializer).TypeHandle,
                [typeof(UnityEngine.Color32).TypeHandle] = typeof(UnitySerializer).TypeHandle
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
            RuntimeTypeHandle objectType = typeof(TObject).TypeHandle;
            RuntimeTypeHandle serializerType = typeof(TSerializer).TypeHandle;

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

            RuntimeTypeHandle objectTypeHandle = objectType.TypeHandle;

            if (s_TypeMap.ContainsKey(objectTypeHandle))
            {
                s_TypeMap[objectTypeHandle] = serializerType.TypeHandle;
            }
            else
            {
                s_TypeMap.Add(objectTypeHandle, serializerType.TypeHandle);
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
            return s_TypeMap.ContainsKey(typeof(TObject).TypeHandle) 
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
            return s_TypeMap.ContainsKey(objectType.TypeHandle)
                || (checkAttribute && objectType.GetCustomAttribute<SerializeByAttribute>()?.SerializerType != null);
        }

        internal static IReadOnlyDictionary<RuntimeTypeHandle, RuntimeTypeHandle> GetTypeMap()
        {
            return s_TypeMap;
        }

        private sealed class TypeHandleComparer : IEqualityComparer<RuntimeTypeHandle>
        {
            bool IEqualityComparer<RuntimeTypeHandle>.Equals(RuntimeTypeHandle x, RuntimeTypeHandle y)
            {
                return x.Equals(y);
            }

            int IEqualityComparer<RuntimeTypeHandle>.GetHashCode(RuntimeTypeHandle obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
