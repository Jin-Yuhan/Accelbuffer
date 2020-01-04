using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static Accelbuffer.SerializationUtility;

namespace Accelbuffer.Runtime.Injection
{
    internal sealed class DeserializeMethodDefineProgress : ProxyGenerationProgress
    {
        public override void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods)
        {
            MethodBuilder method = builder.DefineMethod(s_DeserializeName,
                                                        s_MethodAttributes,
                                                        s_CallingConventions,
                                                        objType,
                                                        s_DeserializeTypes);

            ILGenerator il = method.GetILGenerator();

            Predefine(il, objType);
            EmitIL(il, objType, fields, methods);

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            builder.DefineMethodOverride(method, interfaceType.GetMethod(s_DeserializeName));
        }

        private void Predefine(ILGenerator il, Type objType)
        {
            il.DeclareLocal(objType);

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

        private void EmitIL(ILGenerator il, Type objType, List<FieldData> fields, List<MethodData> methods)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                FieldData data = fields[i];
                EmitFieldDeserialize(il, objType, data.Field, data.Index);
            }

            for (int i = 0; i < methods.Count; i++)
            {
                MethodData data = methods[i];

                if (data.Type == SerializationCallbackMethod.OnAfterDeserialize)
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

        private static void EmitSerializerDotDeserializeCall(ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Call, typeof(Serializer<>).MakeGenericType(type).GetMethod(s_DeserializeName, s_DeserializeTypes));
        }

        private static void EmitLoadReaderAndContext(ILGenerator il, FieldInfo field)
        {
            il.Emit(OpCodes.Ldarg_1);
            EmitContext(il, field, OpCodes.Ldarg_2);
        }

        private static void EmitLoadReaderAndIndex(ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4, (int)index);
        }

        private static void EmitFieldDeserialize(ILGenerator il, Type objType, FieldInfo field, byte index)
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
                EmitLoadReaderAndIndex(il, index);
            }

            switch (type)
            {
                case SerializedType.Number:
                    
                    EmitNumberDeserialize(il, field, name);
                    break;

                case SerializedType.Char:
                    
                    EmitCharDeserialize(il, field, name);
                    break;

                case SerializedType.Boolean:
                    EmitBooleanDeserialize(il, name);
                    break;

                default:
                    EmitLoadReaderAndContext(il, field);
                    EmitSerializerDotDeserializeCall(il, field.FieldType);
                    break;
            }

            il.Emit(OpCodes.Stfld, field);
        }

        private static void EmitNumberDeserialize(ILGenerator il, FieldInfo field, string name)
        {
            string methodName = s_ReadString + name;

            EmitNumberType(il, field, OpCodes.Ldarg_2);
            MethodInfo method = s_ReaderType.GetMethod(methodName, s_IndexAndNumberTypes);
            il.Emit(OpCodes.Call, method);
        }

        private static void EmitCharDeserialize(ILGenerator il, FieldInfo field, string name)
        {
            string methodName = s_ReadString + name;

            EmitEncoding(il, field, OpCodes.Ldarg_2);
            MethodInfo method = s_ReaderType.GetMethod(methodName, s_IndexAndCharEncodingTypes);
            il.Emit(OpCodes.Call, method);
        }

        private static void EmitBooleanDeserialize(ILGenerator il, string name)
        {
            string methodName = $"{s_ReadString}{name}";
            MethodInfo method = s_ReaderType.GetMethod(methodName, s_IndexTypes);
            il.Emit(OpCodes.Call, method);
        }
    }
}
