using System;
using System.Collections.Generic;
using System.Reflection;
using static Accelbuffer.SerializeProxyUtility;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对序列化代理的操作权限
    /// </summary>
    public static class SerializeProxyInjector
    {
        private static readonly Dictionary<Type, Type> s_ProxyMap;

        static SerializeProxyInjector()
        {
            s_ProxyMap = new Dictionary<Type, Type>()
            {
                [typeof(List<>)] = typeof(ListSerializeProxy<>),
                [typeof(ISerializableEnumerable<>)] = typeof(SerializableEnumerableSerializeProxy<,>),
                [typeof(Dictionary<,>)] = typeof(DictionarySerializeProxy<,>)
            };
        }

        /// <summary>
        /// 添加一个序列化代理的绑定
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        /// <typeparam name="TProxy">被绑定的代理类型</typeparam>
        /// <exception cref="InvalidOperationException">已经存在一个绑定</exception>
        public static void AddBinding<TObject, TProxy>() where TProxy : ISerializeProxy<TObject>
        {
            Type objectType = typeof(TObject);

            if (s_ProxyMap.ContainsKey(objectType))
            {
                throw new InvalidOperationException($"关于{objectType.Name}的绑定已经存在");
            }

            s_ProxyMap.Add(objectType, typeof(TProxy));
        }

        /// <summary>
        /// 添加一个序列化代理的绑定
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        /// <param name="proxyType">被绑定的代理类型</param>
        /// <exception cref="InvalidCastException"><paramref name="proxyType"/>类型错误</exception>
        /// <exception cref="InvalidOperationException">已经存在一个绑定</exception>
        public static void AddBinding(Type objectType, Type proxyType)
        {
            Type expectedType = typeof(ISerializeProxy<>).MakeGenericType(objectType);

            if (!expectedType.IsAssignableFrom(proxyType))
            {
                throw new InvalidCastException($"无法将代理装换为{expectedType.Name}类型");
            }

            if (s_ProxyMap.ContainsKey(objectType))
            {
                throw new InvalidOperationException($"关于{objectType.Name}的绑定已经存在");
            }

            s_ProxyMap.Add(objectType, proxyType);
        }

        /// <summary>
        /// 移除一个序列化代理的绑定
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        public static void RemoveBinding<TObject>()
        {
            s_ProxyMap.Remove(typeof(TObject));
        }

        /// <summary>
        /// 移除一个序列化代理的绑定
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        public static void RemoveBinding(Type objectType)
        {
            s_ProxyMap.Remove(objectType);
        }

        internal static ISerializeProxy<T> Inject<T>()
        {
            return (ISerializeProxy<T>)Activator.CreateInstance(GetProxyType<T>());
        }

        private static Type GetProxyType<T>()
        {
            Type objectType = typeof(T);

            if (s_ProxyMap.TryGetValue(objectType, out Type proxyType))
            {
                return proxyType;
            }

            Type enumerableType = typeof(ISerializableEnumerable<>);
            Type impType = objectType.GetInterface(enumerableType.Name);

            if (impType != null 
                && GetGenericProxyType(objectType, enumerableType, new Type[] { objectType, impType.GenericTypeArguments[0] }, ref proxyType))
            {
                return proxyType;
            }

            if (objectType.IsGenericType 
                && GetGenericProxyType(objectType, objectType.GetGenericTypeDefinition(), objectType.GenericTypeArguments, ref proxyType))
            {
                return proxyType;
            }

            SerializeContractAttribute attr = objectType.GetCustomAttribute<SerializeContractAttribute>(true);
            proxyType = attr?.ProxyType;

            if (proxyType == null)
            {
                if (!IsInjectable(objectType))
                {
                    throw new NotSupportedException($"无法为类型{objectType}注入代理");
                }

                proxyType = GenerateProxy(objectType);
            }
            else
            {
                if (proxyType.IsGenericTypeDefinition)
                {
                    proxyType = proxyType.MakeGenericType(objectType.GenericTypeArguments);
                }

                if (!typeof(ISerializeProxy<T>).IsAssignableFrom(proxyType))
                {
                    throw new NotSupportedException($"{proxyType.Name}类型不是有效的序列化代理类型");
                }
            }

            s_ProxyMap.Add(objectType, proxyType);
            return proxyType;
        }

        private static bool GetGenericProxyType(Type objectType, Type matchType, Type[] genericTypeArguments, ref Type proxyType)
        {
            if (s_ProxyMap.TryGetValue(matchType, out proxyType))
            {
                proxyType = proxyType.MakeGenericType(genericTypeArguments);
                s_ProxyMap.Add(objectType, proxyType);
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
