Imports Accelbuffer.Injection
Imports Accelbuffer.Memory

<MemorySize(160)>
Partial Public Structure TestClass

    <FieldIndex(1)>
    Public Field1 As Boolean

    <FieldIndex(2)>
    Public Field2 As Byte

    <FieldIndex(3)>
    Public Field3 As SByte

    <FieldIndex(4)>
    Public Field4 As UShort

    <FieldIndex(5)>
    Public Field5 As Short

    <FieldIndex(6)>
    Public Field6 As UInteger

    <FieldIndex(7)>
    Public Field7 As Integer

    <FieldIndex(8)>
    Public Field8 As ULong

    <FieldIndex(9)>
    Public Field9 As Long

    <FieldIndex(10)>
    Public Field10 As Single

    <FieldIndex(11)>
    Public Field11 As Double

    <FieldIndex(12)>
    Public Field12 As Decimal

    <FieldIndex(13)>
    Public Field13 As Char

    <FieldIndex(14)>
    Public Field14 As String

    <FieldIndex(15)>
    Public Field15 As IntPtr

    <FieldIndex(16)>
    Public Field16 As UIntPtr

    <FieldIndex(17)>
    Public Field17 As vint

    <FieldIndex(18)>
    Public Field18 As vuint
End Structure


Module Module1

    Sub Main()
        Dim instance As TestClass = New TestClass With {
            .Field1 = True,
            .Field2 = 24,
            .Field3 = 24,
            .Field4 = 242,
            .Field5 = -2242,
            .Field6 = 24242,
            .Field7 = -275242,
            .Field8 = 21578278,
            .Field9 = -272278278,
            .Field10 = 27827278287,
            .Field11 = -287287278287,
            .Field12 = -28727827278287,
            .Field13 = "Char",
            .Field14 = "Hello Visual Basic.NET!",
            .Field15 = 287278288,
            .Field16 = 278272728,
            .Field17 = -287278272727278,
            .Field18 = 27827827827523876
        }

        Dim buffer As Byte() = Nothing

        Serializer.Serialize(Of TestClass)(instance, buffer)

        instance = Serializer.Deserialize(Of TestClass)(buffer, 0, buffer.Length)

        Console.WriteLine(instance.Field15 = 287278288) 'not supported
        Console.WriteLine(instance.Field16 = 278272728) 'not supported
        Console.WriteLine("Finish")

        Dim nbuffer As NativeBuffer = Serializer.Serialize(Of Accelbuffer.Test.FieldTypeTest)(Nothing)
        Serializer.Deserialize(Of Accelbuffer.Test.FieldTypeTest)(nbuffer, 0, nbuffer.Length)

        MemoryAllocator.Shared.Trim(True)

        Console.ReadKey()

    End Sub

End Module
