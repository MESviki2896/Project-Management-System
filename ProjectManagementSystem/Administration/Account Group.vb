Imports ProjectManagementSystem.RoleSettings
Public Class Account_Group
    Private Sub Account_Group_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = PubVals.Dicon
        MdiParent = PMSMain
        SetFields()
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.ReadOnly = True
        GRID1.AllowUserToDeleteRows = False
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click, Button2.Click


    End Sub
    Private Sub SetFields()
        Dim dtGRP As New DataTable
        Dim dtBASEROLE As New DataTable
        Dim retval As Boolean = getgroupinfo(dtGRP, dtBASEROLE)
        If retval = False Then
            MsgBox("No records Found!!")
        Else
            GRID1.DataSource = dtGRP
            FucntionModules.FillComboBox(ComboBox1, dtBASEROLE, True)
        End If
    End Sub


    Private Function getgroupinfo(ByRef GRPROLES As DataTable, ByRef BASEROLES As DataTable) As Boolean
        Dim dt As New DataSet
        dt = GetGroupRoleInfo()
        If dt Is Nothing Then
            Return False
        End If
        If dt.Tables.Count = 0 Then
            GRPROLES = Nothing
            Return False
        End If
        GRPROLES = dt.Tables(0)
        BASEROLES = dt.Tables(1)
        Return True
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click, Button1.Click
        If TextBox1.Text = "" Then
            MsgBox("Please Enter the Group Name")
            Exit Sub
        ElseIf ComboBox1.SelectedIndex = 0 Then
            MsgBox("Please select the category of the group!!")
            Exit Sub
        End If
        Dim result = RoleSettings.SetGroupRoleInfo(TextBox1.Text, ComboBox1.SelectedItem.row(0).ToString, TextBox2.Text)
        If result.Contains("~") Then
            MsgBox(result.Substring(1, result.Length - 1))
        Else
            MsgBox("SUCCESSFUL!!")
            TextBox1.Clear()
            ComboBox1.SelectedIndex = 0
            TextBox2.Clear()
            SetFields()
        End If

    End Sub


End Class