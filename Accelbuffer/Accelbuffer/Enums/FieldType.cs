using Accelbuffer.Text;

namespace Accelbuffer
{
    /// <summary>
    /// 表示一个字段的类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>,
        /// <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/>
        /// </summary>
        VariantInt,

        /// <summary>
        /// <see cref="sbyte"/>
        /// </summary>
        Int8,

        /// <summary>
        /// <see cref="byte"/>
        /// </summary>
        UInt8,

        /// <summary>
        /// <see cref="short"/>
        /// </summary>
        Int16,

        /// <summary>
        /// <see cref="ushort"/>
        /// </summary>
        UInt16,

        /// <summary>
        /// <see cref="int"/>
        /// </summary>
        Int32,

        /// <summary>
        /// <see cref="uint"/>
        /// </summary>
        UInt32,

        /// <summary>
        /// <see cref="long"/>
        /// </summary>
        Int64,

        /// <summary>
        /// <see cref="ulong"/>
        /// </summary>
        UInt64,

        /// <summary>
        /// <see cref="float"/>
        /// </summary>
        Float32,

        /// <summary>
        /// <see cref="double"/>
        /// </summary>
        Float64,

        /// <summary>
        /// <see cref="decimal"/>
        /// </summary>
        Float128,

        /// <summary>
        /// <see cref="bool"/>
        /// </summary>
        Boolean,

        /// <summary>
        /// <see cref="Encoding.ASCII"/> <see cref="char"/>
        /// </summary>
        ASCIIChar,

        /// <summary>
        /// <see cref="Encoding.Unicode"/> <see cref="char"/>
        /// </summary>
        UnicodeChar,

        /// <summary>
        /// <see cref="Encoding.UTF8"/> <see cref="char"/>
        /// </summary>
        UTF8Char,

        /// <summary>
        /// <see cref="Encoding.ASCII"/> <see cref="string"/>
        /// </summary>
        ASCIIString,

        /// <summary>
        /// <see cref="Encoding.Unicode"/> <see cref="string"/>
        /// </summary>
        UnicodeString,

        /// <summary>
        /// <see cref="Encoding.UTF8"/> <see cref="string"/>
        /// </summary>
        UTF8String,

        /// <summary>
        /// 自定义类型，集合，结构...
        /// </summary>
        Complex
    }
}
