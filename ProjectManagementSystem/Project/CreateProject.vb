Imports ProjectManagementSystem.PubVals
Imports ProjectManagementSystem.PVCreateProject
Imports ProjectManagementSystem.FucntionModules
Imports ProjectManagementSystem.QueryModule
Imports System.IO
Imports System.Windows.Documents
Public Class CreateProject
    Private Sub CreateProject_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MdiParent = PMSMain
        Me.Icon = PubVals.Dicon
        FillComboBox(ComboBox1, GetUsers(True), True)
        FillComboBox(ProjectQuery, GetProject(), True)
        panelcontrolstoggle(False, False)
        checkusertype()
        Me.WindowState = FormWindowState.Maximized
        TextBox3.ReadOnly = True
        DateTimePicker1.Value = Now
        TextBox4.ReadOnly = True
        TextBox5.ReadOnly = True
        'RichTextBox1.LoadFile("C:\Users\51009545\Desktop\New Microsoft Word Document (2).rtf")
        ' RichTextBox1.SaveFile(Application.StartupPath + "/data/" + "Document.rtf")

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox3.Text = PMSMain.Label3.Text
        panelcontrolstoggle(True, False)
    End Sub
    Private Sub checkusertype()
        If Not PMSMain.Label12.Text.Contains("ADMIN") Then
            panelcontrolstoggle(False, True)
            FLpanelcontrolstoggle(False, True)
            SPanelcontrolstoggle(False)
            'Query Peoject Will go here...
            'we will add function get project and display in combobox
            ' The user can search the project And load it
            'the loaded project details will be in diabled format so that user can jus tsee it and cannot  edit it.
        Else
            panelcontrolstoggle(True, True)
        End If
    End Sub
    Private Function panelcontrolstoggle(ByVal createbool As Boolean, ByVal querybool As Boolean)
        For Each ctr As Control In Panel1.Controls

            If Not ctr.Name.ToLower.Contains("query") Then
                ctr.Enabled = createbool
            Else
                ctr.Enabled = querybool
            End If
        Next
    End Function
    Private Sub SPanelcontrolstoggle(ByVal createbool As Boolean)
        For Each ctr As Control In SplitContainer1.Panel1.Controls
            Dim val As String = ctr.GetType.Name
            If Not ctr.GetType.Name = "Label" Then
                ctr.Enabled = createbool
            End If
        Next
        For Each ctr As Control In SplitContainer1.Panel2.Controls
            Dim val As String = ctr.GetType.Name

            If Not ctr.GetType.Name = "Label" Then
                ctr.Enabled = createbool
            End If
        Next
    End Sub
    Private Sub FLpanelcontrolstoggle(ByVal createbool As Boolean, ByVal querybool As Boolean)
        For Each ctr As Control In FlowLayoutPanel1.Controls

            If Not ctr.Name.Contains("Button4") Then
                ctr.Enabled = createbool
            Else
                ctr.Enabled = querybool
            End If
        Next
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        '  MsgBox("This is for later Implemetation!!")
        Dim byes As Byte() = Nothing
        getprojdetails(byes)
        Dim loc As String = binarytofileread(byes, ".rtf")
        RichTextBox1.LoadFile(loc)
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        OpenFileDialog1.Filter = "Rich Text format(*.rtf)|*.rtf"
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim filepath As String = OpenFileDialog1.FileName
            RichTextBox1.LoadFile(filepath)
        End If
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim filepath As String = OpenFileDialog1.FileName
            RichTextBox2.LoadFile(filepath)
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click 'save project
        If DateTimePicker1.Value.Date = Now.Date() Then 'Check date
            MsgBox("Please Choose the Start Date of Project!!")
            Exit Sub
        End If

        If ComboBox1.SelectedItem.ToString = "" Then 'Check  admin exist
            MsgBox("Please click on Create Project Button to initiate a Project!")
            Exit Sub
        End If

        If TextBox1.Text = "" And TextBox2.Text = "" And TextBox3.Text = "" And ComboBox1.SelectedIndex = 0 Then 'check if any detail is empty
            TextBox1.Select()
            TextBox1.Focus()
            MsgBox("Please add Required parameters for the Project!")
            Exit Sub
        End If

        If RichTextBox1.TextLength() > 0 And RichTextBox2.TextLength() > 0 Then ' check if the description and specifciation is emppty

            RichTextBox1.SaveFile(Application.StartupPath + "/data/specs.rtf")
            RichTextBox2.SaveFile(Application.StartupPath + "/data/descs.rtf")
            Dim byes1, byes2 As Byte()
            byes1 = filetobinarywrite(Application.StartupPath + "/data/specs.rtf")
            byes2 = filetobinarywrite(Application.StartupPath + "/data/descs.rtf")
            ModelSpecification = byes1
            ModelDescription = byes2
        Else
            MsgBox("Model Specification and Description are important paramters for a Project, Please don't leave it blank!", MsgBoxStyle.Critical)
        End If

        'Get other Parameters
        ProjecName = TextBox1.Text
        ModelName = TextBox2.Text
        CreatedBy = TextBox3.Text
        ModelAdmin = ComboBox1.SelectedItem.row.item(0).ToString
        ProjStartDate = DateTimePicker1.Value.ToString

        'Uploading the files module is remaining
        Dim specificationfilepath As String = TextBox4.Text.Split("|")(0)
        Dim specificationfilename As String = TextBox4.Text.Split("|")(1)
        Dim Descriptionfilepath As String = TextBox5.Text.Split("|")(0)
        Dim Descriptionfilename As String = TextBox5.Text.Split("|")(1)

        Dim SpecificationUploadID As String = ""
        Dim DescriptionUploadID As String = ""

        'If specificationfilepath <> "" And specificationfilename <> "" Then
        '    SpecificationUploadID = FucntionModules.UploadFile(specificationfilepath, specificationfilename)
        'End If
        'If specificationfilepath <> "" And specificationfilename <> "" Then
        '    DescriptionUploadID = FucntionModules.UploadFile(Descriptionfilepath, Descriptionfilename)
        'End If
        If Not SpecificationUploadID.Contains("~") And Not DescriptionUploadID.Contains("~") Then
            Dim retval = saveproject(specificationfilename, specificationfilepath, Descriptionfilename, Descriptionfilepath)
            If retval.Contains("~") Then
                MsgBox("FAILED: Reason = " + retval.Substring(1, retval.Length - 1))
            Else
                MsgBox("Project Created Successfully!")
                clearcontrols()
            End If
        Else
            MsgBox(SpecificationUploadID + vbCrLf + vbCrLf + DescriptionUploadID)
        End If


        Try
            File.Delete(Application.StartupPath + "/data/specs.rtf")
            File.Delete(Application.StartupPath + "/data/descs.rtf")
            File.Delete(specificationfilepath)
            File.Delete(Descriptionfilepath)
        Catch ex As Exception
            MsgBox("File delete Error; Reason: " + vbCrLf + vbCrLf + ex.Message)
            Exit Sub
        End Try
    End Sub
    Private Sub clearcontrols()
        RichTextBox1.Clear()
        RichTextBox2.Clear()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        ComboBox1.SelectedIndex = 0
    End Sub
    Private Sub createprojclosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ModelSpecification = Nothing
        ModelDescription = Nothing
        ProjecName = Nothing
        ModelName = Nothing
        CreatedBy = Nothing
        ModelAdmin = Nothing
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim FILENAME As String = ""
        Dim FILEPATH As String = ""
        FILEDIALOG(FILEPATH, FILENAME)
        Me.Cursor = Cursors.WaitCursor
        FILEPATH = Filehandling(FILEPATH, FILENAME)
        TextBox4.Text = FILEPATH + "|" + FILENAME
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim FILENAME As String = ""
        Dim FILEPATH As String = ""
        FILEDIALOG(FILEPATH, FILENAME)
        Me.Cursor = Cursors.WaitCursor
        FILEPATH = Filehandling(FILEPATH, FILENAME)
        TextBox5.Text = FILEPATH + "|" + FILENAME
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub FILEDIALOG(ByRef FILEPATH As String, ByRef FILENAME As String)
        OpenFileDialog1.Filter = "All Standard File Format(*.docx;*.xlsx;*.xls;*.doc;*.rtf;*.pdf,*rar,*zip)|*.docx;*.xlsx;*.xls;*.doc;*.rtf;*.pdf,*rar,*zip"
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            FILEPATH = OpenFileDialog1.FileName
            Dim INFO As New FileInfo(FILEPATH)
            FILENAME = INFO.Name

        End If
        If FILENAME Is Nothing Or FILENAME = "" Then
            MsgBox("FILE NOT CHOSEN!")
            Exit Sub
        End If
        If FILEPATH Is Nothing Or FILEPATH = "" Then
            MsgBox("FILE NOT CHOSEN!")
            Exit Sub
        End If

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If CalendarView.Visible = False Then
            CalendarView.Show()
        Else
            MsgBox("Calendar is already Open!!")
        End If
    End Sub
End Class