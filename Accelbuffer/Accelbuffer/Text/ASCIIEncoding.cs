using Accelbuffer.Properties;
using System;

namespace Accelbuffer.Text
{
    internal sealed class ASCIIEncoding : ITextEncoding
    {
        private const char m_ReplacementChar = '?';
        private const char m_MaxChar = '\u0080';

        private const byte m_ReplacementCharByte = 63;
        private const byte m_MaxCharByte = 128;

        public unsafe int GetBytes(string str, byte* bytes)
        {
            fixed (char* chars = str)
            {
                char* start = chars;
                char* end = chars + str.Length;
                byte* bytesStart = bytes;

                while (start < end)
                {
                    char c3 = *(start++);

                    *bytes++ = c3 >= m_MaxChar ? m_ReplacementCharByte : (byte)c3;
                }

                return (int)(bytes - bytesStart);
            }
        }

        public unsafe int GetBytes(char c, byte* bytes)
        {
            *bytes++ = c >= m_MaxChar ? m_ReplacementCharByte : (byte)c;
            return 1;
        }

        public unsafe char GetChar(byte* bytes, out int byteCount)
        {
            byteCount = 1;
            byte b = *bytes++;
            return b >= m_MaxCharByte ? m_ReplacementChar : (char)b;
        }

        public unsafe string GetString(byte* bytes, int byteCount)
        {
            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), Resources.ASCIIStringByteCountError);
            }

            char* chars = stackalloc char[byteCount];

            char* p = chars;
            byte* bytesEnd = bytes + byteCount;

            while (bytes < bytesEnd)
            {
                byte b = *bytes++;
                *p++ = b >= m_MaxCharByte ? m_ReplacementChar : (char)b;
            }

            return new string(chars, 0, byteCount);
        }
    }
}
