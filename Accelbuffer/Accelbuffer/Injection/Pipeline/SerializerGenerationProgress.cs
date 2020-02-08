#if UNITY
using UnityEngine;
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Resources = Accelbuffer.Properties.Resources;

namespace Accelbuffer.Injection
{
    internal abstract class SerializerGenerationProgress
    {
        protected static readonly MethodAttributes s_MethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        protected static readonly CallingConventions s_CallingConventions = CallingConventions.Standard;

        protected static readonly Type s_WriterType = typeof(AccelWriter);
        protected static readonly Type s_ReaderType = typeof(AccelReader);

        /// <summary>
        /// T, AccelWriter&amp;
        /// </summary>
        private static readonly Type[] s_SerializeArguments = new Type[] { null, typeof(AccelWriter).MakeByRefType() };
        /// <summary>
        /// AccelReader&amp;
        /// </summary>
        private static readonly Type[] s_DeserializeMethodArguments = new Type[] { typeof(AccelReader).MakeByRefType() };
        /// <summary>
        /// int, T
        /// </summary>
        private static readonly Type[] s_WriteValueMethodArguments = new Type[] { typeof(int), null };
        /// <summary>
        /// <see cref="AccelWriter.WriteValue{T}(int, T)"/>
        /// </summary>
        protected static readonly MethodInfo s_WriteValueGenericMethod;

        /// <summary>
        /// string
        /// </summary>
        protected static readonly Type[] s_RequireFieldExceptionCtorArgument = new Type[] { typeof(string) };


        /// <summary>
        /// <see cref="AccelReader.HasNext(out int)"/>
        /// </summary>
        protected static readonly MethodInfo s_HasNextMethod = s_ReaderType.GetMethod(Resources.HasNextName, new Type[] { typeof(int).MakeByRefType() });
        /// <summary>
        /// <see cref="AccelReader.SkipNext()"/>
        /// </summary>
        protected static readonly MethodInfo s_SkipNextMethod = s_ReaderType.GetMethod(Resources.SkipNextName, Type.EmptyTypes);
        /// <summary>
        /// <see cref="AccelReader.ReadGeneric{T}"/>
        /// </summary>
        protected static readonly MethodInfo s_ReadGenericMethod = s_ReaderType.GetMethod(Resources.ReadName + Resources.GenericName);


        static SerializerGenerationProgress()
        {
            foreach (MethodInfo method in s_WriterType.GetMethods())
            {
                if (method.IsGenericMethodDefinition)
                {
                    s_WriteValueGenericMethod = method;
                    break;
                }
            }
        }


        public abstract void Execute(Type objType, Type interfaceType, TypeBuilder builder, List<FieldData> fields, MethodInfo beforeMethod, MethodInfo afterMethod);


        protected static Type[] GetSerializeMethodArguments(Type type)
        {
            s_SerializeArguments[0] = type;
            return s_SerializeArguments;
        }

        protected static Type[] GetDeserializeMethodArguments()
        {
            return s_DeserializeMethodArguments;
        }

        protected static Type[] GetWriteValueMethodArguments(Type type)
        {
            s_WriteValueMethodArguments[1] = type;
            return s_WriteValueMethodArguments;
        }

        protected static bool IsBuiltinType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32: 
                case TypeCode.UInt32:
                case TypeCode.Int64: 
                case TypeCode.UInt64: 
                case TypeCode.Single: 
                case TypeCode.Double: 
                case TypeCode.Decimal: 
                case TypeCode.Boolean: 
                case TypeCode.Char:
                case TypeCode.String:
                    return true;
                default:
                    if (type == typeof(VInt))
                        return true;
                    if (type == typeof(VUInt))
                        return true;
                    if (type == typeof(IntPtr))
                        return true;
                    if (type == typeof(UIntPtr))
                        return true;
#if UNITY
                    if (type == typeof(Vector2))
                        return true;
                    if (type == typeof(Vector3))
                        return true;
                    if (type == typeof(Vector4))
                        return true;
                    if (type == typeof(Vector2Int))
                        return true;
                    if (type == typeof(Vector3Int))
                        return true;
                    if (type == typeof(Quaternion))
                        return true;
                    if (type == typeof(Color))
                        return true;
                    if (type == typeof(Color32))
                        return true;
#endif
                    return false;
            }
        }

        protected static bool IsBuiltinType(Type type, out string name)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                    name = Resources.SByteName;
                    return true;
                case TypeCode.Byte:
                    name = Resources.ByteName;
                    return true;
                case TypeCode.Int16:
                    name = Resources.ShortName;
                    return true;
                case TypeCode.UInt16:
                    name = Resources.UShortName;
                    return true;
                case TypeCode.Int32:
                    name = Resources.IntName;
                    return true;
                case TypeCode.UInt32:
                    name = Resources.UIntName;
                    return true;
                case TypeCode.Int64:
                    name = Resources.LongName;
                    return true;
                case TypeCode.UInt64:
                    name = Resources.ULongName;
                    return true;
                case TypeCode.Single:
                    name = Resources.FloatName;
                    return true;
                case TypeCode.Double:
                    name = Resources.DoubleName;
                    return true;
                case TypeCode.Decimal:
                    name = Resources.DecimalName;
                    return true;
                case TypeCode.Boolean:
                    name = Resources.BoolName;
                    return true;
                case TypeCode.Char:
                    name = Resources.CharName;
                    return true;
                case TypeCode.String:
                    name = Resources.StringName;
                    return true;
                default:
                    if (type == typeof(VInt))
                    {
                        name = Resources.VIntName;
                        return true;
                    }

                    if (type == typeof(VUInt))
                    {
                        name = Resources.VUIntName;
                        return true;
                    }

                    if (type == typeof(IntPtr))
                    {
                        name = Resources.IntPtrName;
                        return true;
                    }

                    if (type == typeof(UIntPtr))
                    {
                        name = Resources.UIntPtrName;
                        return true;
                    }
#if UNITY
                    if (type == typeof(Vector2))
                    {
                        name = Resources.Vector2Name;
                        return true;
                    }

                    if (type == typeof(Vector3))
                    {
                        name = Resources.Vector3Name;
                        return true;
                    }

                    if (type == typeof(Vector4))
                    {
                        name = Resources.Vector4Name;
                        return true;
                    }

                    if (type == typeof(Vector2Int))
                    {
                        name = Resources.Vector2IntName;
                        return true;
                    }

                    if (type == typeof(Vector3Int))
                    {
                        name = Resources.Vector3IntName;
                        return true;
                    }

                    if (type == typeof(Quaternion))
                    {
                        name = Resources.QuaternionName;
                        return true;
                    }

                    if (type == typeof(Color))
                    {
                        name = Resources.ColorName;
                        return true;
                    }

                    if (type == typeof(Color32))
                    {
                        name = Resources.Color32Name;
                        return true;
                    }
#endif
                    name = Resources.GenericName;
                    return false;
            }
        }
    }
}
