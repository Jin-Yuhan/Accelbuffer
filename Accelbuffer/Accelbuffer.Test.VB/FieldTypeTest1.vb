Namespace Accelbuffer.Test
    Partial Structure FieldTypeTest

        Private Sub OnBeforeSerialization()
            System.Console.WriteLine("OnBeforeSerialization")
        End Sub

        Private Sub OnAfterDeserialization()
            System.Console.WriteLine("OnAfterDeserialization")
        End Sub

    End Structure
End Namespace