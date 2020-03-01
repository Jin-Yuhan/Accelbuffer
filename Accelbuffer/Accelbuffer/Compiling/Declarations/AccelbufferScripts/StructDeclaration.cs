//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 扩展类型请使用partial关键字在其他文件扩展，否则更改可能会丢失。
namespace Accelbuffer.Compiling.Declarations
{
    using Accelbuffer;
    using Accelbuffer.Injection;
    using Accelbuffer.Memory;
    using System;
    using System.Collections.Generic;
    using boolean = System.Boolean;
    using int8 = System.SByte;
    using uint8 = System.Byte;
    using int16 = System.Int16;
    using uint16 = System.UInt16;
    using int32 = System.Int32;
    using uint32 = System.UInt32;
    using int64 = System.Int64;
    using uint64 = System.UInt64;
    using float32 = System.Single;
    using float64 = System.Double;
    using float128 = System.Decimal;
    using nint = System.IntPtr;
    using nuint = System.UIntPtr;
    using vint = Accelbuffer.VInt;
    using vuint = Accelbuffer.VUInt;
    using Accelbuffer.Reflection;
    
    
    /// <summary>
    /// 结构声明
    /// </summary>
    [MemorySize(560)]
    [SerializeBy(typeof(StructDeclaration.StructDeclarationSerializer))]
    public sealed partial class StructDeclaration
    {
        
        [NeverNull()]
        [SerialIndex(1)]
        [FacadeType(typeof(int32))]
        private TypeVisibility m_Visibility;
        
        [NeverNull()]
        [SerialIndex(2)]
        private boolean m_IsFinal;
        
        [NeverNull()]
        [SerialIndex(3)]
        private boolean m_IsRef;
        
        [NeverNull()]
        [SerialIndex(4)]
        private boolean m_IsNested;
        
        [NeverNull()]
        [SerialIndex(5)]
        private boolean m_IsFieldIndexContinuous;
        
        [SerialIndex(6)]
        private string m_Name;
        
        [SerialIndex(7)]
        private string m_Doc;
        
        [NeverNull()]
        [SerialIndex(8)]
        private vint m_Size;
        
        [NeverNull()]
        [SerialIndex(9)]
        private DeclarationArray m_Declarations;
        
partial void OnBeforeSerialization();
partial void OnAfterDeserialization();
        
        /// <summary>
        /// 可访问性
        /// </summary>
        public TypeVisibility Visibility
        {
            get
            {
                return this.m_Visibility;
            }
            set
            {
                this.m_Visibility = value;
            }
        }
        
        /// <summary>
        /// 是否为密封结构
        /// </summary>
        public boolean IsFinal
        {
            get
            {
                return this.m_IsFinal;
            }
            set
            {
                this.m_IsFinal = value;
            }
        }
        
        /// <summary>
        /// 是否为引用结构
        /// </summary>
        public boolean IsRef
        {
            get
            {
                return this.m_IsRef;
            }
            set
            {
                this.m_IsRef = value;
            }
        }
        
        /// <summary>
        /// 是否为嵌套结构
        /// </summary>
        public boolean IsNested
        {
            get
            {
                return this.m_IsNested;
            }
            set
            {
                this.m_IsNested = value;
            }
        }
        
        /// <summary>
        /// 所有字段的索引是否连续
        /// </summary>
        public boolean IsFieldIndexContinuous
        {
            get
            {
                return this.m_IsFieldIndexContinuous;
            }
            set
            {
                this.m_IsFieldIndexContinuous = value;
            }
        }
        
        /// <summary>
        /// 结构的名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        
        /// <summary>
        /// 结构的文档
        /// </summary>
        public string Doc
        {
            get
            {
                return this.m_Doc;
            }
            set
            {
                this.m_Doc = value;
            }
        }
        
        /// <summary>
        /// 结构的大小
        /// </summary>
        public vint Size
        {
            get
            {
                return this.m_Size;
            }
            set
            {
                this.m_Size = value;
            }
        }
        
        /// <summary>
        /// 结构内的声明
        /// </summary>
        public DeclarationArray Declarations
        {
            get
            {
                return this.m_Declarations;
            }
            set
            {
                this.m_Declarations = value;
            }
        }
        
        /// <summary>
        /// 序列化对象
        /// </summary>
        public byte[] ToBytes()
        {
            byte[] result;
            Serializer.Serialize(this, out result);
            return result;
        }
        
        /// <summary>
        /// 反序列化对象
        /// </summary>
        public static StructDeclaration FromBytes(byte[] bytes)
        {
return Serializer.Deserialize<StructDeclaration>(bytes, 0, bytes.Length);
        }
        
        /// <summary>
        /// 对象序列化代理（自动生成）
        /// </summary>
        public sealed class StructDeclarationSerializer : object, ITypeSerializer<StructDeclaration>
        {
            
            /// <summary>
            /// 序列化方法（自动生成）
            /// </summary>
            public void Serialize(StructDeclaration obj, ref AccelWriter writer)
            {
                obj.OnBeforeSerialization();
                writer.WriteValue(1, ((int32)(obj.m_Visibility)));
                writer.WriteValue(2, obj.m_IsFinal);
                writer.WriteValue(3, obj.m_IsRef);
                writer.WriteValue(4, obj.m_IsNested);
                writer.WriteValue(5, obj.m_IsFieldIndexContinuous);
                if ((obj.m_Name != null))
                {
                    writer.WriteValue(6, obj.m_Name);
                }
                if ((obj.m_Doc != null))
                {
                    writer.WriteValue(7, obj.m_Doc);
                }
                writer.WriteValue(8, obj.m_Size);
                writer.WriteValue(9, obj.m_Declarations);
            }
            
            /// <summary>
            /// 反序列化方法（自动生成）
            /// </summary>
            public StructDeclaration Deserialize(ref AccelReader reader)
            {
                StructDeclaration result = new StructDeclaration();
                int index;
			while (reader.HasNext(out index))
			{
				switch (index)
				{
					case 1:
						result.m_Visibility = (TypeVisibility)reader.ReadInt32();
						break;
					case 2:
						result.m_IsFinal = reader.ReadBoolean();
						break;
					case 3:
						result.m_IsRef = reader.ReadBoolean();
						break;
					case 4:
						result.m_IsNested = reader.ReadBoolean();
						break;
					case 5:
						result.m_IsFieldIndexContinuous = reader.ReadBoolean();
						break;
					case 6:
						result.m_Name = reader.ReadString();
						break;
					case 7:
						result.m_Doc = reader.ReadString();
						break;
					case 8:
						result.m_Size = reader.ReadVariantInt();
						break;
					case 9:
						result.m_Declarations = reader.ReadGeneric<DeclarationArray>();
						break;
					default:
						reader.SkipNext();
						break;
				}
			}

                result.OnAfterDeserialization();
                return result;
            }
        }
    }
}
