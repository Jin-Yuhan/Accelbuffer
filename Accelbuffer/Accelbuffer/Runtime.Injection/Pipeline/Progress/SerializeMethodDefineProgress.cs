using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static Accelbuffer.SerializationUtility;

namespace Accelbuffer.Runtime.Injection
{
    internal sealed class SerializeMethodDefineProgress : ProxyGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods)
        {
            s_SerializeTypes[0] = objType;

            MethodBuilder method = builder.DefineMethod(s_SerializeName,
                                                     s_MethodAttributes,
                                                     s_CallingConventions,
                                                     typeof(void),
                                                     s_SerializeTypes);

            ILGenerator il = method.GetILGenerator();
            EmitIL(il, objType, fields, methods);
            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(s_SerializeName));
        }

        private void EmitIL(ILGenerator il, Type objType, List<FieldData> fields, List<MethodData> methods)
        {
            for (int i = 0; i < methods.Count; i++)
            {
                MethodData data = methods[i];

                if (data.Type == SerializationCallbackMethod.OnBeforeSerialize)
                {
                    EmitMessageCallback(il, objType, data.Method);
                }
            }

            for (int i = 0; i < fields.Count; i++)
            {
                FieldData data = fields[i];
                EmitFieldSerialize(il, objType, data.Field, data.Index);
            }
        }

        private static void EmitMessageCallback(ILGenerator il, Type objType, MethodInfo method)
        {
            il.Emit(OpCodes.Ldarga_S, (byte)1);

            if (!objType.IsValueType)
            {
                il.Emit(OpCodes.Ldind_Ref);
            }

            il.Emit(OpCodes.Callvirt, method);
        }

        private static void EmitSerializerDotSerializeCall(ILGenerator il, Type type)
        {
            s_SerializeCallTypes[0] = type;
            il.Emit(OpCodes.Call, typeof(Serializer<>).MakeGenericType(type).GetMethod(s_SerializeName, s_SerializeCallTypes));
        }

        private static void EmitLoadWriterAndIndexAndSerializeField(ILGenerator il, FieldInfo field, byte index)
        {
            il.Emit(OpCodes.Ldarg_2);//writer
            il.Emit(OpCodes.Ldc_I4, (int)index);//index
            il.Emit(OpCodes.Ldarg_1);//arg
            il.Emit(OpCodes.Ldfld, field);//field
        }

        private static void EmitLoadSerializeFieldAndWriter(ILGenerator il, FieldInfo field)
        {
            il.Emit(OpCodes.Ldarg_1);//arg
            il.Emit(OpCodes.Ldfld, field);//field
            il.Emit(OpCodes.Ldarg_2);//writer
        }

        private static void EmitFieldSerialize(ILGenerator il, Type objType, FieldInfo field, byte index)
        {
            Type fieldType = field.FieldType;
            SerializedType type = GetSerializedType(field.FieldType, out _);

            if (type != SerializedType.Complex)
            {
                EmitLoadWriterAndIndexAndSerializeField(il, field, index);
            }

            switch (type)
            {
                case SerializedType.Number:
                    EmitNumberSerialize(il, GetNumberType(field), fieldType);
                    break;

                case SerializedType.Char:
                    EmitCharSerialize(il, GetCharEncoding(field), fieldType);
                    break;

                case SerializedType.Boolean:
                    EmitBooleanSerialize(il);
                    break;

                default:
                    EmitLoadSerializeFieldAndWriter(il, field);
                    EmitSerializerDotSerializeCall(il, fieldType);
                    break;
            }
        }

        public static void EmitNumberSerialize(ILGenerator il, Number option, Type numberType)
        {
            s_IndexAndNumberTypes[1] = numberType;

            EmitIsFixedNumber(il, option);
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString, s_IndexAndNumberTypes);
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitCharSerialize(ILGenerator il, CharEncoding encoding, Type charType)
        {
            s_IndexAndCharEncodingTypes3[1] = charType;

            EmitEncoding(il, encoding);
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString, s_IndexAndCharEncodingTypes3);
            il.Emit(OpCodes.Call, method);
        }

        public static void EmitBooleanSerialize(ILGenerator il)
        {
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString, s_IndexAndBoolTypes);
            il.Emit(OpCodes.Call, method);
        }
    }
}
