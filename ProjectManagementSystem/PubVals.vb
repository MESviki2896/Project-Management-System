Imports System
Imports System.IO
Imports LOGANDINI

Public Class PubVals
    Shared resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PMSMain))
    Public Shared IniFilePath As String = Application.StartupPath + "/data/settings.txt"
    Public Shared DBlogin As String
    Public Shared UserID As String
    Public Shared UserType As String
    Public Shared Password As String
    Public Shared AppVersion As String = "V1.0.1"
    Public Shared ApplicationVersion As String = "Project Management Database " + AppVersion
    Public Shared PUBLICSHAREPATH As String = "\\" + System.Net.Dns.GetHostName + "\" + "PMDUPLOAD"
    Public Shared SERVERHAREPATH As String = FucntionModules.Getini("Parameter", "ShareServer")
    Public Shared Dicon As Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
    'Public Shared SERVERID As String

End Class
Public Class SQLTransact
    Public Shared Query As String
End Class
Public Class PVCreateUser
    Public Shared NewName As String
    Public Shared NewCode As String
    Public Shared NewPassword As String
    Public Shared NewDepartment As String
    Public Shared NewEmail As String
End Class
Public Class PVCreateProject
    Public Shared ProjecName As String
    Public Shared ModelName As String
    Public Shared ModelAdmin As String
    Public Shared CreatedBy As String
    Public Shared ModelSpecification As Byte()
    Public Shared ModelDescription As Byte()
    Public Shared ProjStartDate As String
    Public Shared DURATION As String



    'wE WILL ADD THE PROCESS DETAILS HERE ITSELF, SO THAT WE DONT NEED TO CREATE A NEW CLASS FOR IT.
    Public Shared PROCESSNAME As String
    Public Shared PROCESSDESC As String
    Public Shared PROCESSALIAS As String
    Public Shared CONSTALLstartRANG As Date = DateAndTime.DateSerial(Now.Year - 5, 1, 1)
    Public Shared CONSTALLENDRANG As Date = DateAndTime.DateSerial(Now.Year + 5, 12, 31)
    Public Shared Datesarr() As Date




End Class
