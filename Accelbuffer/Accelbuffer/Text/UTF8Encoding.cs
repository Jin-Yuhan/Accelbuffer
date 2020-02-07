using Accelbuffer.Properties;
using System;

namespace Accelbuffer.Text
{
    internal sealed class UTF8Encoding : IUnsafeEncoding
    {
        public unsafe int GetBytes(string str, byte* bytes)
        {
            int count = str.Length;
            int byteCount = 0;

            fixed (char* chars = str)
            {
                char* p = chars;

                while (count-- > 0)
                {
                    char c = *p++;

                    if (c < 0x80)
                    {
                        *bytes++ = (byte)(c & 0xff);
                        byteCount += 1;
                    }
                    else if (c < 0x800)
                    {
                        *bytes++ = (byte)(((c >> 6) & 0x1f) | 0xc0);
                        *bytes++ = (byte)((c & 0x3f) | 0x80);
                        byteCount += 2;
                    }
                    else if (c < 0x10000)
                    {
                        *bytes++ = (byte)(((c >> 12) & 0x0f) | 0xe0);
                        *bytes++ = (byte)(((c >> 6) & 0x3f) | 0x80);
                        *bytes++ = (byte)((c & 0x3f) | 0x80);
                        byteCount += 3;
                    }
                }
            }

            return byteCount;
        }

        public unsafe string GetString(byte* bytes, int byteCount)
        {
            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), Resources.UTF8StringByteCountError);
            }

            char* chars = stackalloc char[byteCount];
            int strLen = 0;

            while (byteCount > 0)
            {
                byte b = *bytes++;

                if ((b >> 4) == 0xE)
                {
                    int b1 = b & 0xF;
                    int b2 = *bytes++ & 0x3F;
                    int b3 = *bytes++ & 0x3F;

                    chars[strLen] = (char)((b1 << 12) | (b2 << 6) | b3);
                    byteCount -= 3;
                }
                else if ((b >> 5) == 0x6)
                {
                    int b1 = b & 0x1F;
                    int b2 = *bytes++ & 0x3F;

                    chars[strLen] = (char)((b1 << 6) | b2);
                    byteCount -= 2;
                }
                else if ((b >> 7) == 0)
                {
                    chars[strLen] = (char)(b & 0x7F);
                    byteCount -= 1;
                }
                else
                {
                    throw new InvalidUTF8CharException(Resources.InvalidUTF8Char);
                }

                strLen++;
            }

            return new string(chars, 0, strLen);
        }
    }
}
