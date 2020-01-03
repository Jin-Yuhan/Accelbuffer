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
        protected static readonly Type[] s_ReaderTypeRef = new Type[] { typeof(UnmanagedReader).MakeByRefType() };

        protected static readonly Type s_WriterType = typeof(UnmanagedWriter);
        protected static readonly Type[] s_WriterTypeRef = new Type[] { typeof(UnmanagedWriter).MakeByRefType() };

        protected static readonly Type[] s_SerializeTypes = new Type[] { null, s_WriterTypeRef[0] };
        protected static readonly Type[] s_SerializeCallTypes = new Type[] { null, s_WriterTypeRef[0] };

        protected static readonly Type[] s_IndexAndCharEncodingTypes = new Type[] { typeof(byte), typeof(CharEncoding) };
        protected static readonly Type[] s_IndexAndCharEncodingTypes3 = new Type[] { typeof(byte), null, typeof(CharEncoding) };
        protected static readonly Type[] s_IndexAndBoolTypes = new Type[] { typeof(byte), typeof(bool) };
        protected static readonly Type[] s_IndexAndNumberTypes = new Type[] { typeof(byte), null, typeof(Number) };
        protected static readonly Type[] s_IndexTypes = new Type[] { typeof(byte) };

        protected static readonly string s_WriteValueString = "WriteValue";
        protected static readonly string s_ReadString = "Read";
        protected static readonly string s_FixedName = "Fixed";
        protected static readonly string s_VariableName = "Variable";

        public abstract void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods);

        protected static CharEncoding GetCharEncoding(FieldInfo field)
        {
            EncodingAttribute attribute = field.GetCustomAttribute<EncodingAttribute>(true);
            return attribute == null ? SerializationUtility.DefaultCharEncoding : attribute.Encoding;
        }

        protected static Number GetNumberType(FieldInfo field)
        {
            NumberTypeAttribute attr = field.GetCustomAttribute<NumberTypeAttribute>(true);
            return attr == null ? SerializationUtility.DefaultNumberType : attr.NumberType;
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

        protected static void EmitIsFixedNumber(ILGenerator il, Number option)
        {
            il.Emit(option == Number.Fixed ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
        }
    }
}
