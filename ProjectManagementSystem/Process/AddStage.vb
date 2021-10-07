Imports ProjectManagementSystem.QueryModule
Imports ProjectManagementSystem.PVCreateProject
Public Class AddStage
    Private Sub AddStage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        Panel2.Enabled = False
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToDeleteRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.ReadOnly = True
        ComboBox1.SelectedIndex = 0
        GRID1.RowHeadersVisible = False

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        insertdata()
        PROCESSALIAS = Nothing
        PROCESSDESC = Nothing
        PROCESSNAME = Nothing
    End Sub
    Private Sub insertdata()
        If ComboBox1.SelectedIndex = 0 Then
            MsgBox("Please select a proper Type for the Entry!")
            Exit Sub
        End If
        If TextBox1.Text <> "" And TextBox2.Text <> "" And TextBox3.Text <> "" Then
            PROCESSALIAS = TextBox3.Text.ToUpper.Trim()
            PROCESSDESC = TextBox2.Text.Trim
            PROCESSNAME = TextBox1.Text.ToUpper.Trim()
            DURATION = TextBox4.Text
            Dim res As String = ""
            If InsertStageSub(ComboBox1.SelectedItem.ToString(), res) = True Then
                'Lets make a custom message box for this afterwards.
                MsgBox("Succcessful")
                Button4.PerformClick()
            Else
                MsgBox(res.Substring(1, res.Length - 1).ToString, MsgBoxStyle.Exclamation, "Result")
            End If
        End If
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If ComboBox1.SelectedIndex = 0 Then
            MsgBox("Please select the Type of process you want to look at!", MsgBoxStyle.Information, "Selection Error")
            Exit Sub
        End If
        Dim res As String = ""
        Dim dt As New DataTable()
        dt = GetStageProcessDetails(ComboBox1.SelectedItem.ToString, False, res)
        If dt Is Nothing And res.Contains("~") Then
            MsgBox(res.Substring(1, res.Length - 1), MsgBoxStyle.Information, "Error")
            Exit Sub

        End If
        GRID1.DataSource = dt
        GRID1.ClearSelection()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ComboBox1.SelectedIndex = 0 Then
            MsgBox("Please choose the type of process you want to add, from the dropdown!", MsgBoxStyle.Information, "Select Type")
            Exit Sub
        End If
        Panel2.Enabled = True
        TextBox1.Focus()
        TextBox1.Select()
    End Sub
End Class