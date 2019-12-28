using System;
using System.Runtime.Serialization;

namespace Accelbuffer
{
    /// <summary>
    /// 指定序列化索引值未找到错误
    /// </summary>
    [Serializable]
    public sealed class MissingSerializedValueException : Exception
    {
        /// <summary>
        /// 初始化 MissingSerializedValueException 实例
        /// </summary>
        public MissingSerializedValueException() { }

        /// <summary>
        /// 初始化 MissingSerializedValueException 实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public MissingSerializedValueException(string message) : base(message) { }

        /// <summary>
        /// 初始化 MissingSerializedValueException 实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public MissingSerializedValueException(string message, Exception inner) : base(message, inner) { }

        internal MissingSerializedValueException(byte index) : base($"序列化错误，未找到序列化索引为{index.ToString()}的值") { }
    }
}
