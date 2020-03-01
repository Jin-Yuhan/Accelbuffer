using Accelbuffer.Injection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Accelbuffer.Reflection
{
    /// <summary>
    /// 表示一个结构类型
    /// </summary>
    public sealed class AccelTypeInfo : IAccelMemberInfo, IEquatable<AccelTypeInfo>
    {
        private readonly IReadOnlyDictionary<string, AccelFieldInfo> m_Fields;
        private RuntimeMethodHandle? m_SerializeCallbackMethod;
        private RuntimeMethodHandle? m_DeserializeCallbackMethod;

        internal DynamicSerializeFunction SerializeFunction { get; set; }//序列化方法

        internal DynamicDeserializeFunction DeserializeFunction { get; set; }//反序列化方法

        /// <summary>
        /// 获取内部元数据类型的表示形式的句柄
        /// </summary>
        public RuntimeTypeHandle TypeHandle { get; internal set; }

        /// <summary>
        /// 获取类型的元数据类型
        /// </summary>
        public Type Info => Type.GetTypeFromHandle(TypeHandle);

        /// <summary>
        /// 获取类型的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取类型的全名
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// 获取类型的文档
        /// </summary>
        public string Document { get; }

        /// <summary>
        /// 获取类型的可访问性
        /// </summary>
        public TypeVisibility Visibility { get; }

        /// <summary>
        /// 获取类型是否是密封类型
        /// </summary>
        public bool IsFinal { get; }

        /// <summary>
        /// 获取类型是否是引用类型
        /// </summary>
        public bool IsClass { get; }

        /// <summary>
        /// 获取类型是否是值类型
        /// </summary>
        public bool IsValueType => !IsClass;

        /// <summary>
        /// 获取类型是否是嵌套类型
        /// </summary>
        public bool IsNested { get; }

        /// <summary>
        /// 获取类型中的字段的序列索引是否连续
        /// </summary>
        public bool HasContinuousSerialIndexes { get; }

        /// <summary>
        /// 获取类型的实例将会占用的近似字节大小
        /// </summary>
        public int ApproximateMemorySize { get; }

        /// <summary>
        /// 获取声明此类型的类型
        /// </summary>
        public AccelTypeInfo DeclaringType { get; }

        /// <summary>
        /// 获取类型声明的所有字段
        /// </summary>
        public IEnumerable<AccelFieldInfo> Fields => m_Fields.Values;

        /// <summary>
        /// 获取类型声明的序列化前调用的方法
        /// </summary>
        public MethodInfo OnBeforeSerializeMethod
        {
            get => m_SerializeCallbackMethod.HasValue ? MethodBase.GetMethodFromHandle(m_SerializeCallbackMethod.Value) as MethodInfo : null;
            internal set => m_SerializeCallbackMethod = value?.MethodHandle;
        }

        /// <summary>
        /// 获取类型声明的反序列化后调用的方法
        /// </summary>
        public MethodInfo OnAfterDeserializeMethod
        {
            get => m_DeserializeCallbackMethod.HasValue ? MethodBase.GetMethodFromHandle(m_DeserializeCallbackMethod.Value) as MethodInfo : null;
            internal set => m_DeserializeCallbackMethod = value?.MethodHandle;
        }

        internal AccelTypeInfo(string name,
                               string fullName,
                               string doc,
                               TypeVisibility visibility,
                               bool isFinal,
                               bool isRef,
                               bool isNested,
                               bool hasContinuousSerialIndexes,
                               int size,
                               AccelTypeInfo declaringType,
                               IReadOnlyDictionary<string, AccelFieldInfo> fields)
        {
            Name = name;
            FullName = fullName;
            Document = doc;
            Visibility = visibility;
            IsFinal = isFinal;
            IsClass = isRef;
            IsNested = isNested;
            HasContinuousSerialIndexes = hasContinuousSerialIndexes;
            ApproximateMemorySize = size;
            DeclaringType = declaringType;
            m_Fields = fields;
            m_SerializeCallbackMethod = null;
            m_DeserializeCallbackMethod = null;
            s_Types.Add(fullName, this);
        }

        /// <summary>
        /// 返回<see cref="FullName"/>
        /// </summary>
        /// <returns><see cref="FullName"/></returns>
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return TypeHandle.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is AccelTypeInfo info && info.TypeHandle.Equals(TypeHandle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AccelTypeInfo other)
        {
            return other != null && other.TypeHandle.Equals(TypeHandle);
        }

        /// <summary>
        /// 创建类型的一个实例
        /// </summary>
        /// <returns>该类型的实例对象</returns>
        public object CreateInstance()
        {
            return Activator.CreateInstance(Info);
        }

        /// <summary>
        /// 获取类型声明的所有字段
        /// </summary>
        /// <returns>类型声明的所有字段的枚举对象</returns>
        public IEnumerable<AccelFieldInfo> GetFields()
        {
            return m_Fields.Values;
        }

        /// <summary>
        /// 获取类型中指定名称的字段
        /// </summary>
        /// <param name="name">需要获取的字段的名称</param>
        /// <returns>类型中具有指定名称的字段，如果字段不存在，则返回null</returns>
        public AccelFieldInfo GetField(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return m_Fields.TryGetValue(name, out AccelFieldInfo info) ? info : null;
        }


        private static readonly Dictionary<string, AccelTypeInfo> s_Types = new Dictionary<string, AccelTypeInfo>();
        private static readonly BindingFlags s_FieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags s_MessageCallbackBindingFlags = BindingFlags.Public | BindingFlags.Instance;
        private static DataComparer s_Comparer = null;

        /// <summary>
        /// 获取加载的所有类型元数据
        /// </summary>
        /// <returns>类型元数据枚举</returns>
        public static IEnumerable<AccelTypeInfo> GetTypes()
        {
            return s_Types.Values;
        }

        /// <summary>
        /// 获取具有给定名称的类型元数据
        /// </summary>
        /// <param name="name">需要查询的类型名称（FullName）</param>
        /// <returns>具有给定名称的类型元数据</returns>
        public static AccelTypeInfo GetType(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return s_Types.TryGetValue(name, out AccelTypeInfo info) ? info : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fields"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <param name="fieldCount"></param>
        /// <param name="hasContinuousSerialIndexes"></param>
        public static void GetTypeInfo(Type type, out IEnumerable<AccelFieldInfo> fields, out MethodInfo before, out MethodInfo after, out int fieldCount, out bool hasContinuousSerialIndexes)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (s_Types.TryGetValue(type.FullName, out AccelTypeInfo result))
            {
                GetTypeInfo(result, out fields, out before, out after, out fieldCount, out hasContinuousSerialIndexes);
                return;
            }

            List<AccelFieldInfo> fieldInfos = GetSerializedFields(type, null, out hasContinuousSerialIndexes);
            fields = fieldInfos;
            fieldCount = fieldInfos.Count;
            GetMessageCallbacks(type, out before, out after);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fields"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <param name="fieldCount"></param>
        /// <param name="hasContinuousSerialIndexes"></param>
        public static void GetTypeInfo(AccelTypeInfo type, out IEnumerable<AccelFieldInfo> fields, out MethodInfo before, out MethodInfo after, out int fieldCount, out bool hasContinuousSerialIndexes)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            fields = type.Fields;
            before = type.OnBeforeSerializeMethod;
            after = type.OnAfterDeserializeMethod;
            fieldCount = type.m_Fields.Count;
            hasContinuousSerialIndexes = type.HasContinuousSerialIndexes;
        }

        private static List<AccelFieldInfo> GetSerializedFields(Type objType, AccelTypeInfo declaringType, out bool isContinuous)
        {
            FieldInfo[] allFields = objType.GetFields(s_FieldBindingFlags);

            if (allFields == null)
            {
                isContinuous = false;
                return null;
            }

            List<AccelFieldInfo> list = new List<AccelFieldInfo>(allFields.Length);
            
            for (int i = 0; i < allFields.Length; i++)
            {
                FieldInfo field = allFields[i];
                
                if (field.IsInitOnly)
                {
                    continue;
                }

                SerialIndexAttribute attribute = field.GetCustomAttribute<SerialIndexAttribute>(true);

                if (attribute == null)
                {
                    continue;
                }

                NeverNullAttribute isNeverNull = field.GetCustomAttribute<NeverNullAttribute>(true);
                FacadeTypeAttribute facadeType = field.GetCustomAttribute<FacadeTypeAttribute>(true);
                
                AccelFieldInfo fieldInfo = new AccelFieldInfo(field.Name, null, attribute.Index, false, isNeverNull != null, declaringType, field.FieldType, facadeType?.RealType)
                {
                    FieldHandle = field.FieldHandle
                };

                list.Add(fieldInfo);
            }

            list.Sort(s_Comparer ?? (s_Comparer = new DataComparer()));
            isContinuous = IsFieldIndexContinuous(list);
            return list;
        }

        private static void GetMessageCallbacks(Type objType, out MethodInfo beforeMethod, out MethodInfo afterMethod)
        {
            MethodInfo[] methods = objType.GetMethods(s_MessageCallbackBindingFlags);
            beforeMethod = null;
            afterMethod = null;

            if (methods == null)
            {
                return;
            }

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                ParameterInfo[] parameters = method.GetParameters();

                if (parameters != null && parameters.Length != 0)
                {
                    continue;
                }

                OnBeforeSerializationAttribute attr1 = method.GetCustomAttribute<OnBeforeSerializationAttribute>(true);
                OnAfterDeserializationAttribute attr2 = method.GetCustomAttribute<OnAfterDeserializationAttribute>(true);

                if (attr1 != null)
                {
                    beforeMethod = method;
                }

                if (attr2 != null)
                {
                    afterMethod = method;
                }

                if (beforeMethod != null && afterMethod != null)
                {
                    return;
                }
            }
        }

        private static bool IsFieldIndexContinuous(List<AccelFieldInfo> fields)
        {
            if (fields.Count < 2)
            {
                return true;
            }

            int last = fields[0].SerialIndex;

            for (int i = 1; i < fields.Count; i++)
            {
                if (fields[i].SerialIndex - last != 1)
                {
                    return false;
                }

                last = fields[i].SerialIndex;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(AccelTypeInfo left, AccelTypeInfo right)
        {
            RuntimeTypeHandle h1 = left is null ? default : left.TypeHandle;
            RuntimeTypeHandle h2 = right is null ? default : right.TypeHandle;
            return h1.Equals(h2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(AccelTypeInfo left, AccelTypeInfo right)
        {
            RuntimeTypeHandle h1 = left is null ? default : left.TypeHandle;
            RuntimeTypeHandle h2 = right is null ? default : right.TypeHandle;
            return !h1.Equals(h2);
        }

        private sealed class DataComparer : IComparer<AccelFieldInfo>
        {
            public int Compare(AccelFieldInfo x, AccelFieldInfo y)
            {
                return x.SerialIndex - y.SerialIndex;
            }
        }
    }
}
