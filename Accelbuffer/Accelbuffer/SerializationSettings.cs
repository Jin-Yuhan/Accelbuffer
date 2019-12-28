using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accelbuffer
{
    /// <summary>
    /// 公开对序列化设置操作的接口
    /// </summary>
    public static class SerializationSettings
    {
        /// <summary>
        /// 获取/设置 默认初始缓冲区大小
        /// </summary>
        public static long DefaultInitialBufferSize { get; set; }

        /// <summary>
        /// 获取默认数字选项
        /// </summary>
        public static NumberOption DefaultNumberOption { get; }

        /// <summary>
        /// 获取默认字符编码
        /// </summary>
        public static CharEncoding DefaultCharEncoding { get; }

        private static readonly Dictionary<Type, ProxySettings> s_ProxyBindings;

        static SerializationSettings()
        {
            DefaultInitialBufferSize = 20L;
            DefaultNumberOption = NumberOption.FixedLength;
            DefaultCharEncoding = CharEncoding.Unicode;
            s_ProxyBindings = new Dictionary<Type, ProxySettings>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long GetBufferSize(Type objectType, long size)
        {
            if (size <= 0)
            {
                try
                {
                    size = Marshal.SizeOf(objectType);
                }
                catch
                {
                    size = DefaultInitialBufferSize;
                }
            }

            return size;
        }

        internal static bool TryGetProxyBinding(Type objectType, out ProxySettings proxy)
        {
            return s_ProxyBindings.TryGetValue(objectType, out proxy);
        }

        /// <summary>
        /// 添加一个序列化代理的绑定
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        /// <typeparam name="TProxy">被绑定的代理类型</typeparam>
        /// <param name="strictMode">是否使用严格的序列化模式（开启对序列化索引不匹配的错误警告）</param>
        /// <param name="initialBufferSize">初始缓冲区大小，如果该值小于等于0L，则自动计算大小或者使用<see cref="DefaultInitialBufferSize"/></param>
        /// <exception cref="InvalidOperationException">已经存在一个绑定</exception>
        public static void AddSerializeProxyBinding<TObject, TProxy>(bool strictMode = false, long initialBufferSize = 0L) where TProxy : ISerializeProxy<TObject>
        {
            Type objectType = typeof(TObject);

            if (s_ProxyBindings.ContainsKey(objectType))
            {
                throw new InvalidOperationException($"关于{objectType.Name}的绑定已经存在");
            }

            s_ProxyBindings.Add(objectType, new ProxySettings(typeof(TProxy), GetBufferSize(objectType, initialBufferSize), strictMode));
        }

        /// <summary>
        /// 添加一个序列化代理的绑定
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        /// <param name="proxyType">被绑定的代理类型</param>
        /// <param name="strictMode">是否使用严格的序列化模式（开启对序列化索引不匹配的错误警告）</param>
        /// <param name="initialBufferSize">初始缓冲区大小，如果该值小于等于0L，则自动计算大小或者使用<see cref="DefaultInitialBufferSize"/></param>
        /// <exception cref="InvalidCastException"><paramref name="proxyType"/>类型错误</exception>
        /// <exception cref="InvalidOperationException">已经存在一个绑定</exception>
        public static void AddSerializeProxyBinding(Type objectType, Type proxyType, bool strictMode = false, long initialBufferSize = 0L)
        {
            Type expectedType = typeof(ISerializeProxy<>).MakeGenericType(objectType);

            if (!expectedType.IsAssignableFrom(proxyType))
            {
                throw new InvalidCastException($"无法将代理装换为{expectedType.Name}类型");
            }

            if (s_ProxyBindings.ContainsKey(objectType))
            {
                throw new InvalidOperationException($"关于{objectType.Name}的绑定已经存在");
            }

            s_ProxyBindings.Add(objectType, new ProxySettings(proxyType, GetBufferSize(objectType, initialBufferSize), strictMode));
        }

        /// <summary>
        /// 移除一个序列化代理的绑定
        /// </summary>
        /// <typeparam name="TObject">序列化对象类型</typeparam>
        public static void RemoveSerializeProxyBinding<TObject>()
        {
            s_ProxyBindings.Remove(typeof(TObject));
        }

        /// <summary>
        /// 移除一个序列化代理的绑定
        /// </summary>
        /// <param name="objectType">序列化对象类型</param>
        public static void RemoveSerializeProxyBinding(Type objectType)
        {
            s_ProxyBindings.Remove(objectType);
        }
    }
}
