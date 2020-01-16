using System;

namespace Accelbuffer
{
    /// <summary>
    /// 无效的字段索引错误
    /// </summary>
    [Serializable]
    public sealed class InvalidFieldIndexException : Exception
    {
        /// <summary>
        /// 初始化 InvalidFieldIndexException
        /// </summary>
        public InvalidFieldIndexException() { }

        /// <summary>
        /// 初始化 InvalidFieldIndexException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public InvalidFieldIndexException(string message) : base(message) { }

        /// <summary>
        /// 初始化 InvalidFieldIndexException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public InvalidFieldIndexException(string message, Exception inner) : base(message, inner) { }
    }
}
