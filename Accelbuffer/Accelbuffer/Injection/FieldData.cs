using System.Reflection;

namespace Accelbuffer.Injection
{
    internal readonly struct FieldData
    {
        public readonly FieldInfo Field;
        public readonly int Index;
        public readonly bool CheckRef;

        public FieldData(FieldInfo field, int index, bool checkRef)
        {
            Field = field;
            Index = index;
            CheckRef = checkRef;
        }
    }
}
