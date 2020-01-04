namespace Accelbuffer
{
    /// <summary>
    /// 表示序列化上下文
    /// </summary>
    public readonly struct SerializationContext
    {
        /// <summary>
        /// 获取默认的序列化上下文
        /// </summary>
        public static readonly SerializationContext Default = new SerializationContext(CharEncoding.UTF8, Number.Var);

        /// <summary>
        /// 获取默认的字符编码
        /// </summary>
        public readonly CharEncoding DefaultEncoding;

        /// <summary>
        /// 获取默认的数字类型
        /// </summary>
        public readonly Number DefaultNumberType;

        /// <summary>
        /// 初始化 SerializationContext
        /// </summary>
        /// <param name="defaultEncoding">默认的字符编码</param>
        /// <param name="defaultNumberType">默认的数字类型</param>
        public SerializationContext(CharEncoding defaultEncoding, Number defaultNumberType)
        {
            DefaultEncoding = defaultEncoding;
            DefaultNumberType = defaultNumberType;
        }
    }
}
