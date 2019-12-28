﻿using System;
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
            SerializedType type = GetSerializedType(objType, out _);

            if (type != SerializedType.Complex)
            {
                il.EmitLoadWriterAndIndexAndArg(objType, index);
            }

            switch (type)
            {
                case SerializedType.Number:
                    il.EmitNumberSerialize(SerializationSettings.DefaultNumberOption, objType);
                    break;

                case SerializedType.Char:
                    il.EmitCharSerialize(SerializationSettings.DefaultCharEncoding, objType);
                    break;

                case SerializedType.Boolean:
                    il.EmitBooleanSerialize();
                    break;

                default:
                    if (objType.IsArray)
                    {
                        if (objType.GetArrayRank() > 1)
                        {
                            throw new NotSupportedException("不支持多维数组的序列化");
                        }

                        il.EmitArraySerialize(objType, index);
                    }
                    else
                    {
                        List<FieldData> fields = objType.GetSerializedFields();
                        il.TryEmitMsgMethodS(objType);

                        for (int i = 0; i < fields.Count; i++)
                        {
                            FieldData data = fields[i];
                            il.EmitFieldSerialize(objType, data.Field, data.Index);
                        }
                    }
                    break;
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
                    if (field.FieldType.IsArray)
                    {
                        if (field.FieldType.GetArrayRank() > 1)
                        {
                            throw new NotSupportedException("不支持多维数组的序列化");
                        }

                        il.EmitArraySerialize(objType, field, index);
                    }
                    else
                    {
                        il.EmitLoadSerializeFieldAndWriter(objType, field);
                        il.EmitSerializerDotSerializeCall(field.FieldType);
                    }
                    break;
            }
        }

        public static void EmitArraySerialize(this ILGenerator il, Type objType, byte index)
        {
            LocalBuilder lb = il.DeclareLocal(typeof(int));//int i;

            il.EmitLoadWriterAndIndexAndArg(objType, index);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            EmitNumberSerialize(il, NumberOption.VariableLength, typeof(int));// write len

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, lb);//i = 0;

            Label forLabel1 = il.DefineLabel();
            Label forLabel2 = il.DefineLabel();

            il.Emit(OpCodes.Br_S, forLabel1);
            {
                il.MarkLabel(forLabel2);

                SerializedType type = GetSerializedType(objType.GetElementType(), out _);

                if (type == SerializedType.Complex)
                {
                    il.EmitLoadArg(objType);

                    il.Emit(OpCodes.Ldloc, lb);
                    il.Emit(OpCodes.Ldelem_I4);//array[i]

                    il.EmitLoadWriter();
                }
                else
                {
                    il.EmitLoadWriterAndIndexAndArg(objType, index);
                    il.Emit(OpCodes.Ldloc, lb);
                    il.Emit(OpCodes.Ldelem, objType.GetElementType());//array[i]
                }

                switch (type)
                {
                    case SerializedType.Number:
                        il.EmitNumberSerialize(SerializationSettings.DefaultNumberOption, objType.GetElementType());
                        break;
                    case SerializedType.Char:
                        il.EmitCharSerialize(SerializationSettings.DefaultCharEncoding, objType.GetElementType());
                        break;
                    case SerializedType.Boolean:
                        il.EmitBooleanSerialize();
                        break;
                    default:
                        if (objType.GetElementType().IsArray)
                        {
                            throw new NotSupportedException("不支持交错数组的序列化");
                        }

                        il.EmitSerializerDotSerializeCall(objType.GetElementType());
                        break;
                }

                il.Emit(OpCodes.Ldloc, lb);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, lb);//i++;

                il.MarkLabel(forLabel1);

                il.Emit(OpCodes.Ldloc, lb);
                il.EmitLoadArg(objType);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Conv_I4);
                il.Emit(OpCodes.Blt_S, forLabel2);
            }
        }

        public static void EmitArraySerialize(this ILGenerator il, Type objType, FieldInfo field, byte index)
        {
            LocalBuilder lb = il.DeclareLocal(typeof(int));//int i;

            il.EmitLoadWriterAndIndexAndSerializeField(objType, field, index);
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Conv_I4);
            EmitNumberSerialize(il, NumberOption.VariableLength, typeof(int));// write len

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, lb);//i = 0

            Label forLabel1 = il.DefineLabel();
            Label forLabel2 = il.DefineLabel();

            il.Emit(OpCodes.Br_S, forLabel1);
            {
                il.MarkLabel(forLabel2);

                SerializedType type = GetSerializedType(field.FieldType.GetElementType(), out _);

                if (type == SerializedType.Complex)
                {
                    il.EmitLoadSerializeField(objType, field);
                    
                    il.Emit(OpCodes.Ldloc, lb);
                    il.Emit(OpCodes.Ldelem, field.FieldType.GetElementType());//array[i]

                    il.EmitLoadWriter();
                }
                else
                {
                    il.EmitLoadWriterAndIndexAndSerializeField(objType, field, index);
                    il.Emit(OpCodes.Ldloc, lb);
                    il.Emit(OpCodes.Ldelem_I4);//array[i]
                }

                switch (type)
                {
                    case SerializedType.Number:
                        il.EmitNumberSerialize(field.GetNumberOption(), field.FieldType.GetElementType());
                        break;
                    case SerializedType.Char:
                        il.EmitCharSerialize(field.GetCharEncoding(), field.FieldType.GetElementType());
                        break;
                    case SerializedType.Boolean:                    
                        il.EmitBooleanSerialize();
                        break;
                    default:
                        if (field.FieldType.GetElementType().IsArray)
                        {
                            throw new NotSupportedException("不支持交错数组的序列化");
                        }

                        il.EmitSerializerDotSerializeCall(field.FieldType.GetElementType());
                        break;
                }

                il.Emit(OpCodes.Ldloc, lb);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, lb);//i++;

                il.MarkLabel(forLabel1);

                il.Emit(OpCodes.Ldloc, lb);
                il.EmitLoadSerializeField(objType, field);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Conv_I4);
                il.Emit(OpCodes.Blt_S, forLabel2);
            }
        }

        public static void EmitNumberSerialize(this ILGenerator il, NumberOption option, Type numberType)
        {
            il.EmitIsFixedNumber(option);
            MethodInfo method = s_WriterType.GetMethod(s_WriteValueString, new Type[] { typeof(byte), numberType, typeof(NumberOption) });
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