Imports ProjectManagementSystem.PubVals
Imports ProjectManagementSystem.QueryModule
Imports ProjectManagementSystem.FucntionModules
Imports System.Security.Cryptography
Imports ProjectManagementSystem.ShareFolderMake
Public Class FrmLogin
    Dim pubval As New PubVals
    Dim logindata As New QueryModule
    Dim func As New FucntionModules
    Private Sub FrmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = PubVals.Dicon
        LOGGING("LOGIN", 0)
        LOGGING(ApplicationVersion, 1)
        Me.Text = ApplicationVersion
        TextBox1.Select()
        TextBox1.Focus()

        TextBox2.PasswordChar = "*"
        If FucntionModules.createshared() = False Then Me.Close()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If MsgBox("Are You Sure to Close?", vbOKCancel, "Exit PMS") = MsgBoxResult.Ok Then
            LOGGING("Login Close", 1)
            Me.Close()
        End If
    End Sub
    Private Sub Loginformclosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        UserID = Nothing
        Password = Nothing
        GC.Collect(2)
        'We can use this to close our background thead in future if we plan to.
    End Sub
    Private Function checkLogin(Optional ByRef reason As String = "") As Boolean
        UserID = TextBox1.Text
        UserType = ""
        Dim password = getLogin(UserType)
        If password.Contains("~") Then
            reason = password
        Else
            Dim EnterPssword = TextBox2.Text
            EnterPssword = cryptic(EnterPssword)
            If EnterPssword = password Then
                Return True
                reason = password
            Else
                reason = "Passwords Don't Match!!, Please Check!"
                Return False

            End If
        End If
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text <> "" And TextBox2.Text <> "" Then
            Dim reason As String = ""
            If checkLogin(reason) Then
                LOGGING("Login Successful", 1)
                'MsgBox("Login Successful")
                TextBox1.Clear()
                TextBox2.Clear()
                PMSMain.Show()
                Me.Hide()
                '//TODO
            Else
                LOGGING("LOGIN FAILURE!", 1)
                MsgBox(reason)
                TextBox1.Select()
                TextBox1.Focus()
                TextBox1.Clear()
                TextBox2.Clear()
            End If
        Else
            LOGGING("LOGIN CREDENTIALS EMPTY!", 1)
            MsgBox("User ID, Password Empty, Please Check")
        End If
    End Sub
    Private Sub TextBox1_Keydown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox2.Select()
            TextBox2.Focus()
        End If
    End Sub
    Private Sub TextBox2_Keydown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBox1.Text <> "" And TextBox2.Text <> "" Then
                Button1.PerformClick()
            Else
                TextBox1.Select()
                TextBox1.Focus()
            End If
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim createfrm As New FrmCreateUser
        Dim result As DialogResult = createfrm.ShowDialog()
        createfrm.Dispose()
        TextBox1.Select()
        TextBox1.Focus()
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        MsgBox("This Module will be available shortly!!, Thank You for patience!!", MsgBoxStyle.OkOnly, ApplicationVersion)
    End Sub
End Class