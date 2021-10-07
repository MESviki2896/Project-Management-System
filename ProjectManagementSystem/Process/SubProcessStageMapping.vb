Imports ProjectManagementSystem.QueryModule
Imports ProjectManagementSystem.FucntionModules
Imports System.Globalization
Public Class SubProcessStageMapping
    Dim filtnew As String = ""
    Dim val As List(Of String)
    Dim dt3 As New DataTable
    Dim dt2 As New DataTable
    Dim Idxlst As New List(Of Integer)
    Dim Levellst() As String
    Dim dv As New DataView

    Private Sub SubProcessStageMapping_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'PMDDataSet.AccountMaster' table. You can move, or remove it, as needed.
        Me.Icon = PubVals.Dicon
        Me.AccountMasterTableAdapter.Fill(Me.PMDDataSet.AccountMaster)
        MdiParent = PMSMain
        FucntionModules.FillComboBox(ComboBox1, QueryModule.GetProject(), True)
        getsubprocess()
        TextBox1.Text = PubVals.UserID
        TextBox1.ReadOnly = True
        initiatedt2()
        GRID2.AllowUserToAddRows = False
        GRID2.AllowUserToDeleteRows = False
        GRID2.AllowUserToOrderColumns = False
        getusername()
        gridcontrols()
        GRID2.AutoGenerateColumns = False
    End Sub
    Private Function getusername()
        dt3 = QueryModule.GetUsers(False)
    End Function

    Private Sub datagridsortchange()
        For Each columns As DataGridViewColumn In GRID2.Columns
            columns.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        GRID2.Columns.Item(0).ReadOnly = True
        GRID2.Columns.Item(1).ReadOnly = True
        GRID2.Columns.Item(2).ReadOnly = True
        GRID2.Columns.Item(3).ReadOnly = True
        GRID2.Columns.Item(5).ReadOnly = True
    End Sub
    Private Sub getsubprocess()
        Dim res As String = ""
        Dim dt As New DataTable
        If dt Is Nothing Then
            Exit Sub
        End If
        dt = QueryModule.GetStageProcessDetails("Sub-Process", True, res)
        If dt.Rows.Count > 0 Then
            Dim qry = From dr As DataRow In dt Select (dr.Field(Of Int32)("SUBID").ToString + "|" + dr.Field(Of String)("SubProcess") + "|" + dr.Field(Of Decimal)("Standard_Duration(Days)").ToString)
            Dim valist = qry.ToList
            For i = 0 To valist.Count - 1
                Dim rowcount = GRID1.Rows.Add
                GRID1(1, rowcount).Value = valist.Item(i).Trim.Split("|")(0)
                GRID1(2, rowcount).Value = valist.Item(i).Trim.Split("|")(1)
                GRID1(3, rowcount).Value = valist.Item(i).Trim.Split("|")(2)
            Next
        End If
        GRID1.ClearSelection()
    End Sub
    Private Function initiatedt2()
        dt2.Columns.AddRange({New DataColumn("StageID", Type.GetType("System.Int32")),
                        New DataColumn("StageName", Type.GetType("System.String")),
                        New DataColumn("ProjID", Type.GetType("System.String")),
                        New DataColumn("ProjName", Type.GetType("System.String")),
                       New DataColumn("Remarks", Type.GetType("System.String")),
                       New DataColumn("StartDate", Type.GetType("System.DateTime")),
                        New DataColumn("EndDate", Type.GetType("System.DateTime")),
                        New DataColumn("CreatedOn", Type.GetType("System.DateTime")),
                       New DataColumn("CreatedBy", Type.GetType("System.String")),
                       New DataColumn("AssignedTo", Type.GetType("System.String")),
                       New DataColumn("Sequence", Type.GetType("System.String"))})

        dt2.PrimaryKey = New DataColumn() {dt2.Columns("StageID"), dt2.Columns("StageName"), dt2.Columns("ProjName")}
    End Function
    Private Sub gridcontrols()
        GRID0.ReadOnly = True
        GRID0.AllowUserToDeleteRows = False
        GRID0.AllowUserToOrderColumns = False
        GRID0.AllowUserToAddRows = False
        GRID0.ClearSelection()

        GRID1.AllowUserToDeleteRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.AllowUserToAddRows = False
        GRID1.ClearSelection()

        GRID2.AllowUserToDeleteRows = False
        GRID2.AllowUserToOrderColumns = False
        GRID2.AllowUserToAddRows = False
    End Sub
    '----------------------------------------------- Functions End here-----------------------------------

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex > 0 Then
            Dim dt As New DataTable
            dt = QueryModule.GetStageMapping(ComboBox1.SelectedItem.row.item(0).ToString, True)
            If dt Is Nothing Then
                MsgBox("No Stage Mapping Information available for selected Project, Please add stages from Stage Mapping!!", MsgBoxStyle.MsgBoxHelp, "Stage Mapping Information")
                GRID0.DataSource = Nothing
                If GRID2.Rows.Count > 0 Then
                    dt2.Rows.Clear()
                End If
                Exit Sub
            End If
            If dt.Rows.Count > 0 Then
                Dim dts As New DataTable
                dts = dt.Copy()
                dts.Columns.RemoveAt(11)
                dts.Columns.RemoveAt(10)
                dts.Columns.RemoveAt(9)
                dts.Columns.RemoveAt(8)
                dts.Columns.RemoveAt(7)
                dts.Columns.RemoveAt(5)
                dts.Columns.RemoveAt(4)
                dts.Columns.RemoveAt(3)
                dv = dts.AsDataView()
                GRID0.DataSource = dv
            End If

            ' ComboBox1.Enabled = False
        End If

    End Sub
    Private Sub GRID1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID1.CellContentClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 0 Then
                If Idxlst IsNot Nothing Then
                    If Idxlst.Contains(e.RowIndex) Then
                        Idxlst.Remove(e.RowIndex)

                    ElseIf GRID1.Rows.Item(e.RowIndex).Cells(0).Value = False Then
                        Idxlst.Add(e.RowIndex)
                        GRID1.Rows.Item(e.RowIndex).Cells(0).Value = 1
                    Else
                        GRID1.Rows.Item(0).Cells(0).Value = 0
                    End If
                End If
                Dim val = GRID1.Rows.Item(0).Cells(0).Value
            End If
        End If
    End Sub
    Private Sub GRID0_CellContentDBClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID0.CellContentDoubleClick
        If e.RowIndex >= 0 Then
            If Idxlst.Count = 0 Then
                MsgBox("Please select the sub Process!!")
                Exit Sub
            End If

            Dim obj(6) As Object
            For i = 0 To Idxlst.Count - 1
                obj(0) = GRID0(0, e.RowIndex).Value 'mapid
                obj(1) = GRID1(2, Idxlst.Item(i)).Value.ToString
                obj(2) = 0
                obj(3) = ""
                obj(4) = ""
                obj(5) = Date.ParseExact(GRID0(3, e.RowIndex).Value, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")
                obj(6) = FucntionModules.GetPerfectEndDates(GRID0(3, e.RowIndex).Value, (GRID1(3, Idxlst.Item(i)).Value.ToString.Trim))
                Try
                    dt2.Rows.Add(obj)
                Catch ex As System.Data.ConstraintException
                    MsgBox($"Cannot add the Record {GRID1(2, Idxlst.Item(i)).Value} as it already exist!")
                End Try
            Next

            GRID2.DataSource = dt2
            filtnew = filtadd(GRID0(0, e.RowIndex).Value.ToString, filtnew, GRID0.Columns.Item(0).HeaderText)
            dv.RowFilter = filtnew
            dv.RowStateFilter = DataViewRowState.CurrentRows
            Idxclear()
        End If
        GRID0.ClearSelection()
        GRID1.ClearSelection()
        GRID2.ClearSelection()
        If GRID2.RowCount >= 1 Then
            ComboBox1.Enabled = False
        End If
    End Sub
    Private Sub idxclear()
        For Each x In Idxlst
            GRID1(0, x).Value = 0
        Next
        Idxlst.Clear()
    End Sub
    Private Sub GRID2_CellContentDBClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID2.CellContentDoubleClick
        If e.RowIndex >= 0 Then
            idxclear()
            GRID1.ClearSelection()
            If Not filtnew.Contains("MapID<>" + (GRID2(0, e.RowIndex).Value.ToString)) Then
                'GRID2.Rows.RemoveAt(e.RowIndex)
                dt2.Rows.RemoveAt(e.RowIndex)
            Else
                filtnew = filteremove(GRID2(0, e.RowIndex).Value.ToString, filtnew, GRID2.Columns.Item(0).HeaderText)
                dv.RowFilter = filtnew
                dv.RowStateFilter = DataViewRowState.CurrentRows
                dt2.Rows.RemoveAt(e.RowIndex)
            End If
            If GRID2.RowCount = 0 Then
                ComboBox1.Enabled = True
            End If
        End If

        GRID2.ClearSelection()
    End Sub
    Private Sub MEformclosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox1.SelectedIndex > 0 Then
            If GRID0.Rows.Count = 0 Then
                Dim obj(1) As Object
                ' dt2.Columns.AddRange({New DataColumn("AssignedTo", Type.GetType("System.String")), New DataColumn("Sequence", Type.GetType("System.Int32"))})
                For i = 0 To dt2.Rows.Count - 1
                    dt2.Rows(i).Item(2) = ComboBox1.SelectedItem.row(1)
                    dt2.Rows(i).Item(3) = ComboBox1.SelectedItem.row(0)
                    dt2.Rows(i).Item(8) = TextBox1.Text
                    dt2.Rows(i).Item(7) = Now()
                    dt2.Rows(i).Item(9) = GRID2(5, i).Value
                    dt2.Rows(i).Item(10) = GRID2(6, i).Value
                Next
                Dim val = QueryModule.saveSubmapping(dt2, ComboBox1.SelectedItem.row(0))

                If val.Contains("~") Then
                    MsgBox("CRUD unsuccessful, Reason: " + val)
                Else
                    MsgBox(val)
                    dt2.Rows.Clear()
                    filtnew = ""
                    dv.RowFilter = filtnew
                    dv.RowStateFilter = DataViewRowState.CurrentRows
                    ComboBox1.Enabled = True
                End If

            Else
                MsgBox("Please Map for every Stages")
            End If
        End If
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click '-------Search Click
        Dim dt As New DataTable
        Cursor.Current = Cursors.WaitCursor
        dt = QueryModule.GetStageMapping(ComboBox1.SelectedItem.row(0), False, True)
        If dt Is Nothing Then
            MsgBox("No record(s) found!")
            dt2.Rows.Clear()
            Exit Sub
        End If
        Cursor.Current = Cursors.Default
        idxclear()
        GRID1.ClearSelection()
        dt2.Rows.Clear()
        dt2 = dt.Copy
        GRID2.DataSource = dt2
        filtnew = ""
        dv.RowFilter = filtnew
        dv.RowStateFilter = DataViewRowState.CurrentRows
        For i = 0 To GRID2.RowCount - 1
            GRID2(5, i).Value = dt2.Rows(i).Item(9)
            GRID2(6, i).Value = dt2.Rows(i).Item(10)
        Next
        Dim qry = From x In dt2 Select x.Field(Of Int32)(0)
        Dim val = qry.Distinct().ToList
        Dim notcontained As String = ""
        For i = 0 To GRID0.RowCount - 1
            If Not filtnew.Contains("MapID<>" + GRID0(0, i).Value.ToString) And val.Contains(GRID0(0, i).Value) Then
                filtnew = filtadd(GRID0(0, i).Value, filtnew, "MapID")
            Else
                notcontained += GRID0(0, i).Value.ToString + ","
            End If
        Next
        If notcontained <> "" Then
            MsgBox($"Following Map ID are not present in the Saved Records {notcontained.Substring(0, notcontained.Length - 1)} ;" + vbCrLf + vbCrLf + "Solution : Remove the sub Process Records which are not in the stage Mapping and make a new mapping accordingly")
        End If
        dv.RowFilter = filtnew
        dv.RowStateFilter = DataViewRowState.CurrentRows
    End Sub

End Class