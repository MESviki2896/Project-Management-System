Imports ProjectManagementSystem.FucntionModules
Imports ProjectManagementSystem.QueryModule
Public Class SubProcessCompletion
    Private Sub SubProcessCompletion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        FillComboBox(ComboBox1, GetAssignedProject("Sub-Stage"), True)
        Me.Icon = PubVals.Dicon
        GRID1.AutoGenerateColumns = False
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToDeleteRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.ReadOnly = True
    End Sub
    Private Sub GetAssignedMapping()
        Dim dt As New DataTable
        Try

            dt = GetassignedDetails(ComboBox1.SelectedItem.row(0), PubVals.UserID)
            If dt Is Nothing Then
                Exit Sub
            End If
            GRID1.DataSource = dt

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        GRID1.ClearSelection()
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        GetAssignedMapping()
    End Sub

    Private Sub GRID1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID1.CellContentClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 0 Then

                'THERE WILL BE ONE PRIOR STAGE CHECK FUNCTION, IT WILL GIVE OUT THE RESULT.
                If GRID1(10, e.RowIndex).Value.ToString.Contains("NOT STARTED") Or GRID1(10, e.RowIndex).Value.ToString.Contains("REJECTED") Then
                    Dim resultexplain As String = ""
                    Dim value = QueryModule.CHECKCURRENTPROCESS(ComboBox1.SelectedItem.row(0), GRID1(2, e.RowIndex).Value.ToString, resultexplain)
                    If value = True Then
                        Dim UpdateDetails As New ProcessCompletioneEntry
                        UpdateDetails.SubProcessCompletionB(ComboBox1.SelectedItem.row(0), GRID1(0, e.RowIndex).Value + "|" + GRID1(1, e.RowIndex).Value.ToString, GRID1(2, e.RowIndex).Value + "|" + GRID1(3, e.RowIndex).Value.ToString, GRID1(4, e.RowIndex).Value.ToString.Replace("00:00:00", "").Trim, GRID1(5, e.RowIndex).Value.ToString.Replace("00:00:00", "").Trim)
                        If UpdateDetails.ShowDialog() = DialogResult.OK Then
                            Button4.PerformClick()
                        End If
                    Else
                        MsgBox(resultexplain)
                    End If
                Else
                    System.Media.SystemSounds.Asterisk.Play()
                End If
            End If
            End If
    End Sub
End Class