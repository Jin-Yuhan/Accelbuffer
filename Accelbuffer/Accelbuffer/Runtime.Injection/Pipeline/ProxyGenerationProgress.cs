using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Accelbuffer.Runtime.Injection
{
    internal abstract class ProxyGenerationProgress
    {
        protected static readonly string s_SerializeName = "Serialize";
        protected static readonly string s_DeserializeName = "Deserialize";

        protected static readonly MethodAttributes s_MethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        protected static readonly CallingConventions s_CallingConventions = CallingConventions.Standard;

        protected static readonly Type s_ReaderType = typeof(UnmanagedReader);
        protected static readonly Type s_WriterType = typeof(UnmanagedWriter);

        protected static readonly Type[] s_SerializeTypes = new Type[] { null, typeof(UnmanagedWriter).MakeByRefType(), typeof(SerializationContext) };
        protected static readonly Type[] s_DeserializeTypes = new Type[] { typeof(UnmanagedReader).MakeByRefType(), typeof(SerializationContext) };

        protected static readonly Type[] s_SerializeTypes4 = new Type[] { typeof(byte), null, typeof(UnmanagedWriter).MakeByRefType(), typeof(SerializationContext) };
        protected static readonly Type[] s_DeserializeTypes4 = new Type[] { typeof(byte), typeof(UnmanagedReader).MakeByRefType(), typeof(SerializationContext) };

        protected static readonly Type[] s_IndexAndCharEncodingTypes = new Type[] { typeof(byte), typeof(CharEncoding) };
        protected static readonly Type[] s_IndexAndCharEncodingTypes3 = new Type[] { typeof(byte), null, typeof(CharEncoding) };
        protected static readonly Type[] s_IndexAndBoolTypes = new Type[] { typeof(byte), typeof(bool) };
        protected static readonly Type[] s_IndexAndNumberTypes = new Type[] { typeof(byte), typeof(Number) };
        protected static readonly Type[] s_IndexAndNumberTypes3 = new Type[] { typeof(byte), null, typeof(Number) };
        protected static readonly Type[] s_IndexTypes = new Type[] { typeof(byte) };

        protected static readonly FieldInfo s_ContextDefaultEncoding = typeof(SerializationContext).GetField("DefaultEncoding");
        protected static readonly FieldInfo s_ContextDefaultNumberType = typeof(SerializationContext).GetField("DefaultNumberType");

        protected static readonly ConstructorInfo s_ContextCtor = typeof(SerializationContext).GetConstructor(new Type[] { typeof(CharEncoding), typeof(Number) });

        protected static readonly string s_WriteValueString = "WriteValue";
        protected static readonly string s_ReadString = "Read";

        public abstract void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods);

        protected static CharEncoding GetCharEncoding(FieldInfo field, out bool useContext)
        {
            EncodingAttribute attribute = field.GetCustomAttribute<EncodingAttribute>(true);
            useContext = attribute == null;
            return useContext ? default : attribute.Encoding;
        }

        protected static Number GetNumberType(FieldInfo field, out bool useContext)
        {
            NumberTypeAttribute attr = field.GetCustomAttribute<NumberTypeAttribute>(true);
            useContext = attr == null;
            return useContext ? default : attr.NumberType;
        }

        protected static void EmitEncoding(ILGenerator il, CharEncoding encoding)
        {
            switch (encoding)
            {
                case CharEncoding.Unicode:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case CharEncoding.ASCII:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
            }
        }

        protected static void EmitIsFixedNumber(ILGenerator il, Number type)
        {
            il.Emit(type == Number.Fixed ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
        }

        protected static void EmitContext(ILGenerator il, FieldInfo field, OpCode context)
        {
            Number numberType = GetNumberType(field, out bool f1);
            CharEncoding encoding = GetCharEncoding(field, out bool f2);

            if (f1 && f2)
            {
                il.Emit(context);//context
            }
            else
            {
                if (f2)
                {
                    il.Emit(context);
                    il.Emit(OpCodes.Ldfld, s_ContextDefaultEncoding);
                }
                else
                {
                    EmitEncoding(il, encoding);
                }

                if (f1)
                {
                    il.Emit(context);
                    il.Emit(OpCodes.Ldfld, s_ContextDefaultNumberType);
                }
                else
                {
                    EmitIsFixedNumber(il, numberType);
                }

                il.Emit(OpCodes.Newobj, s_ContextCtor);
            }
        }

        protected static void EmitNumberType(ILGenerator il, FieldInfo field, OpCode context)
        {
            Number numberType = GetNumberType(field, out bool useContext);

            if (useContext)
            {
                il.Emit(context);
                il.Emit(OpCodes.Ldfld, s_ContextDefaultNumberType);
            }
            else
            {
                EmitIsFixedNumber(il, numberType);
            }
        }

        protected static void EmitEncoding(ILGenerator il, FieldInfo field, OpCode context)
        {
            CharEncoding encoding = GetCharEncoding(field, out bool useContext);

            if (useContext)
            {
                il.Emit(context);
                il.Emit(OpCodes.Ldfld, s_ContextDefaultEncoding);
            }
            else
            {
                EmitEncoding(il, encoding);
            }
        }
    }
}
