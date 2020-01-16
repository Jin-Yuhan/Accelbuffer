using System;

namespace Accelbuffer.Injection
{
    /// <summary>
    /// 指定一个方法为序列化前的回调方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class OnBeforeSerializationAttribute : Attribute
    {
        /// <summary>
        /// 获取/设置调用优先级
        /// </summary>
        public int Priority { get; set; }
    }
}
