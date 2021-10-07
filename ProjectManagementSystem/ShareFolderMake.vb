Imports System
Imports System.Management
Imports System.IO
Imports System.Security.Principal
Imports System.Security.AccessControl

Public Class ShareFolderMake

    Public Shared Function ShareFolder(ByVal FolderPath As String, ByVal ShareName As String, ByVal Description As String) As String
        Dim strSharePath As String = FolderPath
        Dim strShareName As String = ShareName
        Dim strShareDesc As String = Description
        Dim msg As String = ""
        Try

            Directory.CreateDirectory(strSharePath)
            Dim ManagementClass As ManagementClass = New ManagementClass("Win32_Share")
            Dim inputParameters As ManagementBaseObject = ManagementClass.GetMethodParameters("Create")
            Dim outputParameters As ManagementBaseObject
            inputParameters("Description") = strShareDesc
            inputParameters("Name") = strShareName
            inputParameters("Path") = strSharePath
            inputParameters("Type") = 0 ' disk drive 
            inputParameters("MaximumAllowed") = Nothing
            inputParameters("Access") = Nothing
            inputParameters("Password") = Nothing
            outputParameters = ManagementClass.InvokeMethod("Create", inputParameters, Nothing)
            PubVals.PUBLICSHAREPATH = "\\" + System.Net.Dns.GetHostName + "\" + "PMDUPLOAD"
            Dim sec = Directory.GetAccessControl(PubVals.PUBLICSHAREPATH)
            Dim everyone As System.Security.Principal.SecurityIdentifier = New System.Security.Principal.SecurityIdentifier(WellKnownSidType.WorldSid, Nothing)
            sec.AddAccessRule(New FileSystemAccessRule(everyone, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow))
            Directory.SetAccessControl(PubVals.PUBLICSHAREPATH, sec)
            'Process.Start("CMD.exe", "NET SHARE ")
            If Int32.Parse(outputParameters.Properties("ReturnValue").Value) <> 0 Then
                msg = "There is a problem while sharing the directory."
                Throw New Exception("There is a problem while sharing the directory.")
            Else
                msg = ("Share Folder has been created with the name :" + strShareName)
            End If



        Catch ex As Exception

            msg = (ex.Message.ToString())
        End Try
        Return msg
    End Function



End Class
