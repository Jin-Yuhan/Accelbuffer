using System;

namespace asc
{
    [Flags]
    public enum RunMode
    {
        None = 0,
        ToBytes = 1 << 0,
        ToVisualBasic = 1 << 1,
        ToCSharp = 1 << 2,
        //ToJava = 1 << 3,
        //ToCPP = 1 << 4,
    }
}
