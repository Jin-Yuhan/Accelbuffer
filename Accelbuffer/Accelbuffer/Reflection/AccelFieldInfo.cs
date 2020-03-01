using System;
using System.Reflection;

namespace Accelbuffer.Reflection
{
    /// <summary>
    /// 表示一个结构中的字段。
    /// </summary>
    public sealed class AccelFieldInfo : IAccelMemberInfo, IEquatable<AccelFieldInfo>
    {
        private readonly RuntimeTypeHandle m_FieldTypeHandle;
        private readonly RuntimeTypeHandle? m_RealTypeHandle;

        /// <summary>
        /// 获取内部元数据字段的表示形式的句柄
        /// </summary>
        public RuntimeFieldHandle FieldHandle { get; internal set; }

        /// <summary>
        /// 获取字段的元数据类型
        /// </summary>
        public FieldInfo Info => FieldInfo.GetFieldFromHandle(FieldHandle);

        /// <summary>
        /// 获取字段的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取字段的文档
        /// </summary>
        public string Document { get; }

        /// <summary>
        /// 获取字段的序列索引
        /// </summary>
        public int SerialIndex { get; }

        /// <summary>
        /// 获取字段是否已经被弃用
        /// </summary>
        public bool IsObsolete { get; }

        /// <summary>
        /// 获取字段的值是否永远不会为null
        /// </summary>
        public bool IsNeverNull { get; }

        /// <summary>
        /// 获取字段是否使用外观类型
        /// </summary>
        public bool IsFacadeType => m_RealTypeHandle.HasValue;

        /// <summary>
        /// 获取声明此字段的类型
        /// </summary>
        public AccelTypeInfo DeclaringType { get; }

        /// <summary>
        /// 获取字段的类型
        /// </summary>
        public Type FieldType => Type.GetTypeFromHandle(m_FieldTypeHandle);

        /// <summary>
        /// 获取字段的真实类型，如果字段没有使用外观类型，将返回null
        /// </summary>
        public Type RealFieldType => m_RealTypeHandle.HasValue ? Type.GetTypeFromHandle(m_RealTypeHandle.Value) : null;

        internal AccelFieldInfo(string name,
                                string doc,
                                int index,
                                bool isObsolete,
                                bool isNeverNull,
                                AccelTypeInfo declaringType,
                                Type fieldType,
                                Type realType)
        {
            Name = name;
            Document = doc;
            SerialIndex = index;
            IsObsolete = isObsolete;
            IsNeverNull = isNeverNull;
            DeclaringType = declaringType;
            m_FieldTypeHandle = fieldType.TypeHandle;
            m_RealTypeHandle = realType?.TypeHandle;
        }

        /// <summary>
        /// 返回<see cref="Name"/>
        /// </summary>
        /// <returns><see cref="Name"/></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FieldHandle.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is AccelFieldInfo info && info.FieldHandle.Equals(FieldHandle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AccelFieldInfo other)
        {
            return other != null && other.FieldHandle.Equals(FieldHandle);
        }

        /// <summary>
        /// 返回给定对象的字段的值
        /// </summary>
        /// <param name="obj">获取字段值的对象引用</param>
        /// <returns>给定对象的字段的值</returns>
        public object GetValue(object obj)
        {
            return Info.GetValue(obj);
        }

        /// <summary>
        /// 设置给定对象的字段的值。
        /// </summary>
        /// <param name="obj">设置字段值的对象引用</param>
        /// <param name="value">要分配给字段的值</param>
        public void SetValue(object obj, object value)
        {
            Info.SetValue(obj, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(AccelFieldInfo left, AccelFieldInfo right)
        {
            RuntimeFieldHandle h1 = left is null ? default : left.FieldHandle;
            RuntimeFieldHandle h2 = right is null ? default : right.FieldHandle;
            return !h1.Equals(h2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(AccelFieldInfo left, AccelFieldInfo right)
        {
            RuntimeFieldHandle h1 = left is null ? default : left.FieldHandle;
            RuntimeFieldHandle h2 = right is null ? default : right.FieldHandle;
            return !h1.Equals(h2);
        }
    }
}
