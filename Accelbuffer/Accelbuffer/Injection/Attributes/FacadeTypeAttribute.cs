using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指示一个字段使用外观类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FacadeTypeAttribute : Attribute
    {
        /// <summary>
        /// 字段的真实类型。
        /// 在序列化时，字段会被强制转换为该类型。
        /// 在反序列化时，字段会从该类型强制转换为原类型。
        /// </summary>
        public Type RealType { get; }

        /// <summary>
        /// 初始化 FacadeTypeAttribute
        /// </summary>
        /// <param name="realType">
        /// 字段的真实类型。
        /// 在序列化时，字段会被强制转换为该类型。
        /// 在反序列化时，字段会从该类型强制转换为原类型。
        /// </param>
        public FacadeTypeAttribute(Type realType)
        {
            RealType = realType;
        }
    }
}
