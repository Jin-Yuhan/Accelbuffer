using System;

namespace Accelbuffer
{
    /// <summary>
    /// 标签不匹配错误
    /// </summary>
    [Serializable]
    public sealed class TagDismatchException : Exception
    {
        /// <summary>
        /// 初始化 TagDismatchException 实例
        /// </summary>
        public TagDismatchException() { }

        /// <summary>
        /// 初始化 TagDismatchException 实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public TagDismatchException(string message) : base(message) { }

        /// <summary>
        /// 初始化 TagDismatchException 实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        /// <param name="inner">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public TagDismatchException(string message, Exception inner) : base(message, inner) { }

        internal TagDismatchException(ValueTypeCode expected, ValueTypeCode serialized)
            : base($"数据类型匹配错误，无法将{serialized.ToString()}转换为{expected.ToString()}") { }

        internal TagDismatchException(CharEncoding expected, CharEncoding serialized)
            : base($"字符编码匹配错误，无法将{serialized.ToString()}转换为{expected.ToString()}") { }

        internal TagDismatchException(CharType expected, CharType serialized)
            : base($"字符类型匹配错误，无法将{serialized.ToString()}转换为{expected.ToString()}") { }

        internal TagDismatchException(int expectedByteCount, int serializedByteCount)
            : base($"数字长度匹配错误，无法将{(serializedByteCount << 3).ToString()}位数字隐式转换为{(expectedByteCount << 3).ToString()}位数字") { }

        internal TagDismatchException(NumberSign expected, NumberSign serialized)
            : base($"数字符号匹配错误，无法将{serialized.ToString()}的数字转换为{expected.ToString()}的数字") { }
    }
}
