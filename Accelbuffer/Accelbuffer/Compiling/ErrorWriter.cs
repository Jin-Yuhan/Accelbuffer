namespace Accelbuffer.Compiling
{
    /// <summary>
    /// 表示一个输出错误信息的编写器。 此类为抽象类。
    /// </summary>
    public abstract class ErrorWriter
    {
        private static readonly object[] s_Empty = new object[0];

        /// <summary>
        /// 获取是否输出过错误
        /// </summary>
        public bool IsError { get; private set; }

        /// <summary>
        /// 初始化 ErrorWriter
        /// </summary>
        protected ErrorWriter()
        {
            IsError = false;
        }

        /// <summary>
        /// 重置该对象
        /// </summary>
        public void Reset()
        {
            IsError = false;
        }

        /// <summary>
        /// 输出一个错误
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="token">导致错误的标记</param>
        public void LogError(string msg, Token token)
        {
            LogError(msg, token.FilePath, token.Line, token.Column, token.Raw);
        }

        /// <summary>
        /// 输出一个错误
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="filePath">文件所在路径</param>
        /// <param name="line">错误所在行号</param>
        /// <param name="column">错误所在列号</param>
        /// <param name="args">字符串格式化参数</param>
        public void LogError(string msg, string filePath, int line, int column, params object[] args)
        {
            if (!IsError)
            {
                IsError = true;
                LogErrorMessage(msg, filePath, line, column, args);
            }
        }

        /// <summary>
        /// 输出一个警告
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="token">导致警告的标记</param>
        public void LogWarning(string msg, Token token)
        {
            LogWarning(msg, token.FilePath, token.Line, token.Column, s_Empty);
        }

        /// <summary>
        /// 输出一个警告
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="token">导致警告的标记</param>
        /// <param name="args">字符串格式化参数</param>
        public void LogWarning(string msg, Token token, params object[] args)
        {
            LogWarning(msg, token.FilePath, token.Line, token.Column, args);
        }

        /// <summary>
        /// 输出一个错误
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="filePath">文件所在路径</param>
        /// <param name="line">错误所在行号</param>
        /// <param name="column">错误所在列号</param>
        /// <param name="args">字符串格式化参数</param>
        protected abstract void LogErrorMessage(string msg, string filePath, int line, int column, params object[] args);

        /// <summary>
        /// 输出一个警告
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <param name="filePath">文件所在路径</param>
        /// <param name="line">警告所在行号</param>
        /// <param name="column">警告所在列号</param>
        /// <param name="args">字符串格式化参数</param>
        protected abstract void LogWarning(string msg, string filePath, int line, int column, params object[] args);
    }
}
