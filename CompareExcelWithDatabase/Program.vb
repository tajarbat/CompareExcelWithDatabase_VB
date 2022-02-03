Imports System
Imports System.Data
Imports System.Linq

Module Program
    Sub Main(args As String())
        Console.WriteLine("Excel Sheet Comparison with Database Table")


        Dim exTable As DataTable = GetExcelTable()
        Dim dbTable As DataTable = GetDbTable()

        Dim upsertableRecords As DataTable = CompareDataTableRows(dbTable, exTable)

        ''Perform your Upsert Operation on upsertableRecords 

        Console.ReadKey()

    End Sub

    Function GetDbTable() As DataTable

        Dim conString As String = "ADD YOUR CONNECTION STRING HERE"

        Dim query As String = "select id, fname, lname from dbo.CompareMe"

        Dim db As New DbHelper 'SQL Server Helper

        GetDbTable = db.GetDataTable(conString, query)

    End Function

    Function GetExcelTable() As DataTable

        Dim conString As String = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source='.\ExcelData.xlsx'; Extended Properties=Excel 8.0;"

        Dim query As String = "select id, fname, lname from [DemoTable1$]"

        Dim excel As New ExcelHelper

        GetExcelTable = excel.GetDataTable(conString, query)

    End Function

    Function CompareDataTableRows(ByVal dTable1 As DataTable, ByVal dTable2 As DataTable) As DataTable


        ''Picking mismatched values based on ID column from both tables
        ''Here you need to decide the base table i.e. which table's data to pick as modified information
        Dim R1 = (From dt1 In dTable1
                  Join dt2 In dTable2 On dt1.Field(Of Integer)("id") Equals dt2.Field(Of Double)("id")
                  Where dt1.Field(Of String)("fname") <> dt2.Field(Of String)("fname") OrElse
                      dt1.Field(Of String)("lname") <> dt2.Field(Of String)("lname")
                  Select dt1)

        ''Picking addtional IDs from table1
        Dim R2 = (From dt1 In dTable1
                  Let newIds = (From dt2 In dTable2 Select dt2.Field(Of Double)("id"))
                  Where newIds.Contains(CDbl(dt1.Field(Of Integer)("id"))) <> True
                  Select dt1)

        ''Picking addtional IDs from table2
        Dim R3 = (From dt2 In dTable2
                  Let newIds = (From dt1 In dTable1 Select dt1.Field(Of Integer)("id"))
                  Where newIds.Contains(CInt(dt2.Field(Of Double)("id"))) <> True
                  Select dt2)

        ''Merge all Rs

        Dim upsertableDataTable As DataTable
        upsertableDataTable = dTable1.Clone()

        For Each rowIterator As DataRow In R1.Union(R2).Union(R3)
            'Dim dRow As DataRow = upsertableDataTable.NewRow()
            'dRow = rowIterator

            upsertableDataTable.ImportRow(rowIterator)
            'upsertableDataTable.Rows.Add(dRow)

            'Let's print the required data
            Console.WriteLine(String.Join(",", rowIterator.ItemArray))
        Next



        Return upsertableDataTable

    End Function


End Module
