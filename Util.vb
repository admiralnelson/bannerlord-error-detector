
Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Xml
Imports Microsoft.VisualBasic.FileIO
Imports Newtonsoft.Json
Imports TaleWorlds.Core
Imports TaleWorlds.Library
Imports System.Runtime.CompilerServices
Imports System.IO.Compression
Imports TaleWorlds.ModuleManager
Imports System.Windows.Forms
Imports TaleWorlds
Imports System.Web
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Text
Imports Encoder = System.Drawing.Imaging.Encoder

Public Module Util
    Public AllowInDebugger As Boolean = True
    Public CatchOnApplicationTick As Boolean = True
    Public CatchOnMissionScreenTick As Boolean = True
    Public CatchOnFrameTick As Boolean = True
    Public CatchTick As Boolean = True
    Public CatchComponentBehaviourTick As Boolean = True
    Public CatchGlobalTick As Boolean = True
    Public CatchNative2Managed As Boolean = True
    Public DisableBewButterlibException As Boolean = True
    Public IsFirstTime As Boolean = True
    Public EnableStdoutConsole As Boolean = False
    Public EnableLogger As Boolean = True
    Public ShowHardCrashPrompt As Boolean = False

    Public Sub ReadConfig()
        Try
            Dim data = File.ReadAllText(BewConfigPath)
            Dim configData = JsonConvert.DeserializeObject(data)
            AllowInDebugger = configData("AllowInDebugger")
            CatchOnApplicationTick = configData("CatchOnApplicationTick")
            CatchOnMissionScreenTick = configData("CatchOnMissionScreenTick")
            CatchOnFrameTick = configData("CatchOnFrameTick")
            CatchTick = configData("CatchTick")
            CatchComponentBehaviourTick = configData("CatchComponentBehaviourTick")
            CatchGlobalTick = configData("CatchGlobalTick")
            CatchNative2Managed = configData("CatchNative2Managed")
            IsFirstTime = configData("IsFirstTime")
            DisableBewButterlibException = configData("DisableBewButterlibException")
            EnableStdoutConsole = configData("EnableStdoutConsole")
            EnableLogger = configData("EnableLogger")
        Catch ex As Exception
            Console.WriteLine("unable to read the config. It's ok")
        End Try
    End Sub
    Public Sub SaveSettings()
        Try
            Dim config As New Dictionary(Of String, Boolean)
            config.Add("AllowInDebugger", AllowInDebugger)
            config.Add("CatchOnApplicationTick", CatchOnApplicationTick)
            config.Add("CatchOnMissionScreenTick", CatchOnMissionScreenTick)
            config.Add("CatchOnFrameTick", CatchOnFrameTick)
            config.Add("CatchTick", CatchTick)
            config.Add("CatchComponentBehaviourTick", CatchComponentBehaviourTick)
            config.Add("CatchGlobalTick", CatchGlobalTick)
            config.Add("CatchNative2Managed", CatchNative2Managed)
            config.Add("IsFirstTime", IsFirstTime)
            config.Add("DisableBewButterlibException", DisableBewButterlibException)
            config.Add("EnableConsole", EnableStdoutConsole)
            config.Add("EnableLogger", EnableLogger)
            File.WriteAllText(BewConfigPath, ToJson(config))
        Catch ex As Exception
            Console.WriteLine("cannot write config " & ex.Message)
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
    Public Sub Print(str As String, Optional submitIntoDebugSpool As Boolean = True)
        InformationManager.DisplayMessage(New InformationMessage("BetterExceptionWindow: " & str))
        If submitIntoDebugSpool Then Debug.Print("BetterExceptionWindow: " & str)
    End Sub

    Public Sub ShowToastMessage(str As String)
        MBInformationManager.AddQuickInformation(New Localization.TextObject(str))
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

    Public Function CheckIsAssemblyLoaded(dllFilename As String) As Boolean
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
    Public Function GetAssemblyByDll(dllFilename As String) As Assembly
        Dim asm As Assembly() = AppDomain.CurrentDomain.GetAssemblies()
        For Each x In asm
            Try
                If Path.GetFileName(x.Location) = dllFilename Then
                    Return x
                End If
            Catch ex As Exception
            End Try
        Next
        Return Nothing
    End Function
    Public Function GetTypeFromAssembly(ByRef assem As Assembly, typeName As String) As Type
        If assem Is Nothing Then Throw New Exception("assem was nothing!")
        Dim typ = assem.GetType(typeName)
        Return typ
    End Function
    Public Function GetMethodFromType(ByRef type_ As Type, procedureName As String, Optional flag As BindingFlags = BindingFlags.Public) As MethodInfo
        If type_ Is Nothing Then Throw New Exception("type was nothing!")
        Dim method = type_.GetMethod(procedureName, flag)
        Return method
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
    Public Function GetBinFolderOfModule(modId As String) As String
        Return Path.GetFullPath(ModuleHelper.GetModuleFullPath(modId) & "\bin\Win64_Shipping_Client")
    End Function
    Public Function GetListOfBannerlordDllModules() As List(Of String)
        Dim nativeModules = {"Native", "SandBoxCore", "CustomBattle", "StoryMode"}
        Dim modulesPath = Path.GetFullPath(BaseDir & "\..\..\Modules")
        Dim modulesDirectories = Directory.EnumerateDirectories(modulesPath)
        Dim results As New List(Of String)
        For Each x In modulesDirectories
            If nativeModules.Contains(x) Then Continue For
            If Directory.Exists(Path.GetFullPath(x) & "\bin\Win64_Shipping_Client") Then
                Dim files = Directory.EnumerateFiles(Path.GetFullPath(x) & "\bin\Win64_Shipping_Client").Where(Function(y) y.EndsWith("dll"))
                results.AddRange(files)
            End If
            If Directory.Exists(Path.GetFullPath(x) & "\bin\Win64_Shipping_wEditor") Then
                Dim files = Directory.EnumerateFiles(Path.GetFullPath(x) & "\bin\Win64_Shipping_wEditor").Where(Function(y) y.EndsWith("dll"))
                results.AddRange(files)
            End If
        Next
        Dim isSteamWorkshopFolderExists = Directory.Exists(Path.GetFullPath(BaseDir & "/../../../../workshop/content/261550"))
        If IsRunningSteam And isSteamWorkshopFolderExists Then
            Dim modulesInSteamWorkshopDirectory = Directory.GetDirectories(Path.GetFullPath(BaseDir & "/../../../../workshop/content/261550"))
            For Each x In modulesInSteamWorkshopDirectory
                If Directory.Exists(Path.GetFullPath(x) & "\bin\Win64_Shipping_Client") Then
                    Dim files = Directory.EnumerateFiles(Path.GetFullPath(x) & "\bin\Win64_Shipping_Client").Where(Function(y) y.EndsWith("dll"))
                    results.AddRange(files)
                End If
                If Directory.Exists(Path.GetFullPath(x) & "\bin\Win64_Shipping_wEditor") Then
                    Dim files = Directory.EnumerateFiles(Path.GetFullPath(x) & "\bin\Win64_Shipping_wEditor").Where(Function(y) y.EndsWith("dll"))
                    results.AddRange(files)
                End If
            Next
        End If
        Return results
    End Function
    Public Sub WriteNewDnspySettings()
        Dim xml = FileSystem.ReadAllText(Path.GetFullPath(BewDir & "\dnSpy.settings.xml"))
        If Not Directory.Exists(Path.GetFullPath(BewDir & "\temp\")) Then
            Directory.CreateDirectory(Path.GetFullPath(BewDir & "\temp\"))
        End If
        xml = xml.Replace("{BANNERLORD_MAIN_BIN}", HttpUtility.HtmlEncode(Path.GetFullPath(BaseDir)))
        xml = xml.Replace("{BANNERLORD_MODULE_NATIVE_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("Native")))
        If Not IsNothing(ModuleHelper.GetModuleInfo("DedicatedCustomServerHelper")) Then xml = xml.Replace("{BANNERLORD_MODULE_DEDICATED_SERVER_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("DedicatedCustomServerHelper")))
        xml = xml.Replace("{BANNERLORD_MODULE_BIRTH_AND_DEATH_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("BirthAndDeath")))
        xml = xml.Replace("{BANNERLORD_MODULE_CUSTOM_BATTLE_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("CustomBattle")))
        xml = xml.Replace("{BANNERLORD_MODULE_SANBOX_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("Sandbox")))
        xml = xml.Replace("{BANNERLORD_MODULE_STORY_MODE_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("StoryMode")))
        xml = xml.Replace("{BANNERLORD_MODULE_STORY_MODE_BIN}", HttpUtility.HtmlEncode(GetBinFolderOfModule("StoryMode")))

        Dim thirdPartiesDll = GetListOfBannerlordDllModules()
        Dim section = ""
        For Each x In thirdPartiesDll
            section = section & vbNewLine & "<section _=""File"" name=""" & x & """ />"
        Next
        section = section.Replace("&", "&amp;")
        xml = xml.Replace("{THIRD_PARTY_MODULES}", section)

        FileSystem.WriteAllText(Path.GetFullPath(BewDir & "\temp\dnSpy.settings.1.xml"), xml, False)
    End Sub
    Public Sub RestartAndAttachDnspy()
        WriteNewDnspySettings()
        Dim PsBannerlord As New ProcessStartInfo()
        Dim currentpath = Directory.GetCurrentDirectory()
        PsBannerlord.Arguments = "/C cd """ & currentpath &
                         """ && ping 127.0.0.1 -n 2 && " &
                         "start TaleWorlds.MountAndBlade.Launcher.exe --disablebew && " &
                         " ping 127.0.0.1 -n 3 && " &
                         """" & Path.GetFullPath(DnspyDir) & "dnSpy.exe"" --process-name TaleWorlds.MountAndBlade.Launcher.exe --settings-file " &
                         """" & Path.GetFullPath(BewTemp & "dnSpy.settings.1.xml") & """"
        PsBannerlord.WindowStyle = ProcessWindowStyle.Hidden
        PsBannerlord.CreateNoWindow = True
        PsBannerlord.FileName = "cmd.exe"
        Process.Start(PsBannerlord)
        KillGame()
    End Sub
    Public Sub AttachDnspy()
        WriteNewDnspySettings()
        Dim PsDnspy As New ProcessStartInfo()
        Dim pid = Process.GetCurrentProcess().Id
        Dim currentpath = Directory.GetCurrentDirectory()
        PsDnspy.Arguments = "/C cd """ & currentpath & """ && " &
                         """" & Path.GetFullPath(DnspyDir) & "dnSpy.exe"" --pid " & pid & " " &
                         "--settings-file """ & Path.GetFullPath(BewTemp & "dnSpy.settings.1.xml") & """"
        PsDnspy.WindowStyle = ProcessWindowStyle.Hidden
        PsDnspy.CreateNoWindow = True
        PsDnspy.FileName = "cmd.exe"
        Process.Start(PsDnspy)
    End Sub
    Public Sub KillGame()
        Dim pid = Process.GetCurrentProcess().Id
        Dim proc As Process = Process.GetProcessById(pid)
        proc.Kill()
    End Sub
    <JsonObject>
    Private Class DnsSpyStructure
        Public Url As String
        Public Files As List(Of File)
        <JsonObject>
        Public Class File
            Public Name As String
            Public Children As List(Of File)
            Public IsAvailable = False
        End Class
        Public Shared Function SetFileAvailability(ByRef files As List(Of File), fileName As String, isAvail As Boolean) As Boolean
            For Each f In files
                If f.Name = fileName Then
                    f.IsAvailable = isAvail
                    Return True
                End If
            Next
            Return False
        End Function
    End Class
    Private Function CheckDnspyManifest(path As String, ByRef files As List(Of DnsSpyStructure.File)) As Boolean
        TraverseDnspyManifestRecursively(path, files)
        Return CheckDnspyManifestRecursively(files)
    End Function
    Private Function CheckDnspyManifestRecursively(files As List(Of DnsSpyStructure.File)) As Boolean
        For Each f In files
            If Not f.IsAvailable Then Return False
            Return CheckDnspyManifestRecursively(f.Children)
        Next
        Return True
    End Function
    Private Sub TraverseDnspyManifestRecursively(path As String, ByRef files As List(Of DnsSpyStructure.File))
        For Each f In files
            Dim isDirectory = Directory.Exists(path & f.Name)
            Dim isFile = File.Exists(path & f.Name)
            If isDirectory Then
                DnsSpyStructure.SetFileAvailability(files, f.Name, True)
                TraverseDnspyManifestRecursively(path & "\" & f.Name & "\", f.Children)
            End If
            If isFile Then DnsSpyStructure.SetFileAvailability(files, f.Name, True)
        Next
    End Sub
    Public Function IsDnspyAvailable()
        Dim DnspyManifestFile = File.ReadAllText(DnspyManifest)
        Dim DnspyManifestStruct = JsonConvert.DeserializeObject(Of DnsSpyStructure)(DnspyManifestFile)
        Dim Files = DnspyManifestStruct.Files
        If Not Directory.Exists(DnspyDir) Then Return False
        If Not File.Exists(DnspyDir & "dnSpy.exe") Then Return False
        Return CheckDnspyManifest(DnspyDir & "\bin\", Files)
    End Function
    Private Function GetEncoderInfo(mimeType As String) As ImageCodecInfo
        Dim j As Integer
        Dim encoders As ImageCodecInfo()
        encoders = ImageCodecInfo.GetImageEncoders()
        For j = 0 To encoders.Length
            If encoders(j).MimeType = mimeType Then
                Return encoders(j)
            End If
        Next j
        Return Nothing
    End Function
    Private Sub SaveJPGWithCompressionSetting(image As Image, szFileName As String, lCompression As Long)
        Dim eps As EncoderParameters = New EncoderParameters(1)
        eps.Param(0) = New EncoderParameter(Encoder.Quality, lCompression)
        Dim ici As ImageCodecInfo = GetEncoderInfo("image/jpeg")
        image.Save(szFileName, ici, eps)
    End Sub
    Public Sub TakeScreenshot(filename As String)
        If Not Directory.Exists(Path.GetFullPath(BewDir & "\temp\")) Then Directory.CreateDirectory(Path.GetFullPath(BewDir & "\temp\"))
        Dim filenameAndPath = BewTemp & "\" & filename & ".jpg"
        Engine.Utilities.TakeScreenshot(filenameAndPath)
        Dim filenameAndPathCompressed = BewTemp & "\" & filename & ".compressed.jpg"
        SaveJPGWithCompressionSetting(Image.FromFile(filenameAndPath), filenameAndPathCompressed, 80L)
    End Sub
    Public Function FileToBase64String(filename As String) As String
        Dim bytes = File.ReadAllBytes(filename)
        Dim strB64Encoded As String = Convert.ToBase64String(bytes)
        Return strB64Encoded
    End Function
    Public Function IsMcmLoaded() As Boolean
        Return CheckIsAssemblyLoaded("MCMv5.dll") AndAlso
               CheckIsAssemblyLoaded("MCMv5.UI.dll")
    End Function
    'TODO: grab it from xml string
    Public ReadOnly Version As String = "BetterExceptionWindow version " & GetVersionString()
    Public ReadOnly Commit As String = My.Resources.CurrentCommit
    Public ReadOnly DnspyDir As String =
        BewBasePath() & "\bin\Win64_Shipping_Client\dnspy\"
    Public ReadOnly DnspyManifest As String =
        BewBasePath() & "\dnSpyManifest.json"
    Public ReadOnly BewDir As String =
        BewBasePath()
    Public ReadOnly BaseDir As String =
        Directory.GetCurrentDirectory()
    Public ReadOnly BewTemp As String =
        BewBasePath() & "\Temp\"
    Public ReadOnly BewBinDir As String =
        BewBasePath() & "\bin\Win64_Shipping_Client\"
    Public ReadOnly BewConfigPath As String =
        BewBasePath() & "\config.json"
    Public ReadOnly BewExceptionDefinitionsPath As String =
        BewBasePath() & "\solutions.json"
    Public ReadOnly DefinitionsDownloadUrl As String =
        "https://raw.githubusercontent.com/admiralnelson/bannerlord-error-detector/master/BetterExceptionWindow/solutions.json"
    Public ReadOnly IsDebuggedByDnspy = Debugger.IsAttached And Environment.GetCommandLineArgs.Contains("--disablebew")
    Public ReadOnly IsRunningSteam = Environment.GetEnvironmentVariable("SteamEnv")
    Public Function BewBasePath() As String
        Return ModuleHelper.GetModuleFullPath("BetterExceptionWindow")
    End Function
    Public Function GetVersionString() As String
        Return Assembly.GetExecutingAssembly().GetName().Version.ToString()
    End Function

End Module