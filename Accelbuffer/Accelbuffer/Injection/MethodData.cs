using System.Reflection;

namespace Accelbuffer.Injection
{
    internal readonly struct MethodData
    {
        public readonly MethodInfo Method;
        public readonly AccelbufferCallback Type;
        public readonly int Priority;

        public MethodData(MethodInfo method, AccelbufferCallback type, int priority)
        {
            Method = method;
            Type = type;
            Priority = priority;
        }
    }
}
