using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection
{
    internal sealed class DeserializeMethodDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, MethodInfo beforeMethod, MethodInfo afterMethod)
        {
            MethodBuilder method = builder.DefineMethod(Resources.DeserializeMethodName,
                                                        s_MethodAttributes,
                                                        s_CallingConventions,
                                                        objType,
                                                        GetDeserializeMethodArguments());

            ILGenerator il = method.GetILGenerator();

            Predefine(il, objType);
            EmitIL(il, objType, fields, afterMethod);

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(Resources.DeserializeMethodName));
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

        private void EmitIL(ILGenerator il, Type objType, List<FieldData> fields, MethodInfo afterMethod)
        {
            EmitFieldsDeserialize(il, objType, fields);
            EmitMessageCallback(il, objType, afterMethod);
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

        private static void EmitFieldsDeserialize(ILGenerator il, Type objType, List<FieldData> fields)
        {
            if (IsFieldIndexContinuous(fields))
            {
                EmitFieldsDeserializeSwith(il, objType, fields);
            }
            else
            {
                EmitFieldsDeserializeIf(il, objType, fields);
            }
        }

        private static void EmitFieldsDeserializeSwith(ILGenerator il, Type objType, List<FieldData> fields)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label defaultCaseLabel = il.DefineLabel();
            Label[] cases = new Label[fields.Count];

            for (int i = 0; i < fields.Count; i++)
            {
                cases[i] = il.DefineLabel();
            }

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldarg_1);//循环开始

            il.Emit(OpCodes.Ldloca_S, (byte)1);//&index
            il.Emit(OpCodes.Call, s_HasNextMethod);//HasNext(out index);
            il.Emit(OpCodes.Brfalse, endLabel);

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldc_I4, fields[0].Index);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Switch, cases);

            il.Emit(OpCodes.Br, defaultCaseLabel);

            //case ...:
            for (int i = 0; i < cases.Length; i++)
            {
                FieldInfo field = fields[i].Field;

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

                Type fieldType = field.FieldType;

                if (fieldType.IsEnum)
                {
                    fieldType = Enum.GetUnderlyingType(fieldType);
                }

                MethodInfo method;

                if (IsBuiltinType(fieldType, out string name))
                {
                    method = s_ReaderType.GetMethod(Resources.ReadName + name, Type.EmptyTypes);
                }
                else
                {
                    method = s_ReadGenericMethod.MakeGenericMethod(fieldType);
                }

                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Br, startLabel);
            }

            //default:
            {
                il.MarkLabel(defaultCaseLabel);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, s_SkipNextMethod);
                il.Emit(OpCodes.Br, startLabel);
            }

            il.MarkLabel(endLabel);//循环结束
        }

        private static void EmitFieldsDeserializeIf(ILGenerator il, Type objType, List<FieldData> fields)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label elseLabel = il.DefineLabel();
            Label[] ifs = new Label[fields.Count - 1];

            for (int i = 0; i < fields.Count; i++)
            {
                if (i < ifs.Length)
                {
                    ifs[i] = il.DefineLabel();
                }
            }

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldarg_1);//循环开始

            il.Emit(OpCodes.Ldloca_S, (byte)1);//&index
            il.Emit(OpCodes.Call, s_HasNextMethod);//HasNext(out index);
            il.Emit(OpCodes.Brfalse, endLabel);

            //if:
            for (int i = 0; i < fields.Count; i++)
            {
                FieldInfo field = fields[i].Field;
                int index = fields[i].Index;

                if (i > 0)
                {
                    il.MarkLabel(ifs[i - 1]);
                }

                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldc_I4, index);

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

                Type fieldType = field.FieldType;

                if (fieldType.IsEnum)
                {
                    fieldType = Enum.GetUnderlyingType(fieldType);
                }

                MethodInfo method;

                if (IsBuiltinType(fieldType, out string name))
                {
                    method = s_ReaderType.GetMethod(Resources.ReadName + name, Type.EmptyTypes);
                }
                else
                {
                    method = s_ReadGenericMethod.MakeGenericMethod(fieldType);
                }

                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Br, startLabel);
            }

            //else:
            {
                il.MarkLabel(elseLabel);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, s_SkipNextMethod);
                il.Emit(OpCodes.Br, startLabel);
            }

            il.MarkLabel(endLabel);//循环结束
        }

        private static bool IsFieldIndexContinuous(List<FieldData> fields)
        {
            if (fields.Count < 2)
            {
                return true;
            }

            int last = fields[0].Index;

            for (int i = 1; i < fields.Count; i++)
            {
                if (fields[i].Index - last != 1)
                {
                    return false;
                }

                last = fields[i].Index;
            }

            return true;
        }
    }
}
