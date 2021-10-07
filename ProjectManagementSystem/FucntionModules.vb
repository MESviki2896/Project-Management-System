Imports System
Imports System.IO
Imports LOGANDINI
Imports System.Data.SqlClient
Imports System.Data.Sql
Imports System.Security.Cryptography
Imports ProjectManagementSystem.PubVals
Imports ProjectManagementSystem.ShareFolderMake
Public Class FucntionModules
    Dim pubvals As New PubVals
    Shared sql As New SQL_Conn.Class1
    Shared LOG As New LOGANDINI.Log
    Public Shared Function createshared() As Boolean
        If Not System.IO.Directory.Exists(PubVals.PUBLICSHAREPATH) Then
            ShareFolder(Application.StartupPath + "\UploadFilee", "PMDUPLOAD", "This is PMD Sharing")
            PubVals.PUBLICSHAREPATH = "\\" + System.Net.Dns.GetHostName + "\" + "PMDUPLOAD"
            If String.IsNullOrEmpty(PubVals.PUBLICSHAREPATH) Then
                MsgBox("Unable to Create Common shared folder, Please Check, Try to run Application in Administrator Mode")
                Return False
            End If
        End If
        Return True
    End Function
    '''<summary>
    '''This Function obtains the information from the ini file.
    '''</summary>
    Public Shared Function Getini(ByVal getSection As String, ByVal getKey As String) As String
        If File.Exists(IniFilePath) Then
            Dim ini As New IniFile(IniFilePath)
            Return ini.GetString(getSection, getKey, "NA")
        Else
            Return "File Path Error"
        End If
    End Function
    '--------------DATABASE FUNCTIONS START HERE------------------------
    '''<summary>
    '''This Function handles all the heavy lifting in terms of the database connection.
    '''</summary>

    Public Shared Function OpenDB(ByVal query As String, ByRef command As SqlCommand) As Boolean
        Try
            If query <> "" Then
                sql.DBCONNECTION("DBLogin", IniFilePath)
                If Not (query.ToLower.StartsWith("select") Or query.ToLower.StartsWith("if")) Then
                    sql.BeginTransaction()
                    command.Transaction = sql.command.Transaction
                End If
                sql.sql_command(query)
                command = sql.command
            Else
                Return False
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Public Shared Function CloseDB() As Boolean
        If sql.cnn.State = ConnectionState.Open Then
            sql.cnn.Close()
            Return True
        Else
            Return False
        End If
    End Function
    '''<summary>
    '''This Function Checks the Database Connection status.
    '''</summary>
    Public Shared Function isopen() As Integer
        If sql.cnn.State = ConnectionState.Open Then
            Return 1
        ElseIf sql.cnn.State = ConnectionState.Closed Then
            Return 0
        Else
            Return 3
        End If
    End Function
    '''<summary>
    '''This Function Provides delay if the connection status is open.
    '''</summary>
    Public Shared Function delay(ByVal delaymills As Integer)
        If isopen() = 1 Then
            If delaymills > 0 Then
                Threading.Thread.Sleep(delaymills)
            End If
        End If
    End Function
    '''<summary>
    '''This Function Provides vanity to create database parameters.
    '''</summary>
    Public Shared Function CreateParameter(ByVal type As Integer) As Boolean
        Dim sqlparalist As New List(Of SqlParameter)
        For i = 0 To type - 1
            sqlparalist.Add(New SqlParameter())
        Next
        Dim sqlparaarr() As SqlParameter
        sqlparaarr = sqlparalist.ToArray()
        If sqlparalist.Count > 0 Then
            sql.command.Parameters.AddRange(sqlparaarr)
            Return True
        End If
        Return False
    End Function
    '''<summary>
    '''This Function to add/Provide data to different database parameters.
    '''</summary>
    Public Shared Function AddParamaeter(ByVal index As Integer, ByVal sqlvalue As Object, ByVal sqldata As Object, Optional ByVal dattype As SqlDbType = SqlDbType.VarChar, Optional ByVal size As Integer = 200, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As Boolean

        If index <= sql.command.Parameters.Count - 1 And index >= 0 Then
            sql.command.Parameters(index).ParameterName = sqlvalue
            sql.command.Parameters(index).Value = sqldata
            sql.command.Parameters(index).Direction = direction
            sql.command.Parameters(index).SqlDbType = dattype
            sql.command.Parameters(index).Size = size
        Else
            Return False
        End If

        Return True
    End Function
    '-----------------DATABASE FUNCTION ENDS HERE--------------------------------
    '''<summary>
    '''This Function Provides LogWriting geature to the application.
    '''</summary>
    Public Shared Function LOGGING(ByVal MESSAGE As String, ByVal TYPE As Integer)
        Select Case TYPE
            Case 0
                LOG.logstart(MESSAGE)
            Case 1
                LOG.Logger(MESSAGE)
        End Select
    End Function
    '''<summary>
    '''This Function is designed to generate MD5 Hash of the given input data.
    '''</summary>
    Public Shared Function cryptic(ByVal theinput As String)
        Using hash As MD5 = MD5.Create()
            Dim dbytes As Byte() =
             hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(theinput))

            ' sb to create string from bytes
            Dim sBuilder As New System.Text.StringBuilder()

            ' convert byte data to hex string
            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString()
        End Using
    End Function
    '''<summary>
    '''This Function is simple yet powerful method to fill a combobox using the datatable.
    '''</summary>
    Public Shared Sub FillComboBox(cbo As ComboBox, dt As DataTable, isSelect As Boolean)
        Try
            If Not dt Is Nothing Then
                If isSelect Then
                    Dim dataRow As DataRow = dt.NewRow()
                    dataRow(0) = "--Select--"
                    dataRow(1) = ""
                    dt.Rows.InsertAt(dataRow, 0)
                End If
                cbo.DisplayMember = dt.Columns(0).ToString()
                cbo.ValueMember = dt.Columns(1).ToString()
                cbo.DataSource = dt
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    '''<summary>
    '''File Streams takes path as input and provides a bytestream or BLob as output.
    '''</summary>
    Public Shared Function filetobinarywrite(ByVal filepath As String) As Byte()
        Dim fs As FileStream = New FileStream(filepath, FileMode.Open, FileAccess.Read)
        Dim br As BinaryReader = New BinaryReader(fs)
        Dim bytes As Byte() = br.ReadBytes(Convert.ToInt32(fs.Length))
        br.Close()
        fs.Close()
        Return bytes
    End Function
    '''<summary>
    '''This Function obtains the byte stream data and this data is converted to file, to be read by the richtextbox.
    '''</summary>
    Public Shared Function binarytofileread(ByVal fileData1 As Byte(), fileextension As String) As String
        Dim sFilepath As String
        sFilepath = System.IO.Path.GetTempFileName()
        System.IO.File.Move(sFilepath, System.IO.Path.ChangeExtension(sFilepath, fileextension))
        sFilepath = System.IO.Path.ChangeExtension(sFilepath, fileextension)
        System.IO.File.WriteAllBytes(sFilepath, fileData1)
        Return sFilepath
    End Function
    '''<summary>
    '''This Function a small help in adding the required data to a Datagridview Combobox Column.
    '''</summary>
    Public Shared Sub fillgridcombobox(ByVal dgvcol As DataGridViewComboBoxColumn, ByVal dt As DataTable, ByVal showselect As Boolean)
        Try
            If Not dt Is Nothing Then
                If showselect Then
                    Dim dataRow As DataRow = dt.NewRow()
                    dataRow(0) = "--Select--"
                    dataRow(1) = ""
                    dt.Rows.InsertAt(dataRow, 0)
                End If
                dgvcol.DisplayMember = dt.Columns(1).ToString()
                dgvcol.ValueMember = dt.Columns(0).ToString()
                dgvcol.DataSource = dt
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    '''<summary>
    '''This Function Does Filtering of the Datagridview.
    '''</summary>
    Public Shared Function filtadd(ByVal stageid As String, ByVal filtnew As String, Optional ByVal stageName As String = "StageID") As String
        Dim filt As String
        filt = String.Format(stageName + "<>{0}", stageid)
        ' filt = "$" + stageName + "<>{stageid}"
        'filtnew = ""
        filtnew += " AND " + filt
        If filtnew.StartsWith(" AND ") Then
            filtnew = filtnew.Substring(4, filtnew.Length - 4).ToString().Trim
        End If
        Return filtnew
    End Function
    '''<summary>
    '''This Function Does Filtering Removal of the Datagridview.
    '''</summary>
    Public Shared Function filteremove(ByVal stageid As String, ByVal filtnew As String, Optional ByVal stagename As String = "StageID") As String
        Dim filt As String
        filt = String.Format(stagename + "<>{0}", stageid)
        Dim query = From x As String In filtnew.Split(" AND ") Where x = filt Select x
        Dim resval As String = query(0).ToString

        If filtnew.IndexOf(resval) = 0 Then
            filtnew = filtnew.Remove(filtnew.IndexOf(resval), resval.Length)
        Else
            filtnew = filtnew.Remove(filtnew.IndexOf(" AND " + resval), resval.Length + " AND ".Length)
        End If
        If filtnew.StartsWith(" AND ") Then
            filtnew = filtnew.Substring(filtnew.IndexOf(" AND ") + " AND ".Length(), filtnew.Length - " AND ".Length())
        End If
        Return filtnew
    End Function
    '''<summary>
    '''This Function Checks the available drive to move the application easily and copies the file to respective drive.
    '''</summary>
    Public Shared Function Filehandling(ByVal fielpath As String, ByVal filename As String) As String
        Dim dirves = DriveInfo.GetDrives()
        'Dim qry = From x In dirves Where x.Name <> "C:\" And x.DriveType = DriveType.Fixed Select x
        ' Dim arr As DriveInfo() = qry.ToArray
        Dim newfilepath = SERVERHAREPATH + "\" + filename
        If newfilepath <> fielpath Then
            If File.Exists(newfilepath) Then
                File.Delete(newfilepath)
            End If
            Try
                File.Copy(fielpath, newfilepath)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            Return newfilepath
        Else
            Return newfilepath
        End If

    End Function
    '''<summary>
    '''This Function Helps to Upload the file to the sever.
    '''</summary>
    Public Shared Function UploadFile(ByVal filepath As String, ByVal filename As String) As String
        ' Dim filelocation = Filehandling(filepath, filename)
        'File.Move(filepath,)
        Try
            Dim val = QueryModule.UploadDoc(filename, filepath)
            If File.Exists(filepath) Then
                File.Delete(filepath)
            End If
            If val.Contains("~") Then
                Return val
            Else
                Return val
            End If


        Catch ex As Exception
            If File.Exists(filepath) Then
                File.Delete(filepath)
            End If
            Return "~" + ex.Message
        End Try

    End Function
    '''<summary>
    '''This Function Helps to get the Datearray for the date in between the dates mentioned.
    '''</summary>
    Public Shared Iterator Function Getdateenum(ByVal start As DateTime, ByVal enddate As DateTime) As IEnumerable(Of Date)
        Dim i As DateTime = start
        While i <= enddate
            Yield i
            i = i.AddDays(1)
        End While
    End Function
    '''<summary>
    '''This Function sends the Datearray, which will be consumed in the MonthCalendar form and displayed as bold .
    '''</summary>
    Public Shared Function HighlightWeekends(ByVal start As DateTime, ByVal enddate As DateTime) As Date()
        Dim qry = From x In Getdateenum(start, enddate) Where (x.DayOfWeek = DayOfWeek.Saturday Or x.DayOfWeek = DayOfWeek.Sunday) Select x
        Dim value = qry.ToList
        Dim dt As New DataTable
        Dim MLISS As New List(Of Date)
        Dim datevalue As String = ""
        For i = -1 To 5
            datevalue += "'" + (Now.Year + i).ToString + "',"
        Next
        datevalue = datevalue.Substring(0, datevalue.Length - 1)
        dt = HolidayEntry.GetHolidays(datevalue)
        If dt Is Nothing Then
            Return value.ToArray
        End If
        Dim QRYDAT As IEnumerable(Of Date)
        For i = 0 To dt.Rows.Count - 1
            QRYDAT = From X In Getdateenum(dt.Rows(i).Item(1), dt.Rows(i).Item(2)) Where (X.DayOfWeek <> DayOfWeek.Saturday Or X.DayOfWeek <> DayOfWeek.Sunday) Select X
            Dim VALUEARR = QRYDAT.ToList
            value.AddRange(VALUEARR)
        Next
        'MonthCalendar1.BoldedDates() = value
        PVCreateProject.Datesarr = value.ToArray
        Return value.ToArray
    End Function
    Public Shared Function GetWorkingdays(ByVal start As DateTime, ByVal enddate As DateTime) As Date()
        Dim boldeddatearr() As Date
        Dim getalldays() As Date
        Dim Workingdaylist As New List(Of Date)
        boldeddatearr = CalendarView.getboldeddatesarray(start, enddate)
        getalldays = Getdateenum(start, enddate).ToArray
        Dim qry = From x In boldeddatearr Select x
        Workingdaylist = getalldays.AsEnumerable.Except(qry.AsEnumerable).ToList
        Return Workingdaylist.ToArray
    End Function
    Public Shared Function GetClosestWorkingdays(ByVal todaydate As DateTime) As Date
        If Isworking(todaydate) = True Then
            Return todaydate
            Exit Function
        End If
        Dim getworkingarr() = GetWorkingdays(PVCreateProject.CONSTALLstartRANG, PVCreateProject.CONSTALLENDRANG)
        Dim qry = From x In getworkingarr Where x >= todaydate Select x
        Dim returnedvalue = qry.ToArray(0)
        Return returnedvalue
    End Function
    Public Shared Function Isworking(ByVal todaydate As DateTime) As Boolean
        Dim getworkingarr() = GetWorkingdays(PVCreateProject.CONSTALLstartRANG, PVCreateProject.CONSTALLENDRANG)
        Dim qry = From x In getworkingarr Where x = todaydate Select x
        Dim qryresult = qry.ToList
        If qryresult.Count = 1 Then
            Return True
        End If
        Return False
    End Function
    Public Shared Function GetPerfectEndDates(ByVal start As String, ByVal duration As Decimal) As Date
        Dim startasdate As Date = Date.ParseExact(start, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
        Dim enddate As Date = startasdate.AddDays(duration)
        Dim checkholidays As Integer = CalendarView.getboldeddates(Date.ParseExact(start, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture), enddate)
        If checkholidays > 0 Then
            enddate = enddate.AddDays(checkholidays)
            '  If FucntionModules.Isworking(enddate) = False Then
            'enddate = FucntionModules.GetClosestWorkingdays(enddate)
            Dim getworkingarr() = GetWorkingdays(PVCreateProject.CONSTALLstartRANG, PVCreateProject.CONSTALLENDRANG)
            Dim qry = From x In getworkingarr Where x >= startasdate Select x
            Dim returndate = qry.ToList
            Dim perfectdate As Date
            If duration > 1 Then
                perfectdate = returndate.Item(returndate.IndexOf(startasdate) + duration - 1)
            Else
                perfectdate = returndate.Item(1)
            End If
            ' Dim qry1 = From x In returndate Where x.Subtract(startasdate).Days = duration Select x
            'Dim returndate1 = qry1.ToArray
            enddate = perfectdate
            ' End If
        End If
        Return enddate
    End Function
End Class
