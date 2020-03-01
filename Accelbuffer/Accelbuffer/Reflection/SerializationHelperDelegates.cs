namespace Accelbuffer.Reflection
{
    internal delegate byte[] DynamicSerializeFunction(object obj, Encoding encoding, Endian endian);
    internal delegate object DynamicDeserializeFunction(byte[] bytes, int index, int length);
}
