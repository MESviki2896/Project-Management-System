Imports System.IO
Imports ProjectManagementSystem.FucntionModules
Public Class ProcessCompletioneEntry
    Dim project As String
    Dim user As String
    Dim Titletype As String
    Dim result As Integer
    Dim resultmessge As String
    Public Sub SubProcessCompletionB(ByVal selproject As String, ByVal selsubs As String, ByVal selstage As String, ByVal startDate As String, ByVal Enddate As String, Optional ByVal type As String = "Sub-Task")
        If (selproject <> "") Then
            project = selproject
        End If
        If (selsubs <> "") Then
            user = selsubs
        End If
        TextBox1.ReadOnly = True
        TextBox2.ReadOnly = True
        TextBox3.ReadOnly = True
        TextBox1.Text = project
        TextBox2.Text = selsubs
        TextBox3.Text = selstage
        TextBox6.Text = startDate
        TextBox7.Text = Enddate
        Titletype = type
    End Sub

    Private Sub SubProcessCompletionB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        Me.Text = "Entry Completion"
        ReasonBox.Visible = False
        Reason.Visible = False
        Panel3.Enabled = False
        TextBox5.Enabled = False
        TextBox5.Select()
        Me.Text = Me.Text + " || " + Titletype
        If Titletype.ToLower = "sub-task" Then

        Else
            If TextBox2.Text <> "" Then
                RadioButton1.Hide()
            End If
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Panel3.Enabled = True

        ''This sectio nshould check if the records exist, if it really does exit , then it will check whether the date is exceeded, if it has exceeded then it will not enable the editing window and will only show the details and the files 
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        Button5.Enabled = True
        Panel3.Enabled = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OpenFileDialog1.Filter = "All Compressed Files(*.zip;*.rar;*.7z)|*.ZIP;*.RAR;*.7z"
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim filepath = OpenFileDialog1.FileName
            Dim info As New FileInfo(filepath)
            Dim filename As String = info.Name
            TextBox5.Text = filepath + "|" + filename
            ' File.Move(filename,) Then
        End If
    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        UpdateData()
    End Sub
    Private Sub UpdateData()

        '--------DECLARE ALL REQUIRED VARIABLES-----------------
        Dim acktodate As Date = Nothing
        Dim difference As Integer = -500
        Dim reason As String = ""
        Dim UPID As String = ""
        Dim description As String = ""
        Dim filename As String = ""
        Dim filedetails As String
        Dim retval As String
        Dim Finresult As Integer
        Dim resulttext As String = resultmessge
        Dim subtask As String = TextBox2.Text
        '---------FUCNTIONS-----------------------
        If Not (RadioButton1.Checked = True Or RadioButton2.Checked = True) Then
            MsgBox("Please Select any one result i.e Accept or Reject!")
            Exit Sub
        End If
        '-------------- check if reasonbox is visible.
        If ReasonBox.Visible Then
            If ReasonBox.Text = "" Then
                MsgBox("Please enter the reason before saving the data!!")
                Exit Sub
            End If
            reason = ReasonBox.Text
        End If
        Dim res As Boolean = CheckGap(acktodate, difference)
        Finresult = result
        '----- Check if finResult is 0 and sub is selected
        If Finresult = 0 Then
            If subtask = "" Then
                MsgBox("Please Select The reject Sub process!!")
                Exit Sub
            End If
        End If
        If res = False Then
            Exit Sub
        End If
        filedetails = TextBox5.Text
        If Panel3.Enabled And Finresult = 1 Then
            If filedetails <> "" Then
                Cursor.Current = Cursors.WaitCursor
                UPID = UploadFile(filedetails.Split("|")(0), filedetails.Split("|")(1))
                If UPID.Contains("~") Then
                    MsgBox("The Upload file already Exist!!")
                    Exit Sub
                End If
                Cursor.Current = Cursors.Default
                MsgBox($"Document Uploading Successful!!" + vbCrLf + $"Document ID {UPID}")
            End If

            If TextBox4.Text <> "" Then
                description = TextBox4.Text
            End If
        End If
        If Me.Text.ToLower.Contains("sub-task") Then
            retval = QueryModule.UpdateTask(TextBox1.Text, TextBox3.Text.Split("|")(0), TextBox2.Text.Split("|")(0), acktodate, difference, reason, description, UPID, If(filedetails.Contains("|"), filedetails.Split("|")(1), ""))
        Else
            retval = QueryModule.UpdateStageTask(TextBox1.Text, TextBox3.Text.Split("|")(0), acktodate, difference, Finresult, reason, description, UPID, If(filedetails.Contains("|"), filedetails.Split("|")(1), ""), resulttext, subtask)
        End If
        If retval.Contains("~") Then
            MsgBox(retval.Substring(1, retval.Length - 1))
        Else
            MsgBox("Successful!!")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        'MsgBox(diffdates.Days.ToString)
    End Sub

    Private Function CheckGap(ByRef enddate As Date, ByRef difffdate As Integer) As Boolean
        Try
            Dim acktodate As Date = Now.Date()
            Dim todate As Date = Date.ParseExact(TextBox7.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
            Dim diffdates = todate.Subtract(acktodate) 'acktodate.Subtract(todate)
            If ReasonBox.Text <> "" Then
                enddate = acktodate
                difffdate = diffdates.Days
                Return True
            End If
            If diffdates.Days <= -5 Then
                MsgBox("This task is beyond deadline" + vbCrLf + "Please specify the reason!!")
                Reason.Show()
                ReasonBox.Show()
                ReasonBox.Select()
                ReasonBox.Focus()
                Return False
            End If
            enddate = acktodate
            difffdate = diffdates.Days
            Return True
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        view()
    End Sub
    Private Sub view()
        If TextBox5.Text <> "" And TextBox5.Text.Contains("|") Then
            Dim filepath As String = TextBox5.Text.Split("|")(0)
            Process.Start(filepath)
        Else
            MsgBox("Please choose a file to view!!")
        End If
    End Sub

    Private Sub ReasonBox_TextChanged(sender As Object, e As EventArgs) Handles ReasonBox.LostFocus
        If ReasonBox.Text = "" Then
            MsgBox("This is a compulsory field,Kindly Enter the reason!!")
            ReasonBox.Select()
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            result = 1
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            result = 0
            resultmessge = InputBox("Please Enter Reason for Reject!!", Me.Text + "-" + "Reject")
        End If

    End Sub
End Class