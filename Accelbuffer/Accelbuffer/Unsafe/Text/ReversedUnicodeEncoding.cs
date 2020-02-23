using Accelbuffer.Properties;
using System;

namespace Accelbuffer.Unsafe.Text
{
    internal sealed class ReversedUnicodeEncoding : IUnsafeEncoding
    {
        public unsafe int GetBytes(string str, byte* bytes)
        {
            int size = str.Length;

            fixed (char* chars = str)
            {
                char* src = chars;

                while (size-- > 0)
                {
                    byte* charBytes = (byte*)src++;
                    *bytes++ = charBytes[1];
                    *bytes++ = charBytes[0];
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

            for (int i = 0; i < byteCount; i += 2)
            {
                bytes[i] ^= bytes[i + 1];
                bytes[i + 1] ^= bytes[i];
                bytes[i] ^= bytes[i + 1];
            }

            return new string((char*)bytes, 0, byteCount >> 1);
        }
    }
}
