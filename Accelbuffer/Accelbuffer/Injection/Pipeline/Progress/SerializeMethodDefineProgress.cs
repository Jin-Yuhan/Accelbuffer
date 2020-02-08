using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection
{
    internal sealed class SerializeMethodDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, MethodInfo beforeMethod, MethodInfo afterMethod)
        {
            MethodBuilder method = builder.DefineMethod(Resources.SerializeMethodName,
                                                     s_MethodAttributes,
                                                     s_CallingConventions,
                                                     typeof(void),
                                                     GetSerializeMethodArguments(objType));

            ILGenerator il = method.GetILGenerator();
            EmitIL(il, objType, fields, beforeMethod);
            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(Resources.SerializeMethodName));
        }

        private void EmitIL(ILGenerator il, Type objType, List<FieldData> fields, MethodInfo method)
        {
            if (method != null)
            {
                EmitMessageCallback(il, objType, method);
            }

            for (int i = 0; i < fields.Count; i++)
            {
                FieldData data = fields[i];
                EmitFieldSerialize(il, data.Field, data.Index, data.NeverNull);
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

        private static void EmitFieldSerialize(ILGenerator il, FieldInfo field, int index, bool neverNull)
        {
            Type fieldType = field.FieldType;
            Label label = default;

            if (fieldType.IsEnum)
            {
                fieldType = Enum.GetUnderlyingType(fieldType);
            }

            if (!(neverNull || fieldType.IsValueType))
            {
                label = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_1);//arg
                il.Emit(OpCodes.Ldfld, field);//field
                il.Emit(OpCodes.Brfalse, label);
            }

            il.Emit(OpCodes.Ldarg_2);//writer
            il.Emit(OpCodes.Ldc_I4, index);//index
            il.Emit(OpCodes.Ldarg_1);//arg
            il.Emit(OpCodes.Ldfld, field);//field

            MethodInfo method;

            if (IsBuiltinType(fieldType))
            {
                method = s_WriterType.GetMethod(Resources.WriteValueMethodName, GetWriteValueMethodArguments(fieldType));
            }
            else
            {
                method = s_WriteValueGenericMethod.MakeGenericMethod(fieldType);
            }

            il.Emit(OpCodes.Call, method);

            if (!(neverNull || fieldType.IsValueType))
            {
                il.MarkLabel(label);
            }
        }
    }
}
