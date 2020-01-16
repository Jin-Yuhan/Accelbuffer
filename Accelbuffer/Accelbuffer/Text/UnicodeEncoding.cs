using System;
using Accelbuffer.Properties;

namespace Accelbuffer.Text
{
    internal sealed class UnicodeEncoding : ITextEncoding
    {
        public unsafe int GetBytes(string str, byte* bytes)
        {
            int count = str.Length;
            char* p = (char*)bytes;

            fixed (char* chars = str)
            {
                char* source = chars;

                while (count-- > 0)
                {
                    *p++ = *source++;
                }
            }

            return str.Length << 1;
        }

        public unsafe int GetBytes(char c, byte* bytes)
        {
            *(char*)bytes = c;
            return 2;
        }

        public unsafe char GetChar(byte* bytes, out int byteCount)
        {
            byteCount = 2;
            return *(char*)bytes;
        }

        public unsafe string GetString(byte* bytes, int byteCount)
        {
            if ((byteCount < 0) || (byteCount % 2 != 0))
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), Resources.UnicodeStringByteCountError);
            }

            char* chars = (char*)bytes;
            return new string(chars, 0, byteCount >> 1);
        }
    }
}
