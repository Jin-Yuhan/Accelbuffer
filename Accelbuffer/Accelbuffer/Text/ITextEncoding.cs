using System;

namespace Accelbuffer.Text
{
    /// <summary>
    /// 表示一种文本编码
    /// </summary>
    public interface ITextEncoding
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

        /// <summary>
        /// 将指定字符编码为指定的字节数组
        /// </summary>
        /// <param name="c">字符</param>
        /// <param name="bytes">字节数组指针</param>
        /// <returns>字节数量</returns>
        unsafe int GetBytes(char c, byte* bytes);

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一个字符
        /// </summary>
        /// <param name="bytes">字节数组指针</param>
        /// <param name="readByteCount">读取的字节数量</param>
        /// <returns>解码的字符</returns>
        unsafe char GetChar(byte* bytes, out int readByteCount);
    }
}
