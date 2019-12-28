using System.Reflection;

namespace Accelbuffer
{
    internal readonly struct FieldData
    {
        public readonly FieldInfo Field;
        public readonly byte Index;

        public FieldData(FieldInfo field, byte index)
        {
            Field = field;
            Index = index;
        }
    }
}
