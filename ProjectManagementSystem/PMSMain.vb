Imports System
Imports System.Globalization
Imports ProjectManagementSystem.PubVals
Imports ProjectManagementSystem.QueryModule
Imports ProjectManagementSystem.FucntionModules
Public Class PMSMain
    Private Sub changemdicolor()
        For Each ctrl As Control In Me.Controls

            If ctrl.Name = "" Then
                ctrl.BackColor = Color.FromKnownColor(KnownColor.Lavender)
            End If
        Next
    End Sub
    Private Sub PMSMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        changemdicolor()
        Label3.Text = UserID
        Me.Text = ApplicationVersion
        CheckForIllegalCrossThreadCalls = False
        BackgroundWorker1.WorkerSupportsCancellation = True
        Dim dates As CultureInfo = New CultureInfo("en-US")
        Label10.Text = Date.Parse(HeartBeat(), dates).DayOfWeek.ToString()
        BackgroundWorker1.RunWorkerAsync()
        Label6.Text = Getini("DBLogin", "Server_IP")
        Label12.Text = UserType
        CalendarView.Hide()
        hidecontrols()
    End Sub
    Private Sub formclosingPMSMain(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If ActiveMdiChild IsNot Nothing Then
            If ActiveMdiChild.Visible Then
                MsgBox($"Please Save and close '{ActiveMdiChild.Name}' before exit!")
                e.Cancel = True
                Exit Sub
            End If
        End If
        FrmLogin.Show()
        BackgroundWorker1.CancelAsync()
        FrmLogin.Show()
    End Sub
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        While Not e.Cancel
            Try
                Dim value = HeartBeat()

                If value.Contains("~") Then
                    MsgBox(value.Substring(0, value.Length - 1))
                Else
                    Label8.Text = value
                End If
                If BackgroundWorker1.CancellationPending Then
                    Exit Sub
                End If
            Catch ex As Exception
                Exit Sub
            End Try
            ' Threading.Thread.Sleep(500)
        End While
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        ImmediateProject.Show()
    End Sub

    Private Sub CreateProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateProjectToolStripMenuItem.Click
        'MsgBox(sender.ToString)
        CreateProject.Show()
    End Sub

    Private Sub StageListAddToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StageListAddToolStripMenuItem.Click
        AddStage.Show()
    End Sub

    Private Sub StageProjectMappingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StageProjectMappingToolStripMenuItem.Click
        ProjectStageMapping.Show()
    End Sub

    Private Sub SubProcessProjectMappingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SubProcessProjectMappingToolStripMenuItem.Click
        SubProcessStageMapping.Show()
    End Sub

    Private Sub ChangePasswordToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub EntrySubProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EntrySubProcessToolStripMenuItem.Click
        SubProcessCompletion.Show()
    End Sub

    Private Sub ValidateStageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ValidateStageToolStripMenuItem.Click
        ValidateStage.Show()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox1.Show()
    End Sub

    Private Sub HolidayCalendarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HolidayCalendarToolStripMenuItem.Click
        CalendarView.Show()
    End Sub

    Private Sub LogOutToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles LogOutToolStripMenuItem1.Click
        If MsgBox($"Are You sure to LogOut from user {UserID} ? Please Make Sure there is no unsaved data", vbYesNo, ApplicationVersion + " - Log Out") = MsgBoxResult.Yes Then
            End
        End If
    End Sub

    Private Sub SwiitchUserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SwiitchUserToolStripMenuItem.Click
        If MsgBox("Are You sure to change user?", vbYesNo, ApplicationVersion + " - Change User") = MsgBoxResult.Yes Then
            Try
                Me.Close()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub AccountGroupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AccountGroupToolStripMenuItem.Click
        Account_Group.Show()
    End Sub

    Private Sub UserPermissionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UserPermissionsToolStripMenuItem.Click
        UserPermissions.Show()
    End Sub
    Private Function hidecontrols()
        '  Dim controlcollection = Me.tool 'Me.Controls.Item(Me.Controls.IndexOf(MenuStrip1))
        'Dim container = controlcollection
        Dim liss As New List(Of ToolStripMenuItem)
        For Each TSI As ToolStripMenuItem In MenuStrip1.Items
            'liss.Add(TSI)
            liss.AddRange(GetItems(TSI))
        Next

        Dim lissstring As New List(Of String)
        lissstring = RoleSettings.GetRoleModule()
        For i = 0 To lissstring.Count - 1
            Dim value = lissstring.Item(i).ToString
            Dim val = liss.Find(Function(x) x.Text = value)
            val.Visible = False
        Next

        ' Dim val = controlcollection.
        ' controlcollection.IndexOf()

        '  Dim qry = From x As ToolStripMenuItem In controlcollection.AsQueryable Where x.Name = "Create project"
        '  Dim value = qry.ToArray()

        '  CreateProjectToolStripMenuItem.Visible = False
    End Function

    Private Iterator Function GetItems(ByVal item As ToolStripMenuItem) As IEnumerable(Of ToolStripMenuItem)
        For Each dropDownItem As ToolStripMenuItem In item.DropDownItems
            If dropDownItem.HasDropDownItems = True Then

                For Each subItem As ToolStripMenuItem In GetItems(dropDownItem)
                    Yield subItem
                Next
            End If
            Yield dropDownItem
        Next
    End Function


End Class
