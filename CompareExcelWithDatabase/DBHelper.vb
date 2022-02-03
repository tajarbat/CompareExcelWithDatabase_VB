Imports System.Data
Imports System.Data.SqlClient
Public Class DbHelper

    Public Function GetDataTable(ByVal connectionString As String, query As String) As DataTable
        Dim dTable As New DataTable
        Using cnn As New SqlConnection(connectionString)
            cnn.Open()
            Using dAdapter As New SqlDataAdapter(query, cnn)
                dAdapter.Fill(dTable)
            End Using
            cnn.Close()
        End Using

        Return dTable
    End Function

    Public Sub UpsertTable()
        ''implement code here
    End Sub
End Class
