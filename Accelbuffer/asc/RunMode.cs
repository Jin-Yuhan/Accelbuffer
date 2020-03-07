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
        [Obsolete("暂时不提供支持", true)]
        ToJava = 1 << 3,
        [Obsolete("暂时不提供支持", true)]
        ToCPP = 1 << 4,
    }
}
