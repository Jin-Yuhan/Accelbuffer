using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Accelbuffer.SerializeProxyUtility;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对序列化代理的操作权限
    /// </summary>
    public static class SerializeProxyInjector
    {
        private static readonly Dictionary<Type, Type> s_ProxyMap;

        /// <summary>
        /// 获取默认数字类型选项
        /// </summary>
        public static Number DefaultNumberType { get; }

        /// <summary>
        /// 获取默认字符编码
        /// </summary>
        public static CharEncoding DefaultCharEncoding { get; }

        static SerializeProxyInjector()
        {
            s_ProxyMap = new Dictionary<Type, Type>()
            {
                [typeof(sbyte)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(byte)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(short)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(ushort)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(int)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(uint)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(long)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(ulong)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(float)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(double)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(bool)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(char)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(string)] = typeof(PrimitiveTypeSerializeProxy),
                [typeof(List<>)] = typeof(ListSerializeProxy<>),
                [typeof(IList<>)] = typeof(ListSerializeProxy<,>),
                [typeof(ICollection<>)] = typeof(CollectionSerializeProxy<,>),
                [typeof(ISerializableEnumerable<>)] = typeof(SerializableEnumerableSerializeProxy<,>),
                [typeof(Dictionary<,>)] = typeof(DictionarySerializeProxy<,>),
                [typeof(IDictionary<,>)] = typeof(DictionarySerializeProxy<,,>)
            };

            DefaultNumberType = Number.Var;
            DefaultCharEncoding = CharEncoding.UTF8;
        }

        /// <summary>
        /// 添加一个序列化代理的绑定，如果已经存在，则覆盖
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        /// <typeparam name="TProxy">被绑定的代理类型</typeparam>
        public static void AddBinding<TObject, TProxy>() where TProxy : ISerializeProxy<TObject>
        {
            Type objectType = typeof(TObject);

            if (s_ProxyMap.ContainsKey(objectType))
            {
                s_ProxyMap[objectType] = typeof(TProxy);
            }
            else
            {
                s_ProxyMap.Add(objectType, typeof(TProxy));
            }
        }

        /// <summary>
        /// 添加一个序列化代理的绑定，如果已经存在，则覆盖
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        /// <param name="proxyType">被绑定的代理类型</param>
        /// <exception cref="InvalidCastException"><paramref name="proxyType"/>类型错误</exception>
        public static void AddBinding(Type objectType, Type proxyType)
        {
            Type expectedType = typeof(ISerializeProxy<>).MakeGenericType(objectType);

            if (!expectedType.IsAssignableFrom(proxyType))
            {
                throw new InvalidCastException($"无法将代理装换为{expectedType.Name}类型");
            }

            if (s_ProxyMap.ContainsKey(objectType))
            {
                s_ProxyMap[objectType] = proxyType;
            }
            else
            {
                s_ProxyMap.Add(objectType, proxyType);
            }
        }

        /// <summary>
        /// 获取是否已经存在绑定
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        /// <returns>如果已经存在，返回true，否则，false</returns>
        public static bool HasBinding<TObject>()
        {
            return s_ProxyMap.ContainsKey(typeof(TObject));
        }

        /// <summary>
        /// 获取是否已经存在绑定
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        /// <returns>如果已经存在，返回true，否则，false</returns>
        public static bool HasBinding(Type objectType)
        {
            return s_ProxyMap.ContainsKey(objectType);
        }

        internal static List<FieldData> GetSerializedFields(this Type objType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo[] allFields = objType.GetFields(flags);
            List<FieldData> fields = new List<FieldData>(allFields.Length);

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
                    fields.Add(new FieldData(field, attribute.Index));
                }
            }

            fields.Sort((f1, f2) => f1.Index - f2.Index);
            return fields;
        }

        internal static ISerializeProxy<T> Inject<T>()
        {
            return (ISerializeProxy<T>)Activator.CreateInstance(GetProxyType<T>());
        }

        private static Type GetProxyType<T>()
        {
            Type proxyType;
            Type objectType = typeof(T);
            SerializeByAttribute attr = objectType.GetCustomAttribute<SerializeByAttribute>(true);

            if (attr == null)
            {
                if (objectType.IsArray)
                {
                    if (objectType.GetArrayRank() > 1)
                    {
                        throw new NotSupportedException($"不支持多维数组序列化与反序列化");
                    }

                    return typeof(ArraySerializeProxy<>).MakeGenericType(objectType.GetElementType());
                }

                if (s_ProxyMap.TryGetValue(objectType, out proxyType))
                {
                    return proxyType;
                }

                if (objectType.IsGenericType && GetGenericProxyType(objectType, ref proxyType))
                {
                    return proxyType;
                }

                if (GetCollectionProxyType(objectType, ref proxyType))
                {
                    return proxyType;
                }

                if (!IsInjectable(objectType))
                {
                    throw new NotSupportedException($"无法为类型{objectType}注入代理");
                }

                return GenerateProxy(objectType);
            }
            else
            {
                proxyType = attr.ProxyType;

                if (proxyType == null)
                {
                    throw new NotSupportedException("序列化代理类型不能为null");
                }

                if (proxyType.IsGenericTypeDefinition)
                {
                    proxyType = proxyType.MakeGenericType(objectType.GenericTypeArguments);
                }

                if (!typeof(ISerializeProxy<T>).IsAssignableFrom(proxyType))
                {
                    throw new NotSupportedException($"{proxyType.Name}类型不是有效的序列化代理类型");
                }

                return proxyType;
            }
        }

        private static bool GetCollectionProxyType(Type objectType, ref Type proxyType)
        {
            Type interfaceType;

            if ((interfaceType = objectType.GetInterface(typeof(IList<>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(IDictionary<,>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(ISerializableEnumerable<>).Name)) != null
             || (interfaceType = objectType.GetInterface(typeof(ICollection<>).Name)) != null)
            {
                proxyType = s_ProxyMap[interfaceType.GetGenericTypeDefinition()];
                Type[] args = new Type[interfaceType.GenericTypeArguments.Length + 1];

                args[0] = objectType;
                args[1] = interfaceType.GenericTypeArguments[0];

                if (args.Length == 3)
                {
                    args[2] = interfaceType.GenericTypeArguments[1];
                }

                proxyType = proxyType.MakeGenericType(args);
                return true;
            }

            return false;
        }

        private static bool GetGenericProxyType(Type objectType, ref Type proxyType)
        {
            if (s_ProxyMap.TryGetValue(objectType.GetGenericTypeDefinition(), out proxyType))
            {
                proxyType = proxyType.MakeGenericType(objectType.GenericTypeArguments);
                return true;
            }

            return false;
        }

        private static bool IsInjectable(Type objectType)
        {
            return !SerializationUtility.IsTrulyComplex(objectType) || objectType.IsValueType || HasDefaultCtor(objectType);
        }

        private static bool HasDefaultCtor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
