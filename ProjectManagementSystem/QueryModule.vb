Imports System
Imports System.Data.Sql
Imports SQL_Conn.Class1
Imports System.Data.SqlClient
Imports ProjectManagementSystem.PubVals
Imports ProjectManagementSystem.FucntionModules
Imports ProjectManagementSystem.PVCreateUser
Imports ProjectManagementSystem.PVCreateProject
Public Class QueryModule
    ' Dim FunMod As New FucntionModules
    Dim pubval As New PubVals
    Public Shared Function getLogin(ByRef account_Type As String) As String
        If UserID <> "" Then
            Dim query As String = "SELECT PASSWORD+';'+Account_Type+';'+CONVERT(VARCHAR(5),status) FROM AccountMaster WHERE EmpCode='" + UserID + "'"
            Dim COMMAND As New SqlCommand
            Dim retval As String = ""
            Try
                If OpenDB(query, COMMAND) Then
                    retval = COMMAND.ExecuteScalar()
                    CloseDB()

                Else
                    Return "~Unable to execute functions to the Database"
                End If
            Catch ex As Exception
                Return "~" + ex.Message
            End Try

            If Not String.IsNullOrEmpty(retval) Then
                If retval.Split(";")(2).Trim = 0 Then
                    retval = $"~The User {UserID} account is disabled!"
                    Return retval
                End If
                account_Type = retval.Split(";")(1).Trim()
                Return retval.Split(";")(0).Trim()
            Else
                retval = $"~No Data Found for the User {UserID}!, Reason:Account Doesn't Exist"
                Return retval
            End If
            COMMAND.Dispose()
        Else
            Return "~User ID is empty"
        End If

    End Function
    Public Shared Function CreateUser() As String
        Dim SQLTRANS As SqlTransaction = Nothing
        If NewName <> "" And NewCode <> "" And NewPassword <> "" And NewEmail <> "" And NewDepartment <> "" Then
            NewPassword = cryptic(NewPassword)
            Dim query As String = "INSERT INTO AccountMaster(NAME,EmpCode,EmailID,Password,Department,CreatedOn,CreatedBy,StageLevel,Account_Type) VALUES('" + NewName + "','" + NewCode + "','" + NewEmail + "','" + NewPassword + "','" + NewDepartment + "',GETDATE(),'" + NewName + "',1,'GUEST')"
            Dim COMMAND As New SqlCommand
            Dim retval As Integer = 0

            Try

                OpenDB(query, COMMAND)
                SQLTRANS = COMMAND.Transaction
                retval = COMMAND.ExecuteNonQuery()
                If retval = 1 Then
                    SQLTRANS.Commit()
                    CloseDB()
                    Return "OK"
                ElseIf retval > 1 Then
                    SQLTRANS.Rollback()
                    CloseDB()
                    Return "~ More than 1 records were executed, please contact Admin!"
                Else
                    SQLTRANS.Rollback()
                    CloseDB()
                    Return "~ No records were Inserted!"
                End If
            Catch ex As Exception
                CloseDB()
                Return "~" + ex.Message
            End Try
            COMMAND.Dispose()
        Else
            Return "~ Some Fields are Empty , Please Check!!"

        End If

    End Function
    Public Shared Function HeartBeat() As String
        Dim query As String = ""
        Dim sqli As New SQL_Conn.Class1
        Dim command As New SqlCommand
        query = "SELECT GETDATE()"
        Dim RETVAL As String
        Try
            If query <> "" Then

                sqli.DBCONNECTION("DBLogin", IniFilePath)
                If Not (query.ToLower.StartsWith("select") Or query.ToLower.StartsWith("if")) Then
                    sqli.BeginTransaction()
                    command.Transaction = sqli.command.Transaction
                End If
                sqli.sql_command(query)
                command = sqli.command
            Else
                Return False
                End If

                RETVAL = command.ExecuteScalar()
            If Sqli.cnn.State = ConnectionState.Open Then
                Sqli.cnn.Close()
            End If
        Catch ex As Exception
            CloseDB()
            Return "~" + ex.Message
        End Try
        If RETVAL <> "" Then
            Return RETVAL
        Else
            Return "~Unable to Connect Server"
        End If
    End Function '---------------Check Time and Server----------------
    Public Shared Function GetUsers(ByVal admin As Boolean) As DataTable
        Dim query As String
        If admin = True Then
            query = "SELECT EmpCode,Name FROM AccountMaster WHERE UPPER(Account_Type) like '%ADMIN' and status=1"
        Else
            query = "SELECT EmpCode,Name FROM AccountMaster WHERE STATUS=1"

        End If
        Dim Command As New SqlCommand
        Dim ds As New DataSet()
        Dim da As New SqlDataAdapter()
        Try
            OpenDB(query, Command)
            da.SelectCommand = Command
            Command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0)
            End If
        Catch ex As Exception
            MsgBox("Error Thrown during Query Execution , Message:- " + ex.Message)
        End Try
        Return Nothing
    End Function
    Public Shared Function GetProject() As DataTable
        Dim query As String
        query = "SELECT distinct a.ProjName,convert(varchar(52),a.projid),b.startdate FROM ProjectMaster a inner join ProjectDetails b on a.projID=b.projID and a.status=0"
        Dim Command As New SqlCommand
        Dim ds As New DataSet()
        Dim da As New SqlDataAdapter()
        Try
            OpenDB(query, Command)
            da.SelectCommand = Command
            Command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0)
            End If
        Catch ex As Exception
            MsgBox("Error Thrown during Query Execution , Message:- " + ex.Message)
        End Try
    End Function
    Public Shared Function saveproject(Optional ByVal SPECSFILENAME As String = "", Optional ByVal SPECSFILEPATH As String = "", Optional ByVal DESCFILENAME As String = "", Optional ByVal DESCFILEPATH As String = "") As String
        Dim query As String = "SaveProject"
        Dim command As New SqlCommand
        Dim SQLTRANS As SqlTransaction = Nothing
        Try
            OpenDB(query, command)
            SQLTRANS = command.Transaction
            CreateParameter(11)
            AddParamaeter(0, "@ProjectName", ProjecName)
            AddParamaeter(1, "@ModelName", ModelName)
            AddParamaeter(2, "@ProjSpecs", ModelSpecification, SqlDbType.VarBinary, Integer.MaxValue)
            AddParamaeter(3, "@ProjDesc", ModelDescription, SqlDbType.VarBinary, Integer.MaxValue)
            AddParamaeter(4, "@CreatedBy", CreatedBy)
            AddParamaeter(5, "@ManagBy", ModelAdmin)
            AddParamaeter(6, "@SpecsFilename", SPECSFILENAME)
            AddParamaeter(7, "@SpecsFilePath", SPECSFILEPATH)
            AddParamaeter(8, "@DescFileName", DESCFILENAME)
            AddParamaeter(9, "@DescFilePath", DESCFILEPATH)
            AddParamaeter(10, "@StartDate", ProjStartDate.ToString, SqlDbType.VarChar, 20)
            command.CommandType = CommandType.StoredProcedure
            Dim retval As String = command.ExecuteScalar()
            SQLTRANS.Commit()
            CloseDB()
            Return retval
        Catch ex As Exception
            SQLTRANS.Rollback()
            CloseDB()
            Return "~" + ex.Message
        End Try
    End Function
    Public Shared Function getprojdetails(ByRef byes As Byte()) As String
        Dim query As String = "select PDescription from ProjectDetails where projid=4"
        Dim command As New SqlCommand
        Try
            OpenDB(query, command)
            Dim retval As Byte() = command.ExecuteScalar()
            CloseDB()
            byes = retval
            Return "OK"
        Catch ex As Exception
            Return "~" + ex.Message
        End Try
    End Function

    '----------------------------SUB /STage Entry----------------------------
    Public Shared Function GetStageProcessDetails(ByVal Type As String, ByVal ID_Include As Boolean, ByRef result As String) As DataTable
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim dt As DataTable = New DataTable()
        Dim ds As DataSet = New DataSet
        Dim da As New SqlDataAdapter
        If Type = "Stages" Then
            Try
                If ID_Include = False Then
                    query = "select StageName as Stage,StageAlias as ALIAS,StageDescription as Description from StageMaster"
                Else
                    query = "select StageID ,StageName as Stage,StageAlias as ALIAS,duration as 'Standard_Duration(Days)',StageDescription as Description from StageMaster"
                End If
                delay(150)
                OpenDB(query, command)
                da.SelectCommand = command
                command.ExecuteNonQuery()
                da.Fill(ds)
                CloseDB()
                If ds.Tables(0).Rows.Count > 0 Then
                    dt = ds.Tables(0)
                    result = "OK"
                Else
                    result = "~NO DATA Found"
                    Return Nothing
                End If
            Catch ex As Exception
                result = "~" + ex.Message
                Return Nothing
            End Try

        ElseIf Type = "Sub-Process" Then
            Try
                If ID_Include = False Then
                    query = "select SUBName AS SubProcess,SUBAlias as ALIAS ,SUBDesc as Description from SUBMaster"
                Else
                    query = "select SUBID, SUBName AS SubProcess,SUBAlias as ALIAS,duration as 'Standard_Duration(Days)',SUBDesc as Description from SUBMaster"
                End If
                delay(150)
                OpenDB(query, command)
                da.SelectCommand = command
                command.ExecuteNonQuery()
                da.Fill(ds)
                CloseDB()
                If ds.Tables(0).Rows.Count > 0 Then
                    dt = ds.Tables(0)
                    result = "OK"
                Else
                    result = "~NO DATA Found"
                    Return Nothing
                End If
            Catch ex As Exception
                result = "~" + ex.Message
                Return Nothing
            End Try
        Else
            result = "~ INVALID OPTION"
            Return Nothing
        End If
        Return dt
    End Function
    Public Shared Function InsertStageSub(type As String, ByRef result As String) As Boolean
        Dim query As String = ""
        Dim SQLTRANS As SqlTransaction = Nothing
        If type = "Stages" Then
            query = "insert into StageMaster(StageName,StageDescription,StageAlias,DURATION) values('" + PROCESSNAME + "','" + PROCESSDESC + "','" + PROCESSALIAS + "','" + DURATION + "')"

        ElseIf type = "Sub-Process" Then
            query = "insert into SUBMaster(SUBName,SUBDesc,SUBAlias,DURATION) values('" + PROCESSNAME + "','" + PROCESSDESC + "','" + PROCESSALIAS + "','" + DURATION + "')"
        Else
            result = "Improper Selection!"
            Return False
        End If

        Dim command As New SqlCommand
        Try
            delay(150)
            OpenDB(query, command)
            SQLTRANS = command.Transaction
            Dim retint As Integer = command.ExecuteNonQuery()
            If retint = 1 Then
                SQLTRANS.Commit()
                CloseDB()
                result = "OK"
            ElseIf retint > 1 Then

                SQLTRANS.Rollback()
                CloseDB()
                result = "~Error: More than 1 records wwere going to be added."
                Return False
            Else
                SQLTRANS.Rollback()
                CloseDB()
                result = "~Failed: No records Added,Please check server connection, contact admin."
                Return False
            End If

        Catch ex As Exception
            CloseDB()
            result = "~" + ex.Message
            Return False
        End Try
        Return True
    End Function

    '-----------------------Task Mapping------------------------------------------
    Public Shared Function GetStageMapping(ByVal project As String, Optional ByVal mapid As Boolean = False, Optional fathermap As Boolean = False) As DataTable
        Dim query As String = ""
        Dim command As New SqlCommand
        '0  StageID
        '1  StageName
        '2  ProjID
        '3  ProjName
        '4  Remarks
        '5  StartDate
        '6  EndDate
        '7  CreatedOn
        '8  CreatedBy
        '9 AssignedTo
        '10  Sequence
        If String.IsNullOrEmpty(project) Then
            Return Nothing
        End If

        Try
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            If mapid = True Then
                query = "SELECT MapID,StageID,StageName,ProjID,ProjName,Remarks,StartDate,EndDate,CreatedOn,CreatedBy,AssignedTo,Sequence FROM TaskMapping where projname='" + project + "' and status=0 and mapidfather=0 order by sequence"
            Else
                If fathermap = True Then
                    query = "SELECT MapIDFather as StageID,StageName,ProjID,ProjName,Remarks,StartDate,EndDate,CreatedOn,CreatedBy,dbo.GETUSERNAME(AssignedTo),CONVERT(varchar(3),Sequence) FROM TaskMapping where projname='" + project + "' and status=0 and mapidfather<>0 order by sequence"
                Else
                    query = "SELECT StageID,StageName,ProjID,ProjName,Remarks,StartDate,EndDate,CreatedOn,CreatedBy,AssignedTo,Sequence FROM TaskMapping where projname='" + project + "' and status=0 and mapidfather=0 order by sequence"
                End If
            End If
            delay(150)
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
        End Try
    End Function
    Public Shared Function GetSubMapping(ByVal project As String) As DataTable
        Dim query As String = ""
        Dim command As New SqlCommand

        If String.IsNullOrEmpty(project) Then
            Return Nothing
        End If

        Try
            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            query = "SELECT b.SUBName as SubPRocess,b.SUBID as SubProcessID,dbo.getBIDNAME(BID) as StageName,dbo.getprojectname(ProjID) as ProjectName, Remarks,  CreatedBy,TargetDate,AssignedTo FROM SubMapping a inner join SUBMaster b on a.SUBID=b.SUBID where dbo.getprojectname(ProjID)='" + project + "' and status=0  order by StageName"
            delay(150)
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
        End Try
    End Function
    Public Shared Function savestagemapping(ByVal dt As DataTable, projectName As String) As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim SQLTRANS As SqlTransaction = Nothing
        If dt Is Nothing Then
            Return "~Error: Empty DataTable"
        End If
        'If dt.Rows.Count = 0 Then
        '    Return "~Error: No input data"
        'End If
        Try
            query = "SP_StageMapping"
            delay(150)
            OpenDB(query, command)
            SQLTRANS = command.Transaction
            CreateParameter(2)
            AddParamaeter(0, "@UMapping", dt, SqlDbType.Structured)
            AddParamaeter(1, "@projName", projectName)
            command.CommandType = CommandType.StoredProcedure
            Dim val = command.ExecuteScalar()

            If val = "CRUD COMPLETE" Then
                SQLTRANS.Commit()
                CloseDB()
                Return "Modification  : OK"
            ElseIf val = "UPDATED" Then
                SQLTRANS.Commit()
                CloseDB()
                Return "Partial Update : OK"
            Else
                SQLTRANS.Rollback()
                CloseDB()
                Return "FAIL"
            End If
        Catch ex As Exception
            SQLTRANS.Rollback()
            CloseDB()
            If ex.Message.Contains("Violation of PRIMARY KEY constraint") Then
                Return "~Cannot Add Duplicate Stages; Inner Error " + ex.Message
            End If
            Return "~" + ex.Message
        End Try
    End Function
    Public Shared Function saveSubmapping(ByVal dt As DataTable, projectName As String) As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim SQLTRANS As SqlTransaction = Nothing
        If dt Is Nothing Then
            Return "~Error: Empty DataTable"
        End If
        'If dt.Rows.Count = 0 Then
        '    Return "~Error: No input data"
        'End If
        Try
            query = "SP_SubMapping"
            delay(100)
            OpenDB(query, command)
            SQLTRANS = command.Transaction
            CreateParameter(2)
            AddParamaeter(0, "@USubMapping", dt, SqlDbType.Structured)
            AddParamaeter(1, "@Projname", projectName)
            command.CommandType = CommandType.StoredProcedure
            Dim val = command.ExecuteScalar()

            If val = "CRUD COMPLETE" Then
                SQLTRANS.Commit()
                CloseDB()
                Return "Modification  : OK"
            ElseIf val = "UPDATED" Then
                SQLTRANS.Commit()
                CloseDB()
                Return "Partial Update : OK"
            Else
                SQLTRANS.Rollback()
                CloseDB()
                Return "FAIL"
            End If
        Catch ex As Exception
            SQLTRANS.Rollback()
            CloseDB()
            If ex.Message.Contains("Violation of PRIMARY KEY constraint") Then
                Return "~Cannot Add Duplicate Stages; Inner Error " + ex.Message
            End If
            Return "~" + ex.Message
        End Try
    End Function


    '------------------------File stage Sub Process Entry----------------------------
    Public Shared Function GetAssignedProject(ByVal TYPE As String) As DataTable
        Dim query As String = ""
        Dim command As SqlCommand = Nothing
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Try
            If TYPE = "Sub-Stage" Then
                query = "select  distinct a.ProjName,convert(varchar(52),a.projid),a.AssignedTo from TaskMapping a inner join ProjectMaster b on a. ProjID=b.ProjID  where MapIDFather<>0 and AssignedTo='" + UserID + "' and a.status>=0 and b.Status=1"
            Else
                query = "select  distinct a.ProjName,convert(varchar(52),a.projid),a.AssignedTo from TaskMapping a inner join ProjectMaster b on a. ProjID=b.ProjID  where MapIDFather=0 and AssignedTo='" + UserID + "' and  a.status>=0  and b.Status>=1"

            End If
            delay(100)
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)

            CloseDB()
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Return ds.Tables(0)
                Else
                    Return Nothing
                End If
            End If
        Catch ex As Exception
            CloseDB()
            MsgBox(ex.Message)
            Return Nothing
        End Try
    End Function
    Public Shared Function GetassignedDetails(ByVal project As String, ByVal owner As String, Optional ByVal Type As String = "SUB") As DataTable
        Dim query As String
        If Type = "SUB" Then
            query = "Select b.StageName As SubName,b.StageID As SubID,a.StageName,a.StageID,convert(Date,b.StartDate) As StartDate,CONVERT(Date,b.ActualEndDate) as ActualEndDate,b.Gap,b.Remarks,CONVERT(Date,b.EndDate) As EndDate,b.Sequence as Priority ,DBO.GETSTATUS(b.status) as status from TaskMapping a 
                    inner Join TaskMapping b on a.MapID=b.MapIDFather
                    where a.ProjName ='" + project + "' and b.AssignedTo='" + owner + "'  order by a.Sequence,b.Sequence desc" 'and a.Status<>2 AND b.Status<>2
        ElseIf Type = "STAGE" Then
            query = "select StageName, StageID, StartDate, EndDate, ActualEndDate, Gap, DBO.GETSTATUS(status) As status From TaskMapping Where projname ='" + project + "' and MapIDFather=0 AND AssignedTo='" + owner + "';"
        Else
            Return Nothing
            Exit Function
        End If

        Dim command As SqlCommand = Nothing
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        Try
            delay(100)
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count = 0 Then
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
            Return ds.Tables(0)
        Catch ex As Exception
            MsgBox("~" + ex.Message)
            Return Nothing
        End Try
    End Function
    Public Shared Function UploadDoc(ByVal filename As String, ByVal filepath As String) As String
        Dim query As String
        Dim command As New SqlCommand
        Dim trans As SqlTransaction = Nothing
        Dim streamID
        Try
            query = "INSERT INTO PMDDOCUMENT (NAME,file_stream) SELECT '" + filename + "',* FROM OPENROWSET(BULK '" + filepath + "',SINGLE_BLOB) AS FILETABLE select stream_id from PMDDOCUMENT where name='" + filename + "';"
            delay(100)
            OpenDB(query, command)
            trans = command.Transaction
            streamID = command.ExecuteScalar()
            If streamID.ToString <> "" Then
                trans.Commit()
                CloseDB()
            Else
                trans.Rollback()
                CloseDB()
                Return "~Cannot Upload the Document"
            End If

        Catch ex As Exception
            trans.Rollback()
            CloseDB()
            Return "~" + ex.Message
        End Try
        Return streamID.ToString
    End Function
    Public Shared Function CHECKCURRENTPROCESS(ByVal projectname As String, ByVal CurrentStage As String, Optional ByRef flag As String = "") As Boolean
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim value As String
        Dim stage As String
        Try
            query = " if not exists(select case
                      when status=1 THEN 'NC'
                      when status=2 THEN 'P'
                      when status=3 THEN 'Y'
                      else 'N'
                      end +'|'+stagename as LastStatus
                      from TaskMapping where Sequence=dbo.GetSeqTask('" + CurrentStage + "',ProjName)-1 and ProjName='" + projectname + "' and MapIDFather=0)
                      BEGIN
                      select 'F' as LastStatus
                      END

                      ELSE
                      BEGIN
                       select case
                      when status=1 THEN 'NC'
                      when status=2 THEN 'P'
                      when status=3 THEN 'Y'
                      else  'N'
                      end +'|'+stagename as LastStatus  from TaskMapping where Sequence=dbo.GetSeqTask('" + CurrentStage + "',ProjName)-1 and ProjName='" + projectname + "' and MapIDFather=0
                      END"
            delay(100)
            OpenDB(query, command)
            Dim valsplit = command.ExecuteScalar()
            If valsplit <> "F" Then
                value = valsplit.ToString.Split("|")(0)
                stage = valsplit.ToString.Split("|")(1)
            Else
                value = valsplit
                stage = ""

            End If

            If value = "F" Then
                flag = "Allowed" + vbCrLf + vbCrLf + "Reason: First Stage"
                CloseDB()

            ElseIf value = "Y" Then
                flag = "Allowed" + vbCrLf + vbCrLf + "Reason: Last Stage Verified"
                CloseDB()
            ElseIf value = "P" Then
                flag = "Not Allowed" + vbCrLf + vbCrLf + $"Reason: Last Stage '{stage}' Verification Pending"
                CloseDB()
                Return False
            ElseIf value = "NC" Then
                flag = "Not Allowed" + vbCrLf + vbCrLf + "Reason: Sub Processes Pending"
                CloseDB()
                Return False
            ElseIf value = "N" Then
                flag = "Not Allowed" + vbCrLf + vbCrLf + $"Reason: Previous Stage '{stage}' has not been Started Yet!!"
                CloseDB()
                Return False
            Else
                flag = "Not Allowed" + vbCrLf + vbCrLf + "Reason: Abnormal Error, Contact admin!"
                CloseDB()
                Return False
            End If
        Catch ex As Exception
            CloseDB()
            MsgBox(ex.Message)
            Return False
        End Try
        Return True
    End Function
    Public Shared Function UpdateTask(ByVal project As String, ByVal stagename As String, ByVal subproecess As String, ByVal enddate As Date, ByVal gap As Integer, Optional ByVal reason As String = "", Optional ByVal description As String = "", Optional ByVal guid As String = "", Optional filename As String = "") As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim transaction As SqlTransaction = Nothing
        Dim retval As String
        Try
            query = "SP_UpdateStatus"
            delay(150)
            OpenDB(query, command)
            CreateParameter(10)
            AddParamaeter(0, "@projname", project)
            AddParamaeter(1, "@SubProcess", subproecess)
            AddParamaeter(2, "@STAGENAME", stagename)
            AddParamaeter(3, "@reason", reason)
            AddParamaeter(4, "@filename", filename)
            AddParamaeter(5, "@GUID", guid)
            AddParamaeter(6, "@Description", description)
            AddParamaeter(7, "@uploadby", UserID)
            AddParamaeter(8, "@ACTUALENDDATE", enddate, SqlDbType.Date)
            AddParamaeter(9, "@GAP", gap, SqlDbType.Int)
            command.CommandType = CommandType.StoredProcedure
            transaction = command.Transaction
            retval = command.ExecuteScalar()
            If retval.Contains("~") Then
                transaction.Rollback()
                CloseDB()
                Return retval
            ElseIf retval.Contains("SUCCESS") Then
                transaction.Commit()
                CloseDB()
                Return retval
            Else
                transaction.Rollback()
                CloseDB()
                Return "~The Response didn't recieve Success status please check!!" + vbCrLf + vbCrLf + " Reason : Abnormal Error"
            End If
        Catch ex As Exception
            If transaction IsNot Nothing Then
                transaction.Rollback()
                CloseDB()
            End If
            Return "~" + ex.Message
        End Try
        Return ""
    End Function
    Public Shared Function UpdateStageTask(ByVal project As String, ByVal stagename As String, ByVal enddate As Date, ByVal gap As Integer, ByVal result As Integer, Optional ByVal reason As String = "", Optional ByVal description As String = "", Optional ByVal guid As String = "", Optional filename As String = "", Optional resultmessage As String = "", Optional ByVal SUBPROCESSRREJECT As String = "") As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim transaction As SqlTransaction = Nothing
        Dim retval As String
        Try
            query = "SP_VerifyStage"
            delay(150)
            OpenDB(query, command)
            CreateParameter(12)

            AddParamaeter(0, "@projname", project)
            AddParamaeter(1, "@Stagename", stagename)
            AddParamaeter(2, "@actualadateend", enddate, SqlDbType.Date)
            AddParamaeter(3, "@filename", filename)
            AddParamaeter(4, "@streamid", guid)
            AddParamaeter(5, "@gap", gap, SqlDbType.Int)
            AddParamaeter(6, "@description", description)
            AddParamaeter(7, "@uploadby", UserID)
            AddParamaeter(8, "@remarks", reason)
            AddParamaeter(9, "@RESULT", result, SqlDbType.Int)
            AddParamaeter(10, "@result_message", resultmessage)
            AddParamaeter(11, "@SubTaskReject", SUBPROCESSRREJECT)
            command.CommandType = CommandType.StoredProcedure
            transaction = command.Transaction
            retval = command.ExecuteScalar()
            If retval.Contains("~") Then
                transaction.Rollback()
                CloseDB()
                Return retval
            ElseIf retval.Contains("ACCEPTED") Then
                transaction.Commit()
                CloseDB()
                Return retval
            ElseIf retval.Contains("REJECTED") Then
                transaction.Commit()
                CloseDB()
                Return retval
            Else
                transaction.Rollback()
                CloseDB()
                Return "~The Response didn't recieve Success status please check!!" + vbCrLf + vbCrLf + " Reason : Abnormal Error"
            End If
        Catch ex As Exception
            If transaction IsNot Nothing Then
                transaction.Rollback()
                CloseDB()
            End If
            Return "~" + ex.Message
        End Try
        Return ""
    End Function
    Public Shared Function GetAssignedSubbMapping(ByVal projname As String, ByVal stage As String) As DataTable
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        query = "select a.StageName,
                a.StageID,
                a.StartDate,
                a.EndDate,
                a.ActualEndDate,
                a.gap,a.Remarks,
                isnull(b.Description,'') as Description,
                isnull(b.FileName,'') as FileName,
                isnull(b.StreamID,'00000000-0000-0000-0000-000000000000') as StreamID,
                DBO.GETSTATUS(STATUS) as status ,
		        a.AssignedTo,
		        dbo.getusername(a.AssignedTo) as EmployeeName
                from TaskMapping a 
                left join PMDUploadExpand b on a.GID=b.GID and a.MapID=b.TASKID 
                where ProjName='" + projname + "' and MapIDFather=dbo.GETMAPID('" + stage + "',ProjName)"
        Try
            delay(150)
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds.Tables(0) IsNot Nothing Then
                If ds.Tables(0).Rows.Count = 0 Then
                    Return Nothing
                End If
            End If
        Catch ex As Exception
            CloseDB()
            Return Nothing
        End Try
        Return ds.Tables(0)
    End Function
    Public Shared Function GetMappingAttachement(ByVal filename As String, ByVal streamid As String, ByRef byes As Byte()) As String
        Dim query As String = "select file_stream from PMDDOCUMENT where stream_id='" + streamid + "' and name='" + filename + "' "
        Dim command As New SqlCommand
        Try
            OpenDB(query, command)
            Dim retval As Byte() = command.ExecuteScalar()
            CloseDB()
            If retval Is Nothing Then
                Return "~Empty Result!"
            End If
            If retval.Length > 0 Then
                byes = retval
                Return "OK"
            Else
                Return "~FAIL"
            End If

        Catch ex As Exception
            Return "~" + ex.Message
        End Try
    End Function

    '-------------------------START IMMEDIATE PROJECT---------------------------
    Public Shared Function IMEEDIATEGET(Optional ByVal USERTYPE As String = "") As DataTable
        Dim QUERY As String
        Dim COMMAND As New SqlCommand
        Dim DA As New SqlDataAdapter
        Dim DS As New DataSet
        Try
            QUERY = "Select  a.ProjName,convert(date,b.StartDate) as StartDate,dbo.GETSTATUS(Status) As Status,b.AdminName from ProjectMaster a inner join ProjectDetails b On a.ProjID=b.ProjID"
            If USERTYPE <> "ADMIN" Then
                QUERY = QUERY + " where  b.AdminID='" + UserID + "'"
            End If
            OpenDB(QUERY, COMMAND)
            DA.SelectCommand = COMMAND
            COMMAND.ExecuteNonQuery()
            DA.Fill(DS)
            CloseDB()
            If DS IsNot Nothing Then
                If DS.Tables(0).Rows.Count > 0 Then
                    Return DS.Tables(0)
                End If
            End If

        Catch ex As Exception
            CloseDB()
            Return Nothing
        End Try
        Return Nothing
    End Function
    Public Shared Function UpdateProjStatus(ByVal projname As String) As List(Of String)
        Dim query As String = ""
        Dim liss As New List(Of String)
        Dim command As New SqlCommand
        Dim trans As SqlTransaction = Nothing
        Try
            query = "SP_Update_ProjStatus"
            OpenDB(query, command)
            trans = command.Transaction
            CreateParameter(2)
            AddParamaeter(0, "@PROJNAME", projname)
            AddParamaeter(1, "@MODIFIEDBY", UserID)
            command.CommandType = CommandType.StoredProcedure
            Dim reader As SqlDataReader = command.ExecuteReader()
            While reader.Read
                liss.Add(reader.Item(0).ToString)
            End While
            reader.Dispose()
            ' liss.Insert(0, result)
            If liss.Contains("SUCCESS") Then
                trans.Commit()
                CloseDB()
                Return liss
            Else
                trans.Rollback()
                CloseDB()
            End If
        Catch ex As Exception
            trans.Rollback()
            CloseDB()
            liss.Insert(0, "~")
            liss.Insert(1, ex.Message)
            Return liss
        End Try
        liss.Insert(0, "~")
        liss.Insert(1, "Projects Not having Records of Stage and Sub Process Mapping are as follows")
        Return liss
    End Function
End Class
Public Class HolidayEntry
    Public Shared Function SaveHolidayRecords(ByVal holidayreason As String, ByVal startdate As Date, ByVal enddate As Date, ByVal year As Integer, ByVal duration As Integer) As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim trans As SqlTransaction = Nothing
        Dim result As String = ""
        Try
            query = "INSERT INTO HolidayEntry(HolidayName, startdate, enddate, Hyear, duration,Createdon,createdby) VALUES('" + holidayreason + "','" + startdate + "','" + enddate + "'," + year.ToString + "," + duration.ToString + ",getdate()," + UserID + ")"
            OpenDB(query, command)
            trans = command.Transaction
            result = command.ExecuteNonQuery()
            If result = 1 Then
                trans.Commit()
                CloseDB()
            Else
                trans.Rollback()
                CloseDB()
                Return "FAILED TO SAVE RECORDS!!, PLEASE CHECK CONNECTION."
            End If

        Catch ex As Exception
            If trans IsNot Nothing Then
                trans.Rollback()
            End If
            CloseDB()
            Return "~" + ex.Message
        End Try
        Return "SUCCESSFUL"
    End Function
    Public Shared Function GetHolidays(ByVal yearset As String) As DataTable
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        Try
            query = "Select holidayname,startdate,enddate,hyear,duration from holidayentry where Hyear In (" + yearset +") and holidayname<>'' and status=1  order by startdate,duration"
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            If ds.Tables(0) IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Return ds.Tables(0)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
        End Try
        Return Nothing
    End Function
End Class
Public Class RoleSettings
    Public Shared Function GetGroupRoleInfo() As DataSet
        Dim query As String = ""
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim command As New SqlCommand
        Try
            query = "select RoleName,RoleCategory,RoleDescription from SysRoleInfo;SELECT RTRIM(CATNAME),CONVERT(VARCHAR(200),CATID) FROM ROLECATEGORYINFO "
            If UserType <> "SUPERADMIN" Then
                query += "where Catid<=dbo.RoleCategoryID('" + UserType + "') "
            Else
                query += "ORDER BY CATID;"
            End If
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds Is Nothing Then
                Return Nothing
            End If
            If ds.Tables.Count > 0 Then
                Return ds
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    Public Shared Function SetGroupRoleInfo(ByVal rolename As String, ByVal rolecat As String, Optional ByVal roledescript As String = "") As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim trans As SqlTransaction = Nothing
        Try
            query = "INSERT INTO SysRoleInfo(RoleName,RoleCategory,RoleDescription)values('" + rolename + "','" + rolecat + "','" + roledescript + "')"
            OpenDB(query, command)
            trans = command.Transaction
            Dim result = command.ExecuteNonQuery()
            If result = 1 Then
                trans.Commit()
                CloseDB()
            Else
                trans.Rollback()
                CloseDB()
                Return "~Unable to Save the details, Please check the connection!!"
            End If

        Catch ex As Exception
            If trans IsNot Nothing Then
                trans.Rollback()
            End If
            CloseDB()
            Return "~" + ex.Message
        End Try
        Return "OK"
    End Function
    Public Shared Function GetUserRoleInfo(ByVal checkuserlevel As Boolean, ByVal getbothresult As Boolean, Optional ByVal usertype As String = "", Optional ByRef result As String = "") As DataSet
        Dim Query1 As String = ""
        Dim Query2 As String = ""
        Dim command As New SqlCommand
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Try
            Query1 = "select modulename,UserLevel,Moduledescription from Module_Level_Info "
            If checkuserlevel = True Then
                If usertype = "" Then
                    result = "~UserType Empty"
                    Return Nothing
                End If
                Query1 += "where catid<=dbo.RoleCategoryID('" + usertype + "') order by Userlevel;"
            Else
                    Query1 += ";"
            End If
            If getbothresult = True Then
                Query2 = "select B.NAME,B.EmpCode,A.RoleName,A.RoleCategory from SysRoleInfo a inner join AccountMaster b on a.RoleName=b.Account_Type; "
                Query1 += Query2
            End If
            OpenDB(Query1, command)
            If command IsNot Nothing Then da.SelectCommand = command

            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds Is Nothing Then
                result = "~No Data Recieved"
                Return Nothing
            End If
            If ds.Tables.Count = 0 Then
                result = "~No Records Found"
                Return Nothing
            End If
        Catch ex As Exception
            result = "~" + ex.Message
            Return Nothing
        End Try
        Return ds
    End Function
    Public Shared Function UpdateuserRoles(ByVal ACCOUNTTYPE As String, ByVal USERNAME As String, ByVal EMPLCODE As String) As String
        Dim query As String = ""
        Dim command As New SqlCommand
        Dim TRANS As SqlTransaction = Nothing
        Try
            query = "UPDATE AccountMaster SET Account_Type='" + ACCOUNTTYPE + "' WHERE NAME='" + USERNAME + "' AND EmpCode='" + EMPLCODE + "'"
            OpenDB(query, command)
            TRANS = command.Transaction
            Dim RESULT = command.ExecuteNonQuery()
            If RESULT = 1 Then
                TRANS.Commit()
                CloseDB()
            Else
                TRANS.Rollback()
                CloseDB()
                Return "~Unable to commit the changes!, Please check the details again"
            End If
        Catch ex As Exception
            If TRANS IsNot Nothing Then
                TRANS.Rollback()
            End If
            CloseDB()
            Return "~" + ex.Message
        End Try
        Return "SUCCESSFUL"
    End Function
    Public Shared Function GetRoleModule() As List(Of String)
        Dim query As String
        Dim command As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet
        Dim liss As New List(Of String)
        Try
            query = "select ModuleName from module_level_info where catid>dbo.RolecategoryID('" + UserType + "') order by catID"
            OpenDB(query, command)
            da.SelectCommand = command
            command.ExecuteNonQuery()
            da.Fill(ds)
            CloseDB()
            If ds Is Nothing Then Return Nothing
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim dt As New DataTable
                    dt = ds.Tables(0)
                    Dim qry = From dr In dt Select dr.Field(Of String)(0)
                    liss = qry.ToList
                End If
            End If
        Catch ex As Exception
            CloseDB()
            Return Nothing
        End Try
        Return liss
    End Function
End Class