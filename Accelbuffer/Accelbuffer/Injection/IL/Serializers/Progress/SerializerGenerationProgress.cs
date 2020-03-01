#if UNITY
using UnityEngine;
#endif

using Accelbuffer.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Resources = Accelbuffer.Properties.Resources;

namespace Accelbuffer.Injection.IL.Serializers.Progress
{
    internal abstract class SerializerGenerationProgress
    {
        private static readonly RuntimeMethodHandle s_WriteValueGenericMethod;
        private static readonly RuntimeMethodHandle s_HasNextMethod;
        private static readonly RuntimeMethodHandle s_SkipNextMethod;
        private static readonly RuntimeMethodHandle s_ReadGenericMethod;

        /// <summary>
        /// <see cref="AccelWriter.WriteValue{T}(int, T)"/>
        /// </summary>
        protected static MethodInfo WriteValueGenericMethod => MethodBase.GetMethodFromHandle(s_WriteValueGenericMethod) as MethodInfo;
        /// <summary>
        /// <see cref="AccelReader.HasNext(out int)"/>
        /// </summary>
        protected static MethodInfo HasNextMethod => MethodBase.GetMethodFromHandle(s_HasNextMethod) as MethodInfo;
        /// <summary>
        /// <see cref="AccelReader.SkipNext()"/>
        /// </summary>
        protected static MethodInfo SkipNextMethod => MethodBase.GetMethodFromHandle(s_SkipNextMethod) as MethodInfo;
        /// <summary>
        /// <see cref="AccelReader.ReadGeneric{T}"/>
        /// </summary>
        protected static MethodInfo ReadGenericMethod=> MethodBase.GetMethodFromHandle(s_ReadGenericMethod) as MethodInfo;

        static SerializerGenerationProgress()
        {
            s_HasNextMethod = typeof(AccelReader).GetMethod(Resources.HasNextName, new Type[] { typeof(int).MakeByRefType() }).MethodHandle;
            s_SkipNextMethod = typeof(AccelReader).GetMethod(Resources.SkipNextName, Type.EmptyTypes).MethodHandle;
            s_ReadGenericMethod = typeof(AccelReader).GetMethod(Resources.ReadName + Resources.GenericName).MethodHandle;

            foreach (MethodInfo method in typeof(AccelWriter).GetMethods())
            {
                if (method.IsGenericMethodDefinition)
                {
                    s_WriteValueGenericMethod = method.MethodHandle;
                    break;
                }
            }
        }

        public abstract void Execute(Type objType, Type interfaceType, TypeBuilder builder, IEnumerable<AccelFieldInfo> fields, int fieldCount, bool hasContinuousSerialIndexes, MethodInfo beforeMethod, MethodInfo afterMethod);

        protected static void EmitLdc_I4(ILGenerator il, int arg)
        {
            switch (arg)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (arg <= sbyte.MaxValue && arg >= sbyte.MinValue)
                        il.Emit(OpCodes.Ldc_I4_S, (byte)arg);
                    else
                        il.Emit(OpCodes.Ldc_I4, arg);
                    break;
            }
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
