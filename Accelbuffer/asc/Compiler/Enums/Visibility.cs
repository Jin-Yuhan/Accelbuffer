using System;

namespace asc.Compiler
{
    [Flags]
    public enum Visibility
    {
        None = 0,
        Public = 1 << 0,
        Internal = 1 << 1,
        Private = 1 << 2,
        Protected = 1 << 3,
        ProtectedInternal = Protected | Internal,
        PrivateProtected = Private | Protected
    }
}
