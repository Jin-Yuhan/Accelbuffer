using System;

namespace Accelbuffer.Reflection
{
    /// <summary>
    /// 表示一个类型的可访问性
    /// </summary>
    [Flags]
    public enum TypeVisibility
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// public
        /// </summary>
        Public = 1 << 0,
        /// <summary>
        /// internal
        /// </summary>
        Internal = 1 << 1,
        /// <summary>
        /// private
        /// </summary>
        Private = 1 << 2,
        /// <summary>
        /// protected
        /// </summary>
        Protected = 1 << 3,
        /// <summary>
        /// protected internal
        /// </summary>
        ProtectedInternal = Protected | Internal,
        /// <summary>
        /// private protected
        /// </summary>
        [Obsolete("使用该可访问性生成的代码并不稳定")]
        PrivateProtected = Private | Protected
    }
}
