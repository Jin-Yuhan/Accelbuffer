using Accelbuffer.Properties;
using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection.IL.Serializers.Progress
{
    internal sealed class DeserializeMethodDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, IEnumerable<AccelFieldInfo> fields, int fieldCount, bool hasContinuousSerialIndexes, MethodInfo beforeMethod, MethodInfo afterMethod)
        {
            MethodBuilder methodBuilder = RuntimeInjector.DefineDeserializeMethod(builder, objType, interfaceType);

            ILGenerator il = methodBuilder.GetILGenerator();

            Predefine(il, objType);
            EmitIL(il, objType, fields, fieldCount, hasContinuousSerialIndexes, afterMethod);

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
        }

        private void Predefine(ILGenerator il, Type objType)
        {
            il.DeclareLocal(objType);
            il.DeclareLocal(typeof(int));

            if (objType.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)0);
                il.Emit(OpCodes.Initobj, objType);
            }
            else
            {
                il.Emit(OpCodes.Newobj, objType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_0);
            }
        }

        private void EmitIL(ILGenerator il, Type objType, IEnumerable<AccelFieldInfo> fields, int fieldCount, bool hasContinuousSerialIndexes, MethodInfo afterMethod)
        {
            if (fieldCount > 0)
            {
                EmitFieldsDeserialize(il, objType, fields, fieldCount, hasContinuousSerialIndexes);
            }

            if (afterMethod != null)
            {
                EmitMessageCallback(il, objType, afterMethod);
            }
        }

        private static void EmitMessageCallback(ILGenerator il, Type objType, MethodInfo method)
        {
            if (objType.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)0);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_0);
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

        private static void EmitFieldsDeserialize(ILGenerator il, Type objType, IEnumerable<AccelFieldInfo> fields, int fieldCount, bool hasContinuousSerialIndexes)
        {
            if (hasContinuousSerialIndexes)
            {
                EmitFieldsDeserializeSwith(il, objType, fields, fieldCount);
            }
            else
            {
                EmitFieldsDeserializeIf(il, objType, fields, fieldCount);
            }
        }

        private static void EmitFieldsDeserializeSwith(ILGenerator il, Type objType, IEnumerable<AccelFieldInfo> fields, int fieldCount)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label defaultCaseLabel = il.DefineLabel();
            Label[] cases = new Label[fieldCount];

            for (int j = 0; j < fieldCount; j++)
            {
                cases[j] = il.DefineLabel();
            }

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldarg_1);//循环开始

            il.Emit(OpCodes.Ldloca_S, (byte)1);//&index
            il.Emit(OpCodes.Call, HasNextMethod);//HasNext(out index);
            il.Emit(OpCodes.Brfalse, endLabel);

            //case ...:
            int i = 0;

            foreach (AccelFieldInfo fieldInfo in fields)
            {
                FieldInfo field = fieldInfo.Info;

                if (i == 0)
                {
                    il.Emit(OpCodes.Ldloc_1);
                    EmitLdc_I4(il, fieldInfo.SerialIndex);
                    il.Emit(OpCodes.Sub);
                    il.Emit(OpCodes.Switch, cases);

                    il.Emit(OpCodes.Br, defaultCaseLabel);
                }

                il.MarkLabel(cases[i]);

                if (objType.IsValueType)
                {
                    il.Emit(OpCodes.Ldloca_S, (byte)0);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc_0);
                }

                il.Emit(OpCodes.Ldarg_1);

                bool isFacadeType = fieldInfo.IsFacadeType;
                Type fieldType = !isFacadeType && field.FieldType.IsEnum ? Enum.GetUnderlyingType(field.FieldType) : field.FieldType;
                Type deserializeType = isFacadeType ? fieldInfo.RealFieldType : fieldType;                

                MethodInfo method;

                if (IsBuiltinType(deserializeType, out string name))
                {
                    method = typeof(AccelReader).GetMethod(Resources.ReadName + name, Type.EmptyTypes);
                }
                else
                {
                    method = ReadGenericMethod.MakeGenericMethod(deserializeType);
                }

                il.Emit(OpCodes.Call, method);

                if (isFacadeType)
                {
                    if (!fieldType.IsValueType && !fieldType.IsAssignableFrom(deserializeType))
                    {
                        il.Emit(OpCodes.Isinst, fieldType);
                    }
                    else if (fieldType.IsValueType && !deserializeType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, fieldType);
                    }
                }

                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Br, startLabel);

                i++;
            }

            //default:
            {
                il.MarkLabel(defaultCaseLabel);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, SkipNextMethod);
                il.Emit(OpCodes.Br, startLabel);
            }

            il.MarkLabel(endLabel);//循环结束
        }

        private static void EmitFieldsDeserializeIf(ILGenerator il, Type objType, IEnumerable<AccelFieldInfo> fields, int fieldCount)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label elseLabel = il.DefineLabel();
            Label[] ifs = new Label[fieldCount - 1];

            for (int j = 0; j < ifs.Length; j++)
            {
                ifs[j] = il.DefineLabel();
            }

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldarg_1);//循环开始

            il.Emit(OpCodes.Ldloca_S, (byte)1);//&index
            il.Emit(OpCodes.Call, HasNextMethod);//HasNext(out index);
            il.Emit(OpCodes.Brfalse, endLabel);

            //if:
            int i = 0;

            foreach (AccelFieldInfo fieldInfo in fields)
            {
                FieldInfo field = fieldInfo.Info;
                int index = fieldInfo.SerialIndex;

                if (i > 0)
                {
                    il.MarkLabel(ifs[i - 1]);
                }

                il.Emit(OpCodes.Ldloc_1);
                EmitLdc_I4(il, index);

                if (i == ifs.Length)
                {
                    il.Emit(OpCodes.Bne_Un, elseLabel);
                }
                else
                {
                    il.Emit(OpCodes.Bne_Un, ifs[i]);
                }

                if (objType.IsValueType)
                {
                    il.Emit(OpCodes.Ldloca_S, (byte)0);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc_0);
                }

                il.Emit(OpCodes.Ldarg_1);

                bool isFacadeType = fieldInfo.IsFacadeType;
                Type fieldType = !isFacadeType && field.FieldType.IsEnum ? Enum.GetUnderlyingType(field.FieldType) : field.FieldType;
                Type deserializeType = isFacadeType ? fieldInfo.RealFieldType : fieldType;

                MethodInfo method;

                if (IsBuiltinType(deserializeType, out string name))
                {
                    method = typeof(AccelReader).GetMethod(Resources.ReadName + name, Type.EmptyTypes);
                }
                else
                {
                    method = ReadGenericMethod.MakeGenericMethod(deserializeType);
                }

                il.Emit(OpCodes.Call, method);

                if (isFacadeType)
                {
                    if (!fieldType.IsValueType && !fieldType.IsAssignableFrom(deserializeType))
                    {
                        il.Emit(OpCodes.Isinst, fieldType);
                    }
                    else if (fieldType.IsValueType && !deserializeType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, fieldType);
                    }
                }

                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Br, startLabel);

                i++;
            }

            //else:
            {
                il.MarkLabel(elseLabel);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, SkipNextMethod);
                il.Emit(OpCodes.Br, startLabel);
            }

            il.MarkLabel(endLabel);//循环结束
        }
    }
}
