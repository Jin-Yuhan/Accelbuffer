using System;

namespace Accelbuffer.Runtime.Injection
{
    /// <summary>
    /// 指定一个方法为序列化回调方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SerializationCallbackAttribute : Attribute
    {
        /// <summary>
        /// 获取序列化回调方法类型
        /// </summary>
        public SerializationCallbackMethod CallbackMethodType { get; }

        /// <summary>
        /// 获取/设置调用优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 初始化 SerializationCallbackAttribute
        /// </summary>
        /// <param name="method">序列化回调方法类型</param>
        public SerializationCallbackAttribute(SerializationCallbackMethod method)
        {
            CallbackMethodType = method;
        }
    }
}
