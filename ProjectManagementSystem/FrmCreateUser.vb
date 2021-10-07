Imports ProjectManagementSystem.PVCreateUser
Imports ProjectManagementSystem.PubVals
Imports ProjectManagementSystem.QueryModule
Public Class FrmCreateUser
    Private Sub FrmCreateUser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = PubVals.Dicon
        Me.Text = ApplicationVersion
        TextBox1.Select()
        TextBox1.Focus()
        Me.StartPosition = FormStartPosition.CenterScreen
        TextBox4.PasswordChar = "*"
        'MdiParent = PMSMain
    End Sub

    Private Sub TextBox1_keydown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox2.SelectAll()
            TextBox2.Focus()
        End If
    End Sub
    Private Sub TextBox2_Keydown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox3.SelectAll()
            TextBox3.Focus()
        End If
    End Sub
    Private Sub TextBox3_keydown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox4.SelectAll()
            TextBox4.Focus()
        End If
    End Sub
    Private Sub TextBox4_Keydown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox5.SelectAll()
            TextBox5.Focus()
        End If
    End Sub
    Private Sub TextBox5_keydown(sender As Object, e As KeyEventArgs) Handles TextBox5.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBox1.Text <> "" And TextBox2.Text <> "" And TextBox3.Text <> "" And TextBox4.Text <> "" And TextBox5.Text <> "" Then
                Button3.PerformClick()
            Else
                TextBox1.Select()
                TextBox1.Focus()
            End If
        End If

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If FrmCreateUser() = True Then
            MsgBox("Create User Successful!!,Please Go back and Login again!!")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            TextBox1.Clear()
            TextBox2.Clear()
            TextBox3.Clear()
            TextBox4.Clear()
            TextBox5.Clear()
            TextBox1.Select()
            TextBox1.Focus()
            Me.DialogResult = DialogResult.Cancel
        End If
    End Sub
    Private Sub FormClos(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        NewName = Nothing
        NewCode = Nothing
        NewDepartment = Nothing
        NewName = Nothing
        NewPassword = Nothing
    End Sub

    Private Function FrmCreateUser() As Boolean
        If TextBox1.Text <> "" And TextBox2.Text <> "" And TextBox3.Text <> "" And TextBox4.Text <> "" And TextBox5.Text <> "" Then
            NewName = TextBox1.Text.Trim()
            NewCode = TextBox2.Text.Trim()
            NewPassword = TextBox4.Text.Trim()
            NewEmail = TextBox3.Text.Trim()
            NewDepartment = TextBox5.Text.Trim()
            Dim value As String = CreateUser()
            If value.Contains("~") Then
                MsgBox(value.Substring(1, value.Length - 1).ToString.Trim())
            Return False
        ElseIf value.Contains("OK") Then
            Return True
        End If
        Else
        MsgBox("You have left some details Blank Please Check")
        TextBox1.Select()
        TextBox1.Focus()
        Return False
        End If

    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If MsgBox("Are You Sure to Close this form?", MsgBoxStyle.OkCancel, "Exit Create User Form") = MsgBoxResult.Ok Then
            Me.DialogResult = DialogResult.Abort
            Me.Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox4.PasswordChar = "*" Then
            TextBox4.PasswordChar = ""
        Else
            TextBox4.PasswordChar = "*"
        End If
    End Sub
End Class