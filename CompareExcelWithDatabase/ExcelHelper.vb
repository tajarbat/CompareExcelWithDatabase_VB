Imports System.Data
Public Class ExcelHelper
    Public Function GetDataTable(ByVal ConnectionString As String, query As String) As DataTable


        Dim dTable As New DataTable
        Using cnn As New OleDb.OleDbConnection(ConnectionString)
            cnn.Open()
            Using dAdapter As New OleDb.OleDbDataAdapter(query, cnn)
                dAdapter.Fill(dTable)
            End Using
            cnn.Close()
        End Using

        Return dTable
    End Function
End Class