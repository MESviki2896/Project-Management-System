Imports ProjectManagementSystem.FucntionModules
Imports ProjectManagementSystem.QueryModule
Public Class ValidateStage
    Dim subreject As String = ""
    Private Sub ValidateStage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        TextBox1.ReadOnly = True
        FillComboBox(ComboBox1, GetAssignedProject("Sub-Stage"), True)
        GRID1.AutoGenerateColumns = False
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToDeleteRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.ReadOnly = True

        GRID2.AutoGenerateColumns = False
        GRID2.AllowUserToAddRows = False
        GRID2.AllowUserToDeleteRows = False
        GRID2.AllowUserToOrderColumns = False
        '  GRID2.ReadOnly = True
        ' GRID2.Columns.Item(GRID2.ColumnCount - 1).ReadOnly = False
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        TextBox1.Text = ComboBox1.SelectedItem.row(2).ToString
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        '-----------SEARCH BUTTON--------------------
        Dim dt As New DataTable
        dt = GetassignedDetails(ComboBox1.SelectedItem.row(0), TextBox1.Text, "STAGE")
        If dt Is Nothing Then
            MsgBox("No Data Obtained!")
            Exit Sub
        End If
        If dt.Rows.Count > 0 Then
            GRID1.DataSource = dt
        Else
            MsgBox("No Records Exist,Please Check")
        End If
    End Sub

    Private Sub GRID1_CellContentDBClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID1.CellContentDoubleClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 0 Then
                Dim dt As New DataTable
                SplitContainer1.SplitterDistance = GRID1.Rows.GetRowsHeight(DataGridViewElementStates.Displayed) * GRID1.Rows.Count
                dt = QueryModule.GetAssignedSubbMapping(ComboBox1.SelectedItem.row(0), GRID1(0, e.RowIndex).Value.ToString)
                If dt Is Nothing Then
                    Exit Sub
                Else
                    GRID2.DataSource = dt
                End If
                subreject = ""
            End If
        End If
    End Sub

    Private Sub GRID1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID1.CellContentClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 6 Then
                If GRID1(6, e.RowIndex).Value.ToString.Contains("COMPLETED") Then
                    Dim resultexplain As String = ""
                    Dim value = QueryModule.CHECKCURRENTPROCESS(ComboBox1.SelectedItem.row(0), GRID1(2, e.RowIndex).Value.ToString, resultexplain)
                    If value = True Then
                        Dim UpdateDetails As New ProcessCompletioneEntry
                        UpdateDetails.SubProcessCompletionB(ComboBox1.SelectedItem.row(0), If(subreject <> "", subreject.Substring(0, subreject.Length - 1), ""), GRID1(0, e.RowIndex).Value + "|" + GRID1(1, e.RowIndex).Value.ToString, GRID1(2, e.RowIndex).Value.ToString.Replace("00:00:00", "").Trim, GRID1(3, e.RowIndex).Value.ToString.Replace("00:00:00", "").Trim, "Stage-Task")
                        If UpdateDetails.ShowDialog() = DialogResult.OK Then
                            Button4.PerformClick()
                            GRID2.DataSource = Nothing
                            SplitContainer1.SplitterDistance = 352
                        Else
                            subreject = ""
                            For i = 0 To GRID2.RowCount - 1
                                GRID2(11, i).Value = 0
                            Next
                        End If
                    Else
                        MsgBox(resultexplain)
                    End If
                ElseIf GRID1(6, e.RowIndex).Value.ToString.Contains("VERIFIED") Then
                    System.Media.SystemSounds.Exclamation.Play()
                Else
                    System.Media.SystemSounds.Asterisk.Play()
                End If
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Help As String = "Process of verifying the Stage Task Records." + vbCrLf + vbCrLf +
            "1. Select Project From the Drop Down" + vbCrLf + vbCrLf +
            "2. Click on Query(The one with the magnifier)" + vbCrLf + vbCrLf +
            "3. Click on the desired Stage to obtain the sub Process information." + vbCrLf + vbCrLf +
            "4. Look at the 2nd table below the Stage Table, check all the given detials." + vbCrLf + vbCrLf +
            "5. Click on the status column of the stage that you want to verify." + vbCrLf + vbCrLf +
            "6. The program will check whether every thing is correct and it will prompt you to enter the attachment for that stage and little description of the status, if the stage is running beyond deadline; the program will ask for a reason. " + vbCrLf + vbCrLf + "7. On a successful completion, it will prompt a 'Successful POP-UP'" + vbCrLf + vbCrLf +
            "8. If the stage has not been completed, then it will give a notification sound!" + vbCrLf + vbCrLf +
            "PS: If you face any Problem kindly share the screenshot at 'vikyath.rao@lavainternational.in'" + vbCrLf + vbCrLf +
            "Happy Using!!☺"
        MsgBox(Help)
    End Sub
    Private Sub GRID2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID2.CellContentClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 8 Then
                Dim byes As Byte() = Nothing
                Cursor.Current = Cursors.WaitCursor
                QueryModule.GetMappingAttachement(GRID2(8, e.RowIndex).Value.ToString, GRID2(9, e.RowIndex).Value.ToString, byes)
                getprojdetails(byes)
                Dim loc As String = binarytofileread(byes, ".rar")
                Process.Start(loc)
                Cursor.Current = Cursors.Default
            ElseIf e.ColumnIndex = 11 Then
                subreject += GRID2(0, e.RowIndex).Value.ToString + ","
            End If
        End If
    End Sub


End Class
