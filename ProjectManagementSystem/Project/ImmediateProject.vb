Imports ProjectManagementSystem.QueryModule
Public Class ImmediateProject
    Dim checklist As New List(Of String)

    Private Sub ImmediateProject_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToDeleteRows = False
        GRID1.AllowUserToOrderColumns = False
        GetProjects()
        GRID1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        GRID1.AutoGenerateColumns = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim Returnliss As New List(Of String)
        Dim strinjon As String = String.Join(",", checklist)
        If strinjon = "" Then
            MsgBox("Please select Project with status of 'NOT STARTED'")
            Exit Sub
        End If
        Returnliss = UpdateProjStatus(strinjon)
        If Returnliss Is Nothing Then
            MsgBox("No Record of any Update Status is recieved, Please check Connection!")
        End If
        If Returnliss.Item(0).Contains("~") Then
            Dim lissmesage As String = Returnliss.Item(1).ToString
            Returnliss.RemoveRange(0, 2)
            Dim joinstr As String = String.Join(vbCrLf, Returnliss)
            MsgBox(lissmesage + vbCrLf + vbCrLf + joinstr + vbCrLf + vbCrLf + "The Update Status process did not complete!!")
        Else
            MsgBox("SUCCESSFUL")
            GetProjects()
            Label2.Text = 0
        End If
    End Sub
    Private Function GetProjects()
        Dim dt As New DataTable
        Dim user As String = PubVals.UserType
        If user.ToUpper.Contains("ADMIN") Then
            dt = QueryModule.IMEEDIATEGET(PubVals.UserType)
            If dt IsNot Nothing Then
                GRID1.DataSource = dt
            Else
                MsgBox("No Records Recieved!!")
            End If
        End If
    End Function

    Private Sub GRID1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles GRID1.CellContentClick
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 0 And GRID1(3, e.RowIndex).Value = "NOT STARTED" Then
                'Start from here 
                If GRID1(0, e.RowIndex).Value = False Then
                    checklist.Add(GRID1(1, e.RowIndex).Value)
                    GRID1(0, e.RowIndex).Value = True
                ElseIf GRID1(0, e.RowIndex).Value = True Then
                    checklist.Remove(GRID1(1, e.RowIndex).Value)
                    GRID1(0, e.RowIndex).Value = False
                End If
                Label2.Text = checklist.Count.ToString
            Else
                GRID1(0, e.RowIndex).Value = False

            End If
        End If
    End Sub
End Class