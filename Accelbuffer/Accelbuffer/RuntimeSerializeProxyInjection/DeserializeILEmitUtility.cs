using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static Accelbuffer.SerializationUtility;

namespace Accelbuffer
{
    internal static partial class ILEmitUtility
    {
        public static void EmitTypeDeserialize(this ILGenerator il, Type objType, bool trulyComplex, byte index)
        {
            SerializedType type = GetSerializedType(objType, out string name);

            if (!trulyComplex)
            {
                il.EmitLoadReaderAndIndex(index);
            }

            switch (type)
            {
                case SerializedType.Number:
                    il.EmitNumberDeserialize(GlobalDefaultNumberTypeOption, name);
                    break;

                case SerializedType.Char:
                    il.EmitCharDeserialize(GlobalDefaultCharEncoding, name);
                    break;

                case SerializedType.Boolean:
                    il.EmitBooleanDeserialize(name);
                    break;

                default:
                    if (objType.IsArray)
                    {
                        if (objType.GetArrayRank() > 1)
                        {
                            throw new NotSupportedException("不支持多维数组反序列化");
                        }

                        il.EmitArrayDeserialize(objType, index);
                    }
                    else
                    {
                        List<FieldData> fields = objType.GetSerializedFields();

                        for (int i = 0; i < fields.Count; i++)
                        {
                            FieldData data = fields[i];
                            il.EmitFieldDeserialize(objType, data.Field, data.Index);
                        }

                        il.TryEmitMsgMethodD(objType);
                    }
                    break;
            }
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
                    if (field.FieldType.IsArray)
                    {
                        if (field.FieldType.GetArrayRank() > 1)
                        {
                            throw new NotSupportedException("不支持多维数组反序列化");
                        }

                        il.EmitLoadReaderAndIndex(index);
                        il.EmitArrayDeserialize(field, index);
                    }
                    else
                    {
                        il.EmitLoadReader();
                        il.EmitSerializerDotDeserializeCall(field.FieldType);
                    }
                    break;
            }

            il.Emit(OpCodes.Stfld, field);
        }

        public static void EmitArrayDeserialize(this ILGenerator il, Type objectType, byte index)
        {
            LocalBuilder lenBuilder = il.DeclareLocal(typeof(int));// int len;
            LocalBuilder arrayBuilder = il.DeclareLocal(objectType);//T[] array;
            LocalBuilder lb = il.DeclareLocal(typeof(int));//int i;

            GetSerializedType(typeof(int), out string name);
            EmitNumberDeserialize(il, Number.Var, name);
            il.Emit(OpCodes.Stloc, lenBuilder);//len = read_len();

            il.Emit(OpCodes.Ldloc, lenBuilder);
            il.Emit(OpCodes.Newarr, objectType.GetElementType());
            il.Emit(OpCodes.Stloc, arrayBuilder);//array = new T[len];

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, lb);//i = 0;

            Label forLabel1 = il.DefineLabel();
            Label forLabel2 = il.DefineLabel();

            il.Emit(OpCodes.Br_S, forLabel1);
            {
                il.MarkLabel(forLabel2);

                il.Emit(OpCodes.Ldloc, arrayBuilder);
                il.Emit(OpCodes.Ldloc, lb);

                SerializedType type = GetSerializedType(objectType.GetElementType(), out name);

                if (type == SerializedType.Complex)
                {
                    il.EmitLoadReader();
                }
                else
                {
                    il.EmitLoadReaderAndIndex(index);
                }

                switch (type)
                {
                    case SerializedType.Number:
                        il.EmitNumberDeserialize(GlobalDefaultNumberTypeOption, name);
                        break;
                    case SerializedType.Char:
                        il.EmitCharDeserialize(GlobalDefaultCharEncoding, name);
                        break;
                    case SerializedType.Boolean:
                        il.EmitBooleanDeserialize(name);
                        break;
                    default:
                        if (objectType.GetElementType().IsArray)
                        {
                            throw new NotSupportedException("不支持交错数组的反序列化");
                        }

                        il.EmitSerializerDotSerializeCall(objectType.GetElementType());
                        break;
                }

                il.Emit(OpCodes.Stelem, objectType.GetElementType());//array[i] = value;

                il.Emit(OpCodes.Ldloc, lb);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, lb);//i++;

                il.MarkLabel(forLabel1);

                il.Emit(OpCodes.Ldloc, lb);
                il.Emit(OpCodes.Ldloc, lenBuilder);
                il.Emit(OpCodes.Blt_S, forLabel2);
            }

            il.Emit(OpCodes.Ldloc, arrayBuilder);
        }

        public static void EmitArrayDeserialize(this ILGenerator il, FieldInfo field, byte index)
        {
            LocalBuilder lenBuilder = il.DeclareLocal(typeof(int));// int len;
            LocalBuilder arrayBuilder = il.DeclareLocal(field.FieldType);//T[] array;
            LocalBuilder lb = il.DeclareLocal(typeof(int));//int i;

            GetSerializedType(typeof(int), out string name);
            EmitNumberDeserialize(il, Number.Var, name);
            il.Emit(OpCodes.Stloc, lenBuilder);//len = read_len();

            il.Emit(OpCodes.Ldloc, lenBuilder);
            il.Emit(OpCodes.Newarr, field.FieldType.GetElementType());
            il.Emit(OpCodes.Stloc, arrayBuilder);//array = new T[len];

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, lb);//i = 0;

            Label forLabel1 = il.DefineLabel();
            Label forLabel2 = il.DefineLabel();

            il.Emit(OpCodes.Br_S, forLabel1);
            {
                il.MarkLabel(forLabel2);

                il.Emit(OpCodes.Ldloc, arrayBuilder);
                il.Emit(OpCodes.Ldloc, lb);

                SerializedType type = GetSerializedType(field.FieldType.GetElementType(), out name);

                if (type == SerializedType.Complex)
                {
                    il.EmitLoadReader();
                }
                else
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
                        if (field.FieldType.GetElementType().IsArray)
                        {
                            throw new NotSupportedException("不支持交错数组的反序列化");
                        }

                        il.EmitSerializerDotSerializeCall(field.FieldType.GetElementType());
                        break;
                }

                il.Emit(OpCodes.Stelem, field.FieldType.GetElementType());//array[i] = value;

                il.Emit(OpCodes.Ldloc, lb);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Stloc, lb);//i++;

                il.MarkLabel(forLabel1);

                il.Emit(OpCodes.Ldloc, lb);
                il.Emit(OpCodes.Ldloc, lenBuilder);
                il.Emit(OpCodes.Blt_S, forLabel2);
            }

            il.Emit(OpCodes.Ldloc, arrayBuilder);
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
