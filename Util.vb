
Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Xml
Imports Microsoft.VisualBasic.FileIO
Imports Newtonsoft.Json
Imports TaleWorlds.Core
Imports System.Runtime.CompilerServices
Imports System.IO.Compression

Public Module Util
    Public AllowInDebugger As Boolean = True
    Public CatchOnApplicationTick As Boolean = True
    Public CatchOnMissionScreenTick As Boolean = True
    Public CatchOnFrameTick As Boolean = True
    Public CatchTick As Boolean = True
    Public CatchComponentBehaviourTick As Boolean = True
    Public CatchGlobalTick As Boolean = True
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
            CatchComponentBehaviourTick = configData("CatchComponentBehaviourTick")
            CatchGlobalTick = configData("CatchGlobalTick")
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

    Public Function ValidateXmlFile(filename As String) As Boolean
        Dim doc As New XmlDocument()
        Try
            doc.Load(New StreamReader(filename))
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Sub Print(str As String)
        InformationManager.DisplayMessage(New InformationMessage(str))
        Console.WriteLine(str)
    End Sub
    Public Function GetAssembliesData() As List(Of Assembly)
        Dim asm = AppDomain.CurrentDomain.GetAssemblies()
        Dim out As New List(Of Assembly)
        For Each x In asm
            Try
                If Not x.Location.ToLower().Contains("Mount & Blade II Bannerlord".ToLower()) Then
                    Continue For
                End If
                out.Add(x)
            Catch ex As Exception

            End Try
        Next
        Return out
    End Function

    Public Function CheckIsAssemblyLoaded(dllFilename As String)
        Dim asm As Assembly() = AppDomain.CurrentDomain.GetAssemblies()
        For Each x In asm
            Try

                If Path.GetFileName(x.Location) = dllFilename Then
                    Return True
                End If
            Catch ex As Exception
            End Try
        Next
        Return False
    End Function
    Public Sub Sleep(t As Integer)
        Threading.Thread.Sleep(t)
    End Sub
    Public Function GetBanenrlordProgramDataPath()
        Dim p = SpecialDirectories.AllUsersApplicationData + "\..\..\..\Mount and Blade II Bannerlord\"
        Return Path.GetFullPath(p) + "\"
    End Function
    Public Function ToJson(o As Object)
        Return JsonConvert.SerializeObject(o)
    End Function
    Public Sub MsgBoxBannerlord(title As String, Optional message As String = "",
                                Optional callbackOk As Action = Nothing,
                                Optional callbackCancel As Action = Nothing,
                                Optional okMessage As String = "Accept",
                                Optional cancelMessage As String = "Cancel")
        If callbackOk Is Nothing Then
            InformationManager.ShowInquiry(New TaleWorlds.Library.InquiryData(
                                           title,
                                           message,
                                           True, False,
                                           okMessage, "",
                                           Sub()

                                           End Sub,
                                           Sub()

                                           End Sub))
            Exit Sub
        End If
        If callbackOk IsNot Nothing And
           callbackCancel Is Nothing Then
            InformationManager.ShowInquiry(New TaleWorlds.Library.InquiryData(
                                           title,
                                           message,
                                           True, False,
                                           okMessage, "",
                                           Sub()
                                               callbackOk()
                                           End Sub,
                                           Sub()

                                           End Sub))
            Exit Sub
        End If
        InformationManager.ShowInquiry(New TaleWorlds.Library.InquiryData(
                                           title,
                                           message,
                                           True, True,
                                           okMessage, cancelMessage,
                                           Sub()
                                               callbackOk()
                                           End Sub,
                                           Sub()
                                               callbackCancel()
                                           End Sub))
    End Sub
    Public Sub RestartAndAttachDnspy()
        Dim PsBannerlord As New ProcessStartInfo()
        Dim currentpath = Directory.GetCurrentDirectory()
        PsBannerlord.Arguments = "/C cd """ & currentpath &
                         """ && ping 127.0.0.1 -n 2 && " &
                         "start TaleWorlds.MountAndBlade.Launcher.exe --disablebew && " &
                         " ping 127.0.0.1 -n 3 && " &
                         DnspyDir & "dnSpy.exe --process-name TaleWorlds.MountAndBlade.Launcher.exe"
        PsBannerlord.WindowStyle = ProcessWindowStyle.Hidden
        PsBannerlord.CreateNoWindow = True
        PsBannerlord.FileName = "cmd.exe"
        Process.Start(PsBannerlord)
        KillGame()
    End Sub
    Public Sub KillGame()
        Dim pid = Process.GetCurrentProcess().Id
        Dim proc As Process = Process.GetProcessById(pid)
        proc.Kill()
    End Sub

    Public ReadOnly Version As String = "BetterExceptionWindow version 4.0.0"
    Public ReadOnly Commit As String = My.Resources.CurrentCommit
    Public ReadOnly DnspyDir As String =
        "..\..\Modules\BetterExceptionWindow\bin\Win64_Shipping_Client\dnspy\"
    Public ReadOnly DnspyManifest As String =
        "..\..\Modules\BetterExceptionWindow\dnSpyManifest.json"
    Public ReadOnly BewDir As String =
        "..\..\Modules\BetterExceptionWindow\"
    Public ReadOnly BewTemp As String =
        "..\..\Modules\BetterExceptionWindow\Temp\"
    Public ReadOnly BewBinDir As String =
        "..\..\Modules\BetterExceptionWindow\bin\Win64_Shipping_Client\"
End Module