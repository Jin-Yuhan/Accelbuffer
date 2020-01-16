using System;

namespace Accelbuffer
{
    /// <summary>
    /// 数据流的长度过短错误
    /// </summary>

    [Serializable]
    public sealed class StreamTooShortException : Exception
    {
        /// <summary>
        /// 初始化 StreamTooShortException
        /// </summary>
        public StreamTooShortException() { }

        /// <summary>
        /// 初始化 StreamTooShortException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public StreamTooShortException(string message) : base(message) { }

        /// <summary>
        /// 初始化 StreamTooShortException
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public StreamTooShortException(string message, Exception inner) : base(message, inner) { }
    }
}
