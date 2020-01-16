using System;

namespace Accelbuffer
{
    /// <summary>
    /// 无效的UTF8字符错误
    /// </summary>
    [Serializable]
    public sealed class InvalidUTF8CharException : Exception
    {
        /// <summary>
        /// 初始化 InvalidUTF8CharException
        /// </summary>
        public InvalidUTF8CharException() { }

        /// <summary>
        /// 初始化 InvalidUTF8CharException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public InvalidUTF8CharException(string message) : base(message) { }

        /// <summary>
        /// 初始化 InvalidUTF8CharException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public InvalidUTF8CharException(string message, Exception inner) : base(message, inner) { }
    }
}
