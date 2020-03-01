namespace Accelbuffer.Reflection
{
    /// <summary>
    /// 表示一个成员
    /// </summary>
    public interface IAccelMemberInfo
    {
        /// <summary>
        /// 获取成员的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取成员的文档
        /// </summary>
        string Document { get; }

        /// <summary>
        /// 获取声明此成员的类型
        /// </summary>
        AccelTypeInfo DeclaringType { get; }
    }
}
