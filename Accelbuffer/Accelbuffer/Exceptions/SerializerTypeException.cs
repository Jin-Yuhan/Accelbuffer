using System;

namespace Accelbuffer
{
    /// <summary>
    /// 序列化代理类型错误
    /// </summary>
    [Serializable]
    public sealed class SerializerTypeException : Exception
    {
        /// <summary>
        /// 初始化 SerializerTypeException
        /// </summary>
        public SerializerTypeException() { }

        /// <summary>
        /// 初始化 SerializerTypeException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public SerializerTypeException(string message) : base(message) { }

        /// <summary>
        /// 初始化 SerializerTypeException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public SerializerTypeException(string message, Exception inner) : base(message, inner) { }
    }
}
