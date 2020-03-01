using Accelbuffer.Properties;
using System;
using System.Reflection;

namespace Accelbuffer.Injection
{
    internal sealed class ObjectSerializer : ITypeSerializer<object>
    {
        private static readonly RuntimeMethodHandle s_SerializeMethod = typeof(ObjectSerializer).GetMethod(Resources.SerializeMethodName).MethodHandle;
        private static readonly RuntimeMethodHandle s_DeserializeMethod = typeof(ObjectSerializer).GetMethod(Resources.DeserializeMethodName).MethodHandle;
        private static readonly string s_SerializerFieldName = Resources.SerializerTypeSuffix;

        object ITypeSerializer<object>.Deserialize(ref AccelReader reader)
        {
            if (!reader.HasNext())
            {
                return null;
            }
            
            Type type = Type.GetType(reader.ReadString());
            object serializer = typeof(InternalTypeCache<>).MakeGenericType(type).GetField(s_SerializerFieldName).GetValue(null);

            if (serializer is IBuiltinTypeSerializer && !reader.HasNext())
            {
                return null;
            }

            MethodInfo method = MethodBase.GetMethodFromHandle(s_DeserializeMethod) as MethodInfo;
            return method.MakeGenericMethod(type).Invoke(null, new object[] { serializer, reader });
        }

        void ITypeSerializer<object>.Serialize(object obj, ref AccelWriter writer)
        {
            if (obj == null)
            {
                return;
            }

            Type type = obj.GetType();
            writer.WriteValue(1, type.AssemblyQualifiedName);
            
            MethodInfo method = MethodBase.GetMethodFromHandle(s_SerializeMethod) as MethodInfo;
            object serializer = typeof(InternalTypeCache<>).MakeGenericType(type).GetField(s_SerializerFieldName).GetValue(null);
            writer = (AccelWriter)method.MakeGenericMethod(type).Invoke(null, new object[] { serializer, obj, writer });
        }

        public static AccelWriter Serialize<T>(ITypeSerializer<T> value, T obj, ref AccelWriter writer)
        {
            value.Serialize(obj, ref writer);
            return writer;
        }

        public static object Deserialize<T>(ITypeSerializer<T> value, ref AccelReader reader)
        {
            return value.Deserialize(ref reader);
        }
    }
}
