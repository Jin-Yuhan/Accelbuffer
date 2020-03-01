using Accelbuffer.Injection.IL.Serializers;
using Accelbuffer.Memory;
using System.Reflection;

namespace Accelbuffer
{
    internal static class InternalTypeCache<T>
    {
        public static readonly ITypeSerializer<T> Serializer;
        public static readonly int ApproximateMemorySize;
        public static SerializationCallback<T> OnBeforeSerializationCallbacks;
        public static SerializationCallback<T> OnAfterDeserializationCallbacks;

        static InternalTypeCache()
        {
            Serializer = SerializerInjector.Inject<T>();
            
            ApproximateMemorySize = 160;//设置默认值

            if (Serializer is IMemorySizeForType<T> memSize && memSize.ApproximateMemorySize > 0)
            {
                ApproximateMemorySize = memSize.ApproximateMemorySize + 5;//tag max len
            }
            else
            {
                MemorySizeAttribute attr = typeof(T).GetCustomAttribute<MemorySizeAttribute>(true);

                if (attr != null && attr.ApproximateMemorySize > 0)
                {
                    ApproximateMemorySize = attr.ApproximateMemorySize + 5;//tag max len
                }
            }
        }

        public static void Initialize() { }
    }
}
