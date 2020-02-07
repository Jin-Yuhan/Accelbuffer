using System.Reflection;

namespace Accelbuffer.Injection
{
    internal readonly struct FieldData
    {
        public readonly FieldInfo Field;
        public readonly int Index;
        public readonly bool NeverNull;

        public FieldData(FieldInfo field, int index, bool neverNull)
        {
            Field = field;
            Index = index;
            NeverNull = neverNull;
        }
    }
}
