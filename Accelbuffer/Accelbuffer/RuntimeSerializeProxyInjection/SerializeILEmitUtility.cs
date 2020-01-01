using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static Accelbuffer.SerializationUtility;

namespace Accelbuffer
{
    internal static partial class ILEmitUtility
    {
        public static void EmitTypeSerialize(this ILGenerator il, Type objType, byte index)
        {
            List<FieldData> fields = objType.GetSerializedFields();
            il.TryEmitMsgMethodS(objType);

            for (int i = 0; i < fields.Count; i++)
            {
                FieldData data = fields[i];
                il.EmitFieldSerialize(objType, data.Field, data.Index);
            }
        }

        private static void TryEmitMsgMethodS(this ILGenerator il, Type objType)
        {
            MethodInfo msgMethod = objType.GetMethod("OnBeforeSerialize", Type.EmptyTypes);

            if (msgMethod != null)
            {
                il.Emit(OpCodes.Ldarg_1);

                if (!objType.IsValueType)
                {
                    il.Emit(OpCodes.Ldind_Ref);
                }

                il.Emit(OpCodes.Callvirt, msgMethod);
            }
        }

        private static void EmitSerializerDotSerializeCall(this ILGenerator il, Type type)
        {
            Type[] args = new Type[] { type, s_WriterPtrType };
            il.Emit(OpCodes.Call, typeof(Serializer<>).MakeGenericType(type).GetMethod(s_SerializeName, args));
        }


        private static void EmitLoadArg(this ILGenerator il, Type objType)
        {
            il.Emit(OpCodes.Ldarg_1);//arg

            if (!objType.IsValueType)
            {
                il.Emit(OpCodes.Ldind_Ref);
            }
        }

        private static void EmitLoadSerializeField(this ILGenerator il, Type objType, FieldInfo field)
        {
            il.EmitLoadArg(objType);
            il.Emit(OpCodes.Ldfld, field);//field
        }

        private static void EmitLoadWriter(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_2);//writer
            il.Emit(OpCodes.Ldind_I);
        }

        private static void EmitLoadWriterAndIndex(this ILGenerator il, byte index)
        {
            il.EmitLoadWriter();//writer
            il.Emit(OpCodes.Ldc_I4, (int)index);//index
        }

        private static void EmitLoadWriterAndIndexAndArg(this ILGenerator il, Type objType, byte index)
        {
            il.EmitLoadWriterAndIndex(index);
            il.EmitLoadArg(objType);
        }

        private static void EmitLoadWriterAndIndexAndSerializeField(this ILGenerator il, Type objType, FieldInfo field, byte index)
        {
            il.EmitLoadWriterAndIndexAndArg(objType, index);
            il.Emit(OpCodes.Ldfld, field);//field
        }

        private static void EmitLoadSerializeFieldAndWriter(this ILGenerator il, Type objType, FieldInfo field)
        {
            il.EmitLoadSerializeField(objType, field);
            il.EmitLoadWriter();
        }


        private static void EmitFieldSerialize(this ILGenerator il, Type objType, FieldInfo field, byte index)
        {
            SerializedType type = GetSerializedType(field.FieldType, out _);

            if (type != SerializedType.Complex)
            {
                il.EmitLoadWriterAndIndexAndSerializeField(objType, field, index);
            }

            switch (type)
            {
                case SerializedType.Number:
                    il.EmitNumberSerialize(field.GetNumberOption(), field.FieldType);
                    break;

                case SerializedType.Char:
                    il.EmitCharSerialize(field.GetCharEncoding(), field.FieldType);
                    break;

                case SerializedType.Boolean:
                    il.EmitBooleanSerialize();
                    break;

                default:
                    il.EmitLoadSerializeFieldAndWriter(objType, field);
                    il.EmitSerializerDotSerializeCall(field.FieldType);
                    break;
            }
        }

        public static void EmitNumberSerialize(this ILGenerator il, Number option, Type numberType)
        {
            il.EmitIsFixedNumber(option);
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString, new Type[] { typeof(byte), numberType, typeof(Number) });
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitCharSerialize(this ILGenerator il, CharEncoding encoding, Type charType)
        {
            il.EmitEncoding(encoding);
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString,
                charType == typeof(string) ? s_IndexAndStringAndCharEncodingTypes : s_IndexAndCharAndCharEncodingTypes);
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitBooleanSerialize(this ILGenerator il)
        {
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString, s_IndexAndBoolTypes);
            il.Emit(OpCodes.Call, method);
        }
    }
}
