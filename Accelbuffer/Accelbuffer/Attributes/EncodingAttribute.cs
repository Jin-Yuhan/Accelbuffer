using System;

namespace Accelbuffer
{
    /// <summary>
    /// 指示字段使用指定的编码被序列化，该特性只对 <see cref="char"/>, <see cref="string"/> 类型和这两个类型构成的集合的字段有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class EncodingAttribute : Attribute
    {
        /// <summary>
        /// 获取字符的编码
        /// </summary>
        public CharEncoding Encoding { get; }

        /// <summary>
        /// 初始化 EncodingAttribute 实例
        /// </summary>
        /// <param name="encoding">字符序列化使用的编码</param>
        public EncodingAttribute(CharEncoding encoding)
        {
            Encoding = encoding;
        }
    }
}
