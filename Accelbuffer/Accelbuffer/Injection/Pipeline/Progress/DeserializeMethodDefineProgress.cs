using Accelbuffer.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection
{
    internal sealed class DeserializeMethodDefineProgress : SerializerGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods, SerializerOption option)
        {
            MethodBuilder method = builder.DefineMethod(Resources.DeserializeMethodName,
                                                        s_MethodAttributes,
                                                        s_CallingConventions,
                                                        objType,
                                                        s_DeserializeTypes);

            ILGenerator il = method.GetILGenerator();

            Predefine(il, objType);
            EmitIL(il, objType, fields, methods, option);

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

        private void EmitIL(ILGenerator il, Type objType, List<FieldData> fields, List<MethodData> methods, SerializerOption option)
        {
            if (option == SerializerOption.Normal)
            {
                EmitFieldsDeserializeNormal(il, objType, fields);
            }
            else
            {
                EmitFieldsDeserializeCompact(il, objType, fields);
            }

            for (int i = 0; i < methods.Count; i++)
            {
                MethodData data = methods[i];

                if (data.Type == AccelbufferCallback.OnAfterDeserialization)
                {
                    EmitMessageCallback(il, objType, data.Method);
                }
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

        private static void EmitFieldsDeserializeNormal(ILGenerator il, Type objType, List<FieldData> fields)
        {
            if (IsFieldIndexContinuous(fields))
            {
                EmitFieldsDeserializeNormalSwith(il, objType, fields);
            }
            else
            {
                EmitFieldsDeserializeNormalIf(il, objType, fields);
            }
        }

        private static void EmitFieldsDeserializeNormalSwith(ILGenerator il, Type objType, List<FieldData> fields)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label defaultCaseLabel = il.DefineLabel();
            Label[] cases = new Label[fields.Count];

            for (int i = 0; i < cases.Length; i++)
            {
                cases[i] = il.DefineLabel();
            }

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldarg_1);//循环开始

            il.Emit(OpCodes.Ldloca_S, (byte)1);//&index
            il.Emit(OpCodes.Call, s_HasNextOutIndexMethod);//HasNext(out index);
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

                WireType type = GetWireType(fieldType, out string name);

                switch (type)
                {
                    case WireType.Int:
                        EmitIntDeserialize(il, field, name, SerializerOption.Normal);
                        break;

                    case WireType.Boolean:
                    case WireType.Float:
                        EmitFloatAndBooleanDeserialize(il, name, SerializerOption.Normal);
                        break;

                    case WireType.Char:
                    case WireType.String:
                        EmitCharDeserialize(il, field, name, SerializerOption.Normal);
                        break;

                    default:
                        EmitComplexDeserialize(il, field.FieldType, SerializerOption.Normal);
                        break;
                }

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

        private static void EmitFieldsDeserializeNormalIf(ILGenerator il, Type objType, List<FieldData> fields)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label elseLabel = il.DefineLabel();
            Label[] ifs = new Label[fields.Count - 1];

            for (int i = 0; i < ifs.Length; i++)
            {
                ifs[i] = il.DefineLabel();
            }

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldarg_1);//循环开始

            il.Emit(OpCodes.Ldloca_S, (byte)1);//&index
            il.Emit(OpCodes.Call, s_HasNextOutIndexMethod);//HasNext(out index);
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

                WireType type = GetWireType(fieldType, out string name);

                switch (type)
                {
                    case WireType.Int:
                        EmitIntDeserialize(il, field, name, SerializerOption.Normal);
                        break;

                    case WireType.Boolean:
                    case WireType.Float:
                        EmitFloatAndBooleanDeserialize(il, name, SerializerOption.Normal);
                        break;

                    case WireType.Char:
                    case WireType.String:
                        EmitCharDeserialize(il, field, name, SerializerOption.Normal);
                        break;

                    default:
                        EmitComplexDeserialize(il, field.FieldType, SerializerOption.Normal);
                        break;
                }

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

        private static void EmitFieldsDeserializeCompact(ILGenerator il, Type objType, List<FieldData> fields)
        {
            Label startLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();
            Label AddIndexLabel = il.DefineLabel();
            Label defaultCaseLabel = il.DefineLabel();
            Label[] cases = new Label[fields.Count];

            for (int i = 0; i < cases.Length; i++)
            {
                cases[i] = il.DefineLabel();
            }

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Br, endLabel);

            il.MarkLabel(startLabel);
            il.Emit(OpCodes.Ldloc_1);//循环开始
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

                WireType type = GetWireType(fieldType, out string name);

                switch (type)
                {
                    case WireType.Int:
                        EmitIntDeserialize(il, field, name, SerializerOption.CompactLayout);
                        break;

                    case WireType.Boolean:
                    case WireType.Float:
                        EmitFloatAndBooleanDeserialize(il, name, SerializerOption.CompactLayout);
                        break;

                    case WireType.Char:
                    case WireType.String:
                        EmitCharDeserialize(il, field, name, SerializerOption.CompactLayout);
                        break;

                    default:
                        EmitComplexDeserialize(il, field.FieldType, SerializerOption.CompactLayout);
                        break;
                }

                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Br, AddIndexLabel);
            }

            //default:
            {
                il.MarkLabel(defaultCaseLabel);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
            }

            il.MarkLabel(AddIndexLabel);//i++;
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_1);
            
            il.MarkLabel(endLabel);//循环判断
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, s_HasNextMethod);
            il.Emit(OpCodes.Brtrue, startLabel);
        }

        private static void EmitIntDeserialize(ILGenerator il, FieldInfo field, string name, SerializerOption option)
        {
            MethodInfo method;

            if (option == SerializerOption.CompactLayout)
            {
                EmitNumberFormat(il, field);
                method = s_IteratorType.GetMethod(string.Format(Resources.NextAsWithoutTagName, name));
            }
            else
            {
                method = s_IteratorType.GetMethod(Resources.NextAsName + name);
            }

            il.Emit(OpCodes.Call, method);
        }

        private static void EmitFloatAndBooleanDeserialize(ILGenerator il, string name, SerializerOption option)
        {
            name = option == SerializerOption.CompactLayout ? string.Format(Resources.NextAsWithoutTagName, name) : Resources.NextAsName + name;
            MethodInfo method = s_IteratorType.GetMethod(name);
            il.Emit(OpCodes.Call, method);
        }

        private static void EmitCharDeserialize(ILGenerator il, FieldInfo field, string name, SerializerOption option)
        {
            MethodInfo method;

            if (option == SerializerOption.CompactLayout)
            {
                EmitEncoding(il, field);
                method = s_IteratorType.GetMethod(string.Format(Resources.NextAsWithoutTagName, name));
            }
            else
            {
                method = s_IteratorType.GetMethod(Resources.NextAsName + name);
            }

            il.Emit(OpCodes.Call, method);
        }

        private static void EmitComplexDeserialize(ILGenerator il, Type type, SerializerOption option)
        {
            if (option == SerializerOption.CompactLayout)
            {
                il.Emit(OpCodes.Call, s_IteratorNextAsComplexWithoutTagMethod.MakeGenericMethod(type));
            }
            else
            {
                il.Emit(OpCodes.Call, s_IteratorNextAsComplexMethod.MakeGenericMethod(type));
            }
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
