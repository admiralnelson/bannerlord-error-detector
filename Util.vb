
Imports System.IO
Imports System.Security.Cryptography
Imports Newtonsoft.Json

Module Util
    Public AllowInDebugger As Boolean = True
    Public CatchOnApplicationTick As Boolean = True
    Public CatchOnMissionScreenTick As Boolean = True
    Public CatchOnFrameTick As Boolean = True
    Public CatchTick As Boolean = True
    Public SaveLogPath As String = ""
    Public Sub ReadConfig()
        Try
            Dim data = File.ReadAllText("..\..\Modules\BetterExceptionWindow\config.json")
            Dim configData = JsonConvert.DeserializeObject(data)
            AllowInDebugger = configData("AllowInDebugger")
            CatchOnApplicationTick = configData("CatchOnApplicationTick")
            CatchOnMissionScreenTick = configData("CatchOnMissionScreenTick")
            CatchOnFrameTick = configData("CatchOnFrameTick")
            CatchTick = configData("CatchTick")
            SaveLogPath = configData("SaveLogPath")
        Catch ex As Exception
            Console.WriteLine("unable to read the config. It's ok")
        End Try
    End Sub
    Public Function CalculateMD5(filename As String) As String
        Dim checksum = MD5.Create()
        Dim stream = File.OpenRead(filename)
        Dim hash = checksum.ComputeHash(stream)
        Return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()
    End Function
End Module
