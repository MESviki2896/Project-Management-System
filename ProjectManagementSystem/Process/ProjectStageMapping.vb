Imports System.Globalization
Imports ProjectManagementSystem.FucntionModules
Public Class ProjectStageMapping
    Dim dt2 As DataTable = New DataTable()
    Dim dt1 As DataTable = New DataTable()
    Dim dv As DataView
    Dim filt As String
    Dim filtnew As String = ""

    Private Sub ProjectStageMapping_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        FucntionModules.FillComboBox(ComboBox1, QueryModule.GetProject(), True)
        FucntionModules.FillComboBox(ComboBox2, QueryModule.GetUsers(True), True)

        GRID1.ReadOnly = True
        TextBox1.Text = PMSMain.Label3.Text
        TextBox1.ReadOnly = True
        'GRID1 settings
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.RowHeadersVisible = False
        GRID1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        GRID2.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        For Each column As DataGridViewColumn In GRID1.Columns
            GRID1.Columns.Item(column.Index).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        Next
        GRID2.AllowUserToAddRows = False
        GRID2.RowHeadersVisible = False
        GRID2.AllowUserToDeleteRows = False
        GRID2.AllowUserToOrderColumns = False
        GRID2.Columns.Item(1).ReadOnly = True
        grid1fill()
        GRID1.Columns.Item(0).Frozen = True
        GRID1.Columns.Item(1).Frozen = True
        GRID1.ClearSelection()
    End Sub
    Private Sub grid1fill()
        Dim result As String = ""
        dt1 = QueryModule.GetStageProcessDetails("Stages", True, result)
        If result.Contains("~") Then
            MsgBox(result.Substring(1, result.Length - 1))
            Exit Sub
        End If
        If dt1 Is Nothing Then
            MsgBox("No data exist in datatable.")
            Exit Sub
        End If
        Dim bs As BindingSource = New BindingSource()
        dv = New DataView(dt1)
        bs.DataSource = dv
        GRID1.DataSource = bs
        GRID1.ClearSelection()
        dt2.Columns.AddRange({New DataColumn("StageID", Type.GetType("System.Int32")),
                       New DataColumn("StageName", Type.GetType("System.String")),
                       New DataColumn("ProjID", Type.GetType("System.Int32")),
                       New DataColumn("ProjName", Type.GetType("System.String")),
                       New DataColumn("Remarks", Type.GetType("System.String")),
                       New DataColumn("StartDate", Type.GetType("System.DateTime")),
                       New DataColumn("EndDate", Type.GetType("System.DateTime")),
                       New DataColumn("CreatedOn", Type.GetType("System.DateTime")),
                       New DataColumn("CreatedBy", Type.GetType("System.String")),
                       New DataColumn("AssignedTo", Type.GetType("System.String")),
                        New DataColumn("Sequence", Type.GetType("System.Int32"))})
    End Sub
    'Whenever user clicks doube times on the cell content the action takes place
    Private Sub GRID1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID1.CellContentDoubleClick
        If e.RowIndex >= 0 Then
            If ComboBox1.SelectedIndex > 0 Then
                If GRID1.Rows.Count > 0 Then
                    If GRID1.CurrentRow.Index >= 0 Then
                        Dim val = GRID2.Rows.Add()
                        GRID2(0, val).Value = val + 1
                        GRID2(1, val).Value = GRID1.CurrentRow.Cells.Item(0).Value.ToString()
                        GRID2(2, val).Value = GRID1.CurrentRow.Cells.Item(1).Value.ToString()
                        Dim datee As DateTime
                        If GRID2(0, val).Value = 1 Then
                            datee = ComboBox1.SelectedItem.row(2)
                            GRID2(3, val).Value = datee.ToString("yyyy-MM-dd")
                        Else
                            GRID2(3, val).Value = If(GRID2(4, val - 1).Value Is Nothing, "", FucntionModules.GetPerfectEndDates(GRID2(4, val - 1).Value, 1).ToString("yyyy-MM-dd"))

                        End If
                        GRID2(4, val).Value = FucntionModules.GetPerfectEndDates(GRID2(3, val).Value, (GRID1.CurrentRow.Cells.Item(3).Value.ToString())).ToString("yyyy-MM-dd")
                        filtnew = filtadd(GRID1.CurrentRow.Cells.Item(0).Value.ToString, filtnew)
                            dv.RowFilter = filtnew
                            dv.RowStateFilter = DataViewRowState.CurrentRows
                            ' Dim vasl = bs.SupportsFiltering
                        End If
                    Else
                    MsgBox("Can't Add more Rows, All stages moved")
                End If
            Else
                MsgBox("Please select the project before selecting the rows")
            End If
            GRID2.ClearSelection()
            GRID1.ClearSelection()
        End If
    End Sub
    Private Sub GRID2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID2.CellContentDoubleClick
        If e.RowIndex >= 0 Then
            If ComboBox1.SelectedIndex > 0 Then
                If GRID2.Rows.Count > 0 Then
                    If GRID2.CurrentRow.Index >= 0 Then
                        filtnew = filteremove(GRID2.CurrentRow.Cells.Item(1).Value, filtnew)
                        dv.RowFilter = filtnew
                        dv.RowStateFilter = DataViewRowState.CurrentRows
                        GRID2.Rows.RemoveAt(GRID2.CurrentRow.Index)
                        'If dt2.Rows.Count > 0 Then
                        '    Dim qry = From dr As DataRow In dt2 Where dr.Field(Of Int32)("StageID") = GRID2.CurrentRow.Cells.Item(1).Value Select dr
                        '    Dim drs = qry(0)
                        '    dt2.Rows.RemoveAt(dt2.Rows.IndexOf(drs))
                        'End If

                        For i = 0 To GRID2.Rows.Count - 1
                            GRID2(0, i).Value = (i + 1).ToString
                        Next
                    End If
                Else
                    MsgBox("Can't remove more records, This was the last!!")
                End If
            Else
                MsgBox("Please select Project!")
            End If
            GRID2.ClearSelection()
            GRID1.ClearSelection()
        End If
    End Sub

    'Functions to add and Remove Filters for the first grid1 as per wish
    '0  StageID
    '1  StageName
    '2  ProjID
    '3  ProjName
    '4  Remarks
    '5  StartDate
    '6  EndDate
    '7  CreatedOn
    '8  CreatedBy
    '9 AssignedTo
    '10  Sequence

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click                 '---Search Click
        Dim dt As New DataTable
        Try
            Cursor = Cursors.WaitCursor
            dt = QueryModule.GetStageMapping(ComboBox1.SelectedItem.row.item(0).ToString)
            Cursor = Cursors.Default
        Catch ex As Exception
            Threading.Thread.Sleep(20)
        End Try

        If dt Is Nothing Then
            GRID2.Rows.Clear()
            MsgBox("No Data Obtained!!")
            Exit Sub
        End If
        GRID2.Rows.Clear()
        filtnew = ""
        For i = 0 To dt.Rows.Count - 1
            Dim val = GRID2.Rows.Add
            GRID2(0, i).Value = dt.Rows(i).Item(10).ToString
            GRID2(1, i).Value = dt.Rows(i).Item(0).ToString
            GRID2(2, i).Value = dt.Rows(i).Item(1).ToString
            GRID2(3, i).Value = dt.Rows(i).Item(5).ToString.Replace(" 00:00:00", "")
            GRID2(4, i).Value = dt.Rows(i).Item(6).ToString.Replace(" 00:00:00", "")
            GRID2(5, i).Value = dt.Rows(i).Item(4).ToString
            filtnew = filtadd(GRID2(1, i).Value.ToString, filtnew)
            dv.RowFilter = filtnew
            dv.RowStateFilter = DataViewRowState.CurrentRows
        Next
        Dim Filtamake As String = ""
        GRID1.ClearSelection()
        GRID2.ClearSelection()
    End Sub
    'Save BUtton to save the new Records/ ONly Works with new Records.
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox2.SelectedIndex <= 0 Then
            MsgBox("Please Select the person who is assigned these Stages!!")
            Exit Sub
        End If
        If MsgBox("Are You sure that you carry out single type of process at once", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            If createtransdt() = False Then
                Exit Sub
            End If
            If dt2.Rows.Count >= 0 Then
                Dim val = QueryModule.savestagemapping(dt2, ComboBox1.SelectedItem.row.item(0).ToString)
                If val.Contains("OK") Then
                    MsgBox(val)
                    GRID2.Rows.Clear()
                    filtnew = ""
                    dv.RowFilter = filtnew
                    dv.RowStateFilter = DataViewRowState.CurrentRows
                    ComboBox1.SelectedIndex = 0
                Else
                    MsgBox(val)
                End If
            End If
        End If
        GRID1.ClearSelection()
        GRID2.ClearSelection()
    End Sub

    'Load Gird Data int to the datatable for the transaction
    Private Function createtransdt() As Boolean
        Try
            GRID1.ClearSelection()
            GRID2.ClearSelection()
            dt2.Rows.Clear()
            For i = 0 To GRID2.Rows.Count - 1
                Int32.Parse(GRID2(1, i).Value, CultureInfo.CurrentCulture)
                'Dim ob As Object = {13, ComboBox1.SelectedItem.row.item(0).ToString, GRID2(4, i).Value.ToString, Date.ParseExact(GRID2(3, i).Value.ToString, "yyyy-MM-dd", CultureInfo.InvariantCulture), Now(), TextBox1.Text}
                Dim datar As DataRow = dt2.Rows.Add()
                datar.Item(0) = GRID2(1, i).Value
                datar.Item(1) = GRID2(2, i).Value
                datar.Item(2) = ComboBox1.SelectedItem.row.item(1).ToString
                datar.Item(3) = ComboBox1.SelectedItem.row.item(0).ToString
                If GRID2(5, i).Value Is Nothing Then
                    GRID2(5, i).Value = ""
                End If
                datar.Item(4) = GRID2(5, i).Value.ToString
                Try
                    datar.Item(5) = Date.ParseExact(GRID2(3, i).Value.ToString, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    datar.Item(6) = Date.ParseExact(GRID2(4, i).Value.ToString, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                Catch ex As Exception
                    MsgBox("Please Enter the date in specified format; Inner Error: " + ex.Message)
                    dt2.Rows.Clear()
                    Return False
                End Try
                datar.Item(7) = Now()
                datar.Item(8) = TextBox1.Text
                datar.Item(9) = ComboBox2.SelectedItem.row(0)
                datar.Item(10) = GRID2(0, i).Value
            Next
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

End Class
