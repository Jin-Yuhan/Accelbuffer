namespace Accelbuffer
{
    internal sealed class PrimitiveTypeSerializeProxy :
        ISerializeProxy<sbyte>,
        ISerializeProxy<byte>,
        ISerializeProxy<short>,
        ISerializeProxy<ushort>,
        ISerializeProxy<int>,
        ISerializeProxy<uint>,
        ISerializeProxy<long>,
        ISerializeProxy<ulong>,
        ISerializeProxy<float>,
        ISerializeProxy<double>,
        ISerializeProxy<bool>,
        ISerializeProxy<char>,
        ISerializeProxy<string>
    {
        unsafe sbyte ISerializeProxy<sbyte>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedInt8(0);
        }

        unsafe byte ISerializeProxy<byte>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedUInt8(0);
        }

        unsafe short ISerializeProxy<short>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedInt16(0);
        }

        unsafe ushort ISerializeProxy<ushort>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedUInt16(0);
        }

        unsafe uint ISerializeProxy<uint>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedUInt32(0);
        }

        unsafe int ISerializeProxy<int>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedInt32(0);
        }

        unsafe ulong ISerializeProxy<ulong>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedUInt64(0);
        }

        unsafe double ISerializeProxy<double>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedFloat64(0);
        }

        unsafe long ISerializeProxy<long>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedInt64(0);
        }

        unsafe float ISerializeProxy<float>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadFixedFloat32(0);
        }

        unsafe bool ISerializeProxy<bool>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadBoolean(0);
        }

        unsafe char ISerializeProxy<char>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadChar(0, SerializeProxyInjector.DefaultCharEncoding);
        }

        unsafe string ISerializeProxy<string>.Deserialize(in UnmanagedReader* reader)
        {
            return reader->ReadString(0, SerializeProxyInjector.DefaultCharEncoding);
        }

        unsafe void ISerializeProxy<sbyte>.Serialize(in sbyte obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<byte>.Serialize(in byte obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<short>.Serialize(in short obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<ushort>.Serialize(in ushort obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<uint>.Serialize(in uint obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<int>.Serialize(in int obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<ulong>.Serialize(in ulong obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<double>.Serialize(in double obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<long>.Serialize(in long obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<float>.Serialize(in float obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultNumberType);
        }

        unsafe void ISerializeProxy<bool>.Serialize(in bool obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj);
        }

        unsafe void ISerializeProxy<char>.Serialize(in char obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultCharEncoding);
        }

        unsafe void ISerializeProxy<string>.Serialize(in string obj, in UnmanagedWriter* writer)
        {
            writer->WriteValue(0, obj, SerializeProxyInjector.DefaultCharEncoding);
        }
    }
}
