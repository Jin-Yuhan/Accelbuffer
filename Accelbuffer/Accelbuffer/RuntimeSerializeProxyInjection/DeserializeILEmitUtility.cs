using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static Accelbuffer.SerializationUtility;

namespace Accelbuffer
{
    internal static partial class ILEmitUtility
    {
        public static void EmitTypeDeserialize(this ILGenerator il, Type objType)
        {
            List<FieldData> fields = objType.GetSerializedFields();

            for (int i = 0; i < fields.Count; i++)
            {
                FieldData data = fields[i];
                il.EmitFieldDeserialize(objType, data.Field, data.Index);
            }

            il.TryEmitMsgMethodD(objType);
        }

        private static void TryEmitMsgMethodD(this ILGenerator il, Type objType)
        {
            MethodInfo msgMethod = objType.GetMethod("OnAfterDeserialize", Type.EmptyTypes);

            if (msgMethod != null)
            {
                if (objType.IsValueType)
                {
                    il.Emit(OpCodes.Ldloca_S, (byte)0);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc_0);
                }

                il.Emit(OpCodes.Callvirt, msgMethod);
            }
        }

        private static void EmitSerializerDotDeserializeCall(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Call, typeof(Serializer<>).MakeGenericType(type).GetMethod(s_DeserializeName, s_ReaderPtrTypes));
        }


        private static void EmitLoadReader(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldind_I);
        }

        private static void EmitLoadReaderAndIndex(this ILGenerator il, byte index)
        {
            il.EmitLoadReader();
            il.Emit(OpCodes.Ldc_I4, (int)index);
        }


        private static void EmitFieldDeserialize(this ILGenerator il, Type objType, FieldInfo field, byte index)
        {
            if (objType.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)0);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_0);
            }

            SerializedType type = GetSerializedType(field.FieldType, out string name);

            if (type != SerializedType.Complex)
            {
                il.EmitLoadReaderAndIndex(index);
            }

            switch (type)
            {
                case SerializedType.Number:
                    il.EmitNumberDeserialize(field.GetNumberOption(), name);
                    break;

                case SerializedType.Char:
                    il.EmitCharDeserialize(field.GetCharEncoding(), name);
                    break;

                case SerializedType.Boolean:
                    il.EmitBooleanDeserialize(name);
                    break;

                default:
                    il.EmitLoadReader();
                    il.EmitSerializerDotDeserializeCall(field.FieldType);
                    break;
            }

            il.Emit(OpCodes.Stfld, field);
        }

        public static void EmitNumberDeserialize(this ILGenerator il, Number option, string name)
        {
            string methodName = $"{s_ReadString}{(option == Number.Fixed ? s_FixedName : s_VariableName)}{name}";
            MethodInfo method = s_ReaderType.GetMethod(methodName, s_IndexTypes);
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitCharDeserialize(this ILGenerator il, CharEncoding encoding, string name)
        {
            string methodName = $"{s_ReadString}{name}";
            il.EmitEncoding(encoding);
            MethodInfo method = s_ReaderType.GetMethod(methodName, s_IndexAndCharEncodingTypes);
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitBooleanDeserialize(this ILGenerator il, string name)
        {
            string methodName = $"{s_ReadString}{name}";
            MethodInfo method = s_ReaderType.GetMethod(methodName, s_IndexTypes);
            il.Emit(OpCodes.Call, method);
        }
    }
}
