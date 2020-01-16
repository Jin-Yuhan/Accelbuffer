using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection
{
    internal sealed class SerializeMethodDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods, SerializerOption option)
        {
            s_SerializeTypes[0] = objType;

            MethodBuilder method = builder.DefineMethod(Resources.SerializeMethodName,
                                                     s_MethodAttributes,
                                                     s_CallingConventions,
                                                     typeof(void),
                                                     s_SerializeTypes);

            ILGenerator il = method.GetILGenerator();
            EmitIL(il, objType, fields, methods, option);
            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(Resources.SerializeMethodName));
        }

        private void EmitIL(ILGenerator il, Type objType, List<FieldData> fields, List<MethodData> methods, SerializerOption option)
        {
            for (int i = 0; i < methods.Count; i++)
            {
                MethodData data = methods[i];

                if (data.Type == AccelbufferCallback.OnBeforeSerialization)
                {
                    EmitMessageCallback(il, objType, data.Method);
                }
            }

            for (int i = 0; i < fields.Count; i++)
            {
                FieldData data = fields[i];
                EmitFieldSerialize(il, data.Field, data.Index, data.CheckRef, option);
            }
        }

        private static void EmitMessageCallback(ILGenerator il, Type objType, MethodInfo method)
        {
            il.Emit(OpCodes.Ldarga_S, (byte)1);

            if (!objType.IsValueType)
            {
                il.Emit(OpCodes.Ldind_Ref);
            }

            if (method.IsAbstract || method.IsVirtual)
            {
                il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                il.Emit(OpCodes.Call, method);
            }
        }

        private static void EmitFieldSerialize(ILGenerator il, FieldInfo field, int index, bool checkRef, SerializerOption option)
        {
            Type fieldType = field.FieldType;
            WireType type = GetWireType(field.FieldType, out _);
            Label label = default;

            if (checkRef && (!fieldType.IsValueType) && (option == SerializerOption.Normal))
            {
                label = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_1);//arg
                il.Emit(OpCodes.Ldfld, field);//field
                il.Emit(OpCodes.Brfalse, label);
            }

            il.Emit(OpCodes.Ldarg_2);//writer

            if (option == SerializerOption.Normal)
            {
                il.Emit(OpCodes.Ldc_I4, index);//index
            }

            il.Emit(OpCodes.Ldarg_1);//arg
            il.Emit(OpCodes.Ldfld, field);//field

            switch (type)
            {
                case WireType.Int:
                    EmitIntSerialize(il, field, fieldType, option);
                    break;

                case WireType.Float:
                    EmitFloatSerialize(il, fieldType, option);
                    break;

                case WireType.Char:
                    EmitCharSerialize(il, field, option);
                    break;

                case WireType.String:
                    EmitStringSerialize(il, field, option);
                    break;

                case WireType.Boolean:
                    EmitBooleanSerialize(il, option);
                    break;

                default:
                    EmitComplexSerialize(il, fieldType, option);
                    break;
            }

            if (checkRef && (!fieldType.IsValueType) && (option == SerializerOption.Normal))
            {
                il.MarkLabel(label);
            }
        }

        private static void EmitIntSerialize(ILGenerator il, FieldInfo field, Type type, SerializerOption option)
        {
            EmitNumberFormat(il, field);
            MethodInfo method;

            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            if (option == SerializerOption.Normal)
            {
                s_WriteValueIntWithIndexTypes[1] = type;
                method = s_WriterType.GetMethod(Resources.WriteValueMethodName, s_WriteValueIntWithIndexTypes);
            }
            else
            {
                s_WriteValueIntTypes[0] = type;
                method = s_WriterType.GetMethod(Resources.WriteValueMethodName, s_WriteValueIntTypes);
            }

            il.Emit(OpCodes.Call, method);
        }

        private static void EmitFloatSerialize(ILGenerator il, Type type, SerializerOption option)
        {
            MethodInfo method;

            if (option == SerializerOption.Normal)
            {
                s_WriteValueFloatWithIndexTypes[1] = type;
                method = s_WriterType.GetMethod(Resources.WriteValueMethodName, s_WriteValueFloatWithIndexTypes);
            }
            else
            {
                s_WriteValueFloatTypes[0] = type;
                method = s_WriterType.GetMethod(Resources.WriteValueMethodName, s_WriteValueFloatTypes);
            }

            il.Emit(OpCodes.Call, method);
        }

        private static void EmitCharSerialize(ILGenerator il, FieldInfo field, SerializerOption option)
        {
            EmitEncoding(il, field);
            MethodInfo method = option == SerializerOption.Normal ? s_WriteValueCharWithIndexMethod : s_WriteValueCharMethod;
            il.Emit(OpCodes.Call, method);
        }

        private static void EmitStringSerialize(ILGenerator il, FieldInfo field, SerializerOption option)
        {
            EmitEncoding(il, field);
            MethodInfo method = option == SerializerOption.Normal ? s_WriteValueStringWithIndexMethod : s_WriteValueStringMethod;
            il.Emit(OpCodes.Call, method);
        }

        private static void EmitBooleanSerialize(ILGenerator il, SerializerOption option)
        {
            MethodInfo method = option == SerializerOption.Normal ? s_WriteValueBoolWithIndexMethod : s_WriteValueBoolMethod;
            il.Emit(OpCodes.Call, method);
        }

        private static void EmitComplexSerialize(ILGenerator il, Type type, SerializerOption option)
        {
             MethodInfo method;

            if (option == SerializerOption.Normal)
            {
                method = s_WriteValueComplexWithIndexMethod.MakeGenericMethod(type);
            }
            else
            {
                method = s_WriteValueComplexTypesMethod.MakeGenericMethod(type);
            }

            il.Emit(OpCodes.Call, method);
        }
    }
}
