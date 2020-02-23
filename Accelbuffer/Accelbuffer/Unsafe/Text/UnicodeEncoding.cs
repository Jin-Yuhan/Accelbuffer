using Accelbuffer.Properties;
using System;

namespace Accelbuffer.Unsafe.Text
{
    internal sealed class UnicodeEncoding : IUnsafeEncoding
    {
        public unsafe int GetBytes(string str, byte* bytes)
        {
            int size = str.Length;

            fixed (char* chars = str)
            {
                char* src = chars;
                char* dst = (char*)bytes;

                while (size-- > 0)
                {
                    *dst++ = *src++;
                }
            }

            return size << 1;
        }

        public unsafe string GetString(byte* bytes, int byteCount)
        {
            if ((byteCount < 0) || (byteCount % 2 != 0))
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), Resources.UnicodeStringByteCountError);
            }

            return new string((char*)bytes, 0, byteCount >> 1);
        }
    }
}
