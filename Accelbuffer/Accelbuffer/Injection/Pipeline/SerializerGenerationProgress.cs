using Accelbuffer.Properties;
using Accelbuffer.Text;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Accelbuffer.Injection
{
    internal abstract class SerializerGenerationProgress
    {
        protected static readonly MethodAttributes s_MethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        protected static readonly CallingConventions s_CallingConventions = CallingConventions.Standard;

        protected static readonly Type s_WriterType = typeof(StreamingWriter);
        protected static readonly Type s_IteratorType = typeof(StreamingIterator);

        /// <summary>
        /// 用于定义接口的序列化方法的类型 <see cref="ITypeSerializer{T}.Serialize(T, ref StreamingWriter)"/>
        /// </summary>
        protected static readonly Type[] s_SerializeTypes = new Type[] { null, typeof(StreamingWriter).MakeByRefType() };
        /// <summary>
        /// 用于定义接口的反序列化方法的类型 <see cref="ITypeSerializer{T}.Deserialize(ref StreamingIterator)"/>
        /// </summary>
        protected static readonly Type[] s_DeserializeTypes = new Type[] { typeof(StreamingIterator).MakeByRefType() };


        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(int, int, NumberFormat)"/> etc...
        /// </summary>
        protected static readonly Type[] s_WriteValueIntWithIndexTypes = new Type[] { typeof(int), null, typeof(NumberFormat) };
        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(int, NumberFormat)"/> etc...
        /// </summary>
        protected static readonly Type[] s_WriteValueIntTypes = new Type[] { null, typeof(NumberFormat) };


        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(int, float)"/> etc...
        /// </summary>
        protected static readonly Type[] s_WriteValueFloatWithIndexTypes = new Type[] { typeof(int), null };
        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(float)"/> etc...
        /// </summary>
        protected static readonly Type[] s_WriteValueFloatTypes = new Type[] { null };


        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(int, char, Encoding)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueCharWithIndexMethod
            = s_WriterType.GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(int), typeof(char), typeof(Encoding) });
        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(char, Encoding)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueCharMethod
            = s_WriterType.GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(char), typeof(Encoding) });


        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(int, string, Encoding)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueStringWithIndexMethod
            = s_WriterType.GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(int), typeof(string), typeof(Encoding) });
        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(string, Encoding)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueStringMethod
            = s_WriterType.GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(string), typeof(Encoding) });


        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(int, bool)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueBoolWithIndexMethod 
            = s_WriterType.GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(int), typeof(bool) });
        /// <summary>
        /// <see cref="StreamingWriter.WriteValue(bool)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueBoolMethod
            = s_WriterType.GetMethod(Resources.WriteValueMethodName, new Type[] { typeof(bool) });


        /// <summary>
        /// <see cref="StreamingWriter.WriteValue{T}(int, T)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueComplexWithIndexMethod;
        /// <summary>
        /// <see cref="StreamingWriter.WriteValue{T}(T)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueComplexTypesMethod;


        /// <summary>
        /// <see cref="NumberFormat"/>
        /// </summary>
        protected static readonly Type[] s_NumberFormatTypes = new Type[] { typeof(NumberFormat) };//...
        /// <summary>
        /// <see cref="Encoding"/>
        /// </summary>
        protected static readonly Type[] s_CharEncodingTypes = new Type[] { typeof(Encoding) };//...


        /// <summary>
        /// <see cref="StreamingIterator.HasNext(out int)"/>
        /// </summary>
        protected static readonly MethodInfo s_HasNextOutIndexMethod = s_IteratorType.GetMethod(Resources.HasNextName, new Type[] { typeof(int).MakeByRefType() });
        /// <summary>
        /// <see cref="StreamingIterator.HasNext()"/>
        /// </summary>
        protected static readonly MethodInfo s_HasNextMethod = s_IteratorType.GetMethod(Resources.HasNextName, Type.EmptyTypes);


        /// <summary>
        /// <see cref="StreamingIterator.SkipNext()"/>
        /// </summary>
        protected static readonly MethodInfo s_SkipNextMethod = s_IteratorType.GetMethod(Resources.SkipNextName, Type.EmptyTypes);


        /// <summary>
        /// <see cref="StreamingIterator.NextAs{T}"/>
        /// </summary>
        protected static readonly MethodInfo s_IteratorNextAsComplexMethod = s_IteratorType.GetMethod(Resources.NextAsName + Resources.ComplexName);
        /// <summary>
        /// <see cref="StreamingIterator.NextAsWithoutTag{T}"/>
        /// </summary>
        protected static readonly MethodInfo s_IteratorNextAsComplexWithoutTagMethod = s_IteratorType.GetMethod(string.Format(Resources.NextAsWithoutTagName, Resources.ComplexName));


        static SerializerGenerationProgress()
        {
            foreach (MethodInfo method in s_WriterType.GetMethods())
            {
                if (method.IsGenericMethodDefinition)
                {
                    if (method.GetParameters().Length == 2)
                    {
                        s_WriteValueComplexWithIndexMethod = method;
                    }
                    else
                    {
                        s_WriteValueComplexTypesMethod = method;
                    }
                }
            }
        }


        public abstract void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, List<MethodData> methods, SerializerOption option);

        private static Encoding GetCharEncoding(FieldInfo field)
        {
            EncodingAttribute attribute = field.GetCustomAttribute<EncodingAttribute>(true);
            return attribute == null ? Encoding.UTF8 : attribute.Encoding;
        }

        private static NumberFormat GetNumberFormat(FieldInfo field)
        {
            FixedAttribute attr = field.GetCustomAttribute<FixedAttribute>(true);
            return attr == null ? NumberFormat.Variant : NumberFormat.Fixed;
        }

        private static void EmitEncodingIL(ILGenerator il, Encoding encoding)
        {
            switch (encoding)
            {
                case Encoding.Unicode:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case Encoding.ASCII:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
            }
        }

        private static void EmitNumberFormatIL(ILGenerator il, NumberFormat type)
        {
            il.Emit(type == NumberFormat.Variant ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
        }

        protected static void EmitNumberFormat(ILGenerator il, FieldInfo field)
        {
            EmitNumberFormatIL(il, GetNumberFormat(field));
        }

        protected static void EmitEncoding(ILGenerator il, FieldInfo field)
        {
            EmitEncodingIL(il, GetCharEncoding(field));
        }

        internal static WireType GetWireType(Type type, out string name)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:    name = Resources.SByteName;     return WireType.Int;
                case TypeCode.Byte:     name = Resources.ByteName;      return WireType.Int;
                case TypeCode.Int16:    name = Resources.ShortName;     return WireType.Int;
                case TypeCode.UInt16:   name = Resources.UShortName;    return WireType.Int;
                case TypeCode.Int32:    name = Resources.IntName;       return WireType.Int;
                case TypeCode.UInt32:   name = Resources.UIntName;      return WireType.Int;
                case TypeCode.Int64:    name = Resources.LongName;      return WireType.Int;
                case TypeCode.UInt64:   name = Resources.ULongName;     return WireType.Int;
                case TypeCode.Single:   name = Resources.FloatName;     return WireType.Float;
                case TypeCode.Double:   name = Resources.DoubleName;    return WireType.Float;
                case TypeCode.Decimal:  name = Resources.DecimalName;   return WireType.Float;
                case TypeCode.Boolean:  name = Resources.BoolName;      return WireType.Boolean;
                case TypeCode.Char:     name = Resources.CharName;      return WireType.Char;
                case TypeCode.String:   name = Resources.StringName;    return WireType.String;
                default:                name = Resources.ComplexName;   return WireType.Complex;
            }
        }
    }
}
