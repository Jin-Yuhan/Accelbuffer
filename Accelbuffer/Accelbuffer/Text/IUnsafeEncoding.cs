using System;

namespace Accelbuffer.Text
{
    /// <summary>
    /// '不安全'的字符串编码转换器
    /// </summary>
    [CLSCompliant(false)]
    public interface IUnsafeEncoding
    {
        /// <summary>
        /// 将指定字符串中的一组字符编码为指定的字节数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="bytes">字节数组指针</param>
        /// <returns>字节数量</returns>
        unsafe int GetBytes(string str, byte* bytes);

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一个字符串
        /// </summary>
        /// <param name="bytes">字节数组指针</param>
        /// <param name="byteCount">字节数量</param>
        /// <returns>解码的字符串</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="byteCount"/>的值错误</exception>
        unsafe string GetString(byte* bytes, int byteCount);
    }
}
