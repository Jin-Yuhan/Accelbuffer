using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Accelbuffer
{
    internal static partial class ILEmitUtility
    {
        private static readonly Type s_ReaderType = typeof(UnmanagedReader);
        private static readonly Type s_WriterType = typeof(UnmanagedWriter);
        private static readonly Type s_WriterPtrType = typeof(UnmanagedWriter*);
        private static readonly Type[] s_ReaderPtrTypes = new Type[] { typeof(UnmanagedReader*) };
        private static readonly Type[] s_IndexAndCharEncodingTypes = new Type[] { typeof(byte), typeof(CharEncoding) };
        private static readonly Type[] s_IndexAndCharAndCharEncodingTypes = new Type[] { typeof(byte), typeof(char), typeof(CharEncoding) };
        private static readonly Type[] s_IndexAndStringAndCharEncodingTypes = new Type[] { typeof(byte), typeof(string), typeof(CharEncoding) };
        private static readonly Type[] s_IndexAndBoolTypes = new Type[] { typeof(byte), typeof(bool) };
        private static readonly Type[] s_IndexTypes = new Type[] { typeof(byte) };

        private static readonly string s_WriteValueString = "WriteValue";
        private static readonly string s_ReadString = "Read";
        private static readonly string s_FixedName = "Fixed";
        private static readonly string s_VariableName = "Variable";
        private static readonly string s_SerializeName = "Serialize";
        private static readonly string s_DeserializeName = "Deserialize";

        private static CharEncoding GetCharEncoding(this FieldInfo field)
        {
            EncodingAttribute attribute = field.GetCustomAttribute<EncodingAttribute>(true);
            return attribute == null ? SerializationUtility.GlobalDefaultCharEncoding : attribute.Encoding;
        }

        private static NumberOption GetNumberOption(this FieldInfo field)
        {
            NumberAttribute attr = field.GetCustomAttribute<NumberAttribute>(true);
            return attr == null ? SerializationUtility.GlobalDefaultNumberOption : attr.Option;
        }

        private static void EmitEncoding(this ILGenerator il, CharEncoding encoding)
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

        private static void EmitIsFixedNumber(this ILGenerator il, NumberOption option)
        {
            il.Emit(option == NumberOption.FixedLength ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
        }

        public static List<FieldData> GetSerializedFields(this Type objType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo[] allFields = objType.GetFields(flags);
            List<FieldData> fields = new List<FieldData>(allFields.Length);

            for (int i = 0; i < allFields.Length; i++)
            {
                FieldInfo field = allFields[i];

                if (field.GetCustomAttribute<CompilerGeneratedAttribute>() != null || field.IsInitOnly)
                {
                    continue;
                }

                SerializeIncludeAttribute attribute = field.GetCustomAttribute<SerializeIncludeAttribute>(true);

                if (attribute != null)
                {
                    fields.Add(new FieldData(field, attribute.SerializeIndex));
                }
            }

            fields.Sort((f1, f2) => f1.Index - f2.Index);
            return fields;
        }
    }
}
