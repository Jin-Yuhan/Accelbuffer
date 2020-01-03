using System.Reflection;

namespace Accelbuffer
{
    internal readonly struct MethodData
    {
        public readonly MethodInfo Method;
        public readonly SerializationCallbackMethod Type;
        public readonly int Priority;

        public MethodData(MethodInfo method, SerializationCallbackMethod type, int priority)
        {
            Method = method;
            Type = type;
            Priority = priority;
        }
    }
}
