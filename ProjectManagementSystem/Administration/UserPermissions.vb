Imports ProjectManagementSystem.RoleSettings
Public Class UserPermissions
    Private Sub Button4_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub UserPermissions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        Loadadditonalsettings()
        UserRolesGet(False, True)
        getotherparameters()
        GRID1.ClearSelection()
        GRID2.ClearSelection()
    End Sub
    Private Sub Loadadditonalsettings()
        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToDeleteRows = False
        GRID1.ReadOnly = True
        GRID1.AllowUserToOrderColumns = False

        GRID2.AllowUserToAddRows = False
        GRID2.AllowUserToDeleteRows = False
        GRID2.ReadOnly = True
        GRID2.AllowUserToOrderColumns = False
    End Sub
    Private Sub UserRolesGet(ByVal checkuserlevel As Boolean, ByVal Getallresults As Boolean, Optional ByVal usertype As String = "")
        Dim result As String = ""
        Dim ds As New DataSet
        Dim moduletable As New DataTable
        Dim sysroletable As New DataTable
        Try
            ds = GetUserRoleInfo(checkuserlevel, Getallresults, usertype, result)
            If ds Is Nothing Then
                MsgBox(result.Substring(1, result.Length - 1))
                Exit Sub
            End If
            moduletable = ds.Tables(0)
            sysroletable = ds.Tables(1)
            If ds.Tables.Count > 0 Then
                If moduletable IsNot Nothing Then
                    If moduletable.Rows.Count > 0 Then
                        GRID1.DataSource = moduletable
                    End If
                End If
                If sysroletable IsNot Nothing Then
                    If sysroletable.Rows.Count > 0 Then
                        GRID2.DataSource = sysroletable
                    End If
                End If

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub getotherparameters()
        Try
            Dim usertable As New DataTable
            usertable = QueryModule.GetUsers(False)
            If usertable Is Nothing Then
                Exit Sub
            ElseIf usertable.Rows.Count = 0 Then
                Exit Sub
            End If
            FucntionModules.FillComboBox(ComboBox2, usertable, True)

            Dim grouptable As New DataSet
            grouptable = RoleSettings.GetGroupRoleInfo()
            If grouptable Is Nothing Then
                MsgBox("Unable to fetch User Group Details")
                Exit Sub
            End If
            If grouptable.Tables.Count > 0 Then
                FucntionModules.FillComboBox(ComboBox1, grouptable.Tables(0), True)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        TextBox1.Text = ComboBox2.SelectedItem.row(1).ToString
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        TextBox3.Text = ComboBox1.SelectedItem.row(1).ToString
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox1.SelectedIndex <= 0 Then
            MsgBox("Please select a User Group!")
            Exit Sub
        End If
        If ComboBox2.SelectedIndex <= 0 Then
            MsgBox("Please select a User!")
            Exit Sub
        ElseIf PubVals.USERID = ComboBox2.SelectedItem.row(0) Then
            MsgBox("Cannot Change the userRole for Self Account!!")
            Exit Sub
        End If

        Dim result As String = RoleSettings.UpdateuserRoles(ComboBox1.SelectedItem.row(0), ComboBox2.SelectedItem.row(1), ComboBox2.SelectedItem.row(0))
        If result.Contains("~") Then
            MsgBox(result.Substring(0, result.Length - 1))
        Else
            MsgBox("SUCCESSFUL")
            UserRolesGet(False, True)
        End If
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        If ComboBox1.SelectedIndex <= 0 Then
            MsgBox("Please select a User Group!")
            Exit Sub
        End If
        If ComboBox2.SelectedIndex <= 0 Then
            MsgBox("Please select a User!")
            Exit Sub
        End If
        UserRolesGet(True, True, ComboBox1.SelectedItem.row(0))

    End Sub
End Class