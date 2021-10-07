Imports ProjectManagementSystem.QueryModule
Imports ProjectManagementSystem.HolidayEntry
Imports ProjectManagementSystem.FucntionModules
Public Class CalendarView
    Dim startdate As Date
    Dim enddate As Date
    Private Sub MonthCalendar1_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateSelected
        startdate = e.Start
        enddate = e.End
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If startdate.Year <> enddate.Year Then
            MsgBox("Please add the holiday for the different year separately!!")
            Exit Sub
        End If
        Dim rowvalue = GRID1.Rows.Add()
        GRID1.Rows.Item(rowvalue).HeaderCell.Value = rowvalue.ToString
        GRID1(1, rowvalue).Value = startdate.Date.ToString.Replace("00:00:00", "").Trim
        GRID1(2, rowvalue).Value = enddate.Date.ToString.Replace("00:00:00", "").Trim
        GRID1(3, rowvalue).Value = enddate.Year

        Dim dfference As Integer = enddate.Subtract(startdate).Days
        GRID1(4, rowvalue).Value = dfference + 1
        Highlightdays(startdate.Date, enddate.Date)
    End Sub

    Private Sub CalendarView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = PubVals.Dicon
        MdiParent = PMSMain
        GroupBox1.Enabled = False

        GRID1.AllowUserToAddRows = False
        GRID1.AllowUserToDeleteRows = False
        GRID1.AllowUserToOrderColumns = False
        GRID1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
        GRID1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        MonthCalendar1.BoldedDates = FucntionModules.HighlightWeekends(PVCreateProject.CONSTALLstartRANG, PVCreateProject.CONSTALLENDRANG)

        'HighlightWeekends(DateAndTime.DateSerial( DateAndTime.DateSerial(Now.Year - 5, 1, 1))
    End Sub

    Private Function Highlightdays(ByVal start As DateTime, ByVal enddate As DateTime)
        Dim qry = From x In Getdateenum(start, enddate) Where (x.DayOfWeek <> DayOfWeek.Saturday Or x.DayOfWeek <> DayOfWeek.Sunday) Select x
        Dim value = qry.ToArray
        If MonthCalendar1.BoldedDates.Length > 0 Then
            Dim datearr As New List(Of Date)
            datearr = MonthCalendar1.BoldedDates.ToList
            datearr.AddRange(value.ToList)
            value = datearr.ToArray
        End If
        MonthCalendar1.BoldedDates() = value
    End Function
    Private Function Delightdays(ByVal start As DateTime, ByVal enddate As DateTime)
        Dim qry = From x In Getdateenum(start, enddate) Where (x.DayOfWeek <> DayOfWeek.Saturday Or x.DayOfWeek <> DayOfWeek.Sunday) Select x
        Dim value = qry.ToArray
        If MonthCalendar1.BoldedDates.Length > 0 Then
            Dim datearr As New List(Of Date)
            datearr = MonthCalendar1.BoldedDates.ToList
            For Each x In value
                datearr.Remove(x)
            Next
            value = datearr.ToArray
        End If
        MonthCalendar1.BoldedDates() = value
    End Function


    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If GRID1.SelectedRows.Count = 1 Then
            Delightdays(GRID1(1, GRID1.SelectedRows.Item(0).Index).Value, GRID1(2, GRID1.SelectedRows.Item(0).Index).Value)
            GRID1.Rows.RemoveAt(GRID1.SelectedRows.Item(0).Index)
        End If
        ' MsgBox(getboldeddates(DateAndTime.DateSerial(Now.Year, 9, 1), DateAndTime.DateSerial(Now.Year, 9, 5)))
    End Sub
    Public Function getboldeddates(ByVal start As DateTime, ByVal enddate As DateTime) As Integer
        Dim dates = MonthCalendar1.BoldedDates()
        Dim qry = From x In dates Order By x Ascending Where x.Date >= start And x.Date <= enddate Select x
        Dim val = qry.ToArray()
        Return val.Length
    End Function
    Public Function getboldeddatesarray(ByVal start As DateTime, ByVal enddate As DateTime) As Date()
        Dim dates = MonthCalendar1.BoldedDates()
        Dim qry = From x In dates Order By x Ascending Where x.Date >= start And x.Date <= enddate Select x
        Dim val = qry.ToArray()
        Return val
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If GRID1.Rows.Count > 0 Then
            Try
                For i = 0 To GRID1.RowCount - 1
                    Cursor.Current = Cursors.WaitCursor
                    Dim resutl As String = HolidayEntry.SaveHolidayRecords(GRID1(0, i).Value, GRID1(1, i).Value, GRID1(2, i).Value, GRID1(3, i).Value, GRID1(4, i).Value)
                    Cursor.Current = Cursors.Default
                Next
                MsgBox("Records were Saved!!")
                MonthCalendar1.BoldedDates = FucntionModules.HighlightWeekends(PVCreateProject.CONSTALLstartRANG, PVCreateProject.CONSTALLENDRANG)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.CheckState = CheckState.Checked Then
            If Not PubVals.UserType.ToLower.Contains("admin") Then
                CheckBox1.CheckState = CheckState.Unchecked
            Else
                GroupBox1.Enabled = True
            End If
        Else
            GroupBox1.Enabled = False

        End If
    End Sub
    Public Sub calendarclosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub
End Class
' Next Task is to display the tool tip text for the specific bolded dates.