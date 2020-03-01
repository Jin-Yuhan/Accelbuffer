using Accelbuffer.Properties;
using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection.IL.Serializers.Progress
{
    internal sealed class SerializeMethodDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, IEnumerable<AccelFieldInfo> fields, int fieldCount, bool hasContinuousSerialIndexes, MethodInfo beforeMethod, MethodInfo afterMethod)
        {
            MethodBuilder methodBuilder = RuntimeInjector.DefineSerializeMethod(builder, objType, interfaceType);
            ILGenerator il = methodBuilder.GetILGenerator();
            EmitIL(il, objType, fields, beforeMethod);
            il.Emit(OpCodes.Ret);
        }

        private void EmitIL(ILGenerator il, Type objType, IEnumerable<AccelFieldInfo> fields, MethodInfo method)
        {
            if (method != null)
            {
                EmitMessageCallback(il, objType, method);
            }

            foreach (AccelFieldInfo field in fields)
            {
                EmitFieldSerialize(il, field.Info, field.SerialIndex, field.IsNeverNull, field.IsFacadeType, field.RealFieldType);
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

        private static void EmitFieldSerialize(ILGenerator il, FieldInfo field, int index, bool isNeverNull, bool isFacadeType, Type realType)
        {
            Type fieldType = field.FieldType;
            Label label = default;

            if (!isFacadeType && fieldType.IsEnum)
            {
                fieldType = Enum.GetUnderlyingType(fieldType);
            }

            if (!(isNeverNull || fieldType.IsValueType))
            {
                label = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_1);//arg
                il.Emit(OpCodes.Ldfld, field);//field
                il.Emit(OpCodes.Brfalse, label);
            }

            il.Emit(OpCodes.Ldarg_2);//writer
            EmitLdc_I4(il, index);//index
            il.Emit(OpCodes.Ldarg_1);//arg
            il.Emit(OpCodes.Ldfld, field);//field

            MethodInfo method;
            Type serializeType = isFacadeType ? realType : fieldType;

            if (isFacadeType)
            {
                if (!fieldType.IsValueType && !realType.IsAssignableFrom(fieldType))
                {
                    il.Emit(OpCodes.Isinst, realType);
                }
                else if (fieldType.IsValueType && !realType.IsValueType)
                {
                    il.Emit(OpCodes.Box, fieldType);
                }
            }

            if (IsBuiltinType(serializeType))
            {
                method = typeof(AccelWriter).GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(int), serializeType });
            }
            else
            {
                method = WriteValueGenericMethod.MakeGenericMethod(serializeType);
            }

            il.Emit(OpCodes.Call, method);

            if (!(isNeverNull || fieldType.IsValueType))
            {
                il.MarkLabel(label);
            }
        }
    }
}
