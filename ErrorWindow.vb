Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Xml
Imports HarmonyLib
Imports Newtonsoft.Json
Imports TaleWorlds.Core
Imports TaleWorlds.Library
Imports TaleWorlds.SaveSystem

<PermissionSet(SecurityAction.Demand, Name:="FullTrust")>
<ComVisibleAttribute(True)>
Public Class ErrorWindow
    Public Shared exceptionData As Exception
    Dim problematicModules = New SortedSet(Of String)
    Dim html = File.ReadAllText(BewBasePath & "\errorui.htm")

    Private Sub ErrorWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim menu As New Windows.Forms.ContextMenu()
        menu.MenuItems.Add("none")
        widget.ContextMenu = menu
        Me.TopMost = True
        widget.ObjectForScripting = Me
        html = html.Replace("{errorString}", exceptionData.Message)
        html = html.Replace("{faultingSource}", exceptionData.Source)
        html = html.Replace("{fullStackString}", exceptionData.StackTrace)
        Dim installedMods = ""
        Dim listOfInstalledMods = GetAssembliesList(False)
        For Each x In listOfInstalledMods
            installedMods = installedMods + x
        Next
        html = html.Replace("{installedMods}", installedMods)
        If exceptionData.InnerException IsNot Nothing Then
            html = html.Replace("{innerException}", exceptionData.InnerException.Message)
            html = html.Replace("{innerFaultingSource}", exceptionData.InnerException.Source)
            html = html.Replace("{innerExceptionCallStack}", exceptionData.InnerException.StackTrace)
        Else
            html = html.Replace("{innerException}", "No inner exception was thrown")
            html = html.Replace("{innerFaultingSource}", "No module")
            html = html.Replace("{innerExceptionCallStack}", "No inner exception was thrown")
        End If
        html = html.Replace("{jsonData}", AnalyseModules())
        'wait a bit to gather game log
        Sleep(5 * 1000)
        html = html.Replace("{gameLogs}", GetGameLog())
        html = html.Replace("{logtime}", GetGameLogDateTime())
        html = html.Replace("{jsonHarmony}", GetHarmonyPatches())
        html = html.Replace("{version}", Version)
        html = html.Replace("{commit}", Commit)
        widget.DocumentText = html
        AddHandler widget.Document.ContextMenuShowing, AddressOf WebContextMenuShowing
        AddHandler widget.PreviewKeyDown, AddressOf WebShorcut
        widget.WebBrowserShortcutsEnabled = False
        widget.ScriptErrorsSuppressed = True
        'widget.Document.InvokeScript("AnalyseModule", Nothing)
    End Sub

    Private Sub WebContextMenuShowing(ByVal sender As Object, ByVal e As HtmlElementEventArgs)
        'disables context menu
        widgetMenu.Show(widget, e.MousePosition)
        'Me.widget.ContextMenuStrip.Show(Cursor.Position)
        e.ReturnValue = False
    End Sub

    Private Sub WebShorcut(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs)
        If e.Modifiers = Keys.Control And e.KeyCode = Keys.C Then
            widget.Document.ExecCommand("Copy", False, Nothing)
        End If
        If e.Modifiers = Keys.Control And e.KeyCode = Keys.P Then
            widget.Document.InvokeScript("window.print", Nothing)
        End If
        If e.Modifiers = Keys.Control And e.KeyCode = Keys.A Then
            widget.Document.ExecCommand("SelectAll", False, Nothing)
        End If
    End Sub


    Public Function Save()
        'Dim filename = Str(DateTime.Now.ToFileTimeUtc()) + ".htm"
        Dim fileDialog As New SaveFileDialog()
        fileDialog.Filter = "HTML (*.htm)|*.htm|All files (*.*)|*.*"
        If fileDialog.ShowDialog() = DialogResult.OK AndAlso fileDialog.FileName <> "" Then
            Dim filename = fileDialog.FileName
            File.WriteAllText(filename, html)
            Return filename
        End If
        Return Nothing
    End Function

    Public Sub CloseProgram()
        KillGame()
    End Sub

    Public Sub OpenConfig()
        Try
            Dim p = Path.GetFullPath(BewBasePath & "\config.json")
            Process.Start(p)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub OpenPath(s As String)
        Dim p = Path.GetDirectoryName(s)
        Process.Start(p)
    End Sub
    Private Function GetGameLog()
        Dim dataPath = GetBanenrlordProgramDataPath() + "\logs\"
        Dim temptFile = New DirectoryInfo(dataPath).EnumerateFiles() _
                                                     .Where(Function(x)
                                                                Dim reg = New Regex("rgl_log_([0-9])*\.txt")
                                                                Return reg.Match(x.Name).Success
                                                            End Function) _
                                                     .FirstOrDefault()
        If temptFile IsNot Nothing Then
            Dim filestrm As New FileStream(temptFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Dim txtreader As New StreamReader(filestrm)
            Return txtreader.ReadToEnd()
        End If
        Return ""
    End Function
    Private Class HarmonyPatchInformation
        Public Finalisers As List(Of Patch)
        Public Prefixes As List(Of Patch)
        Public Postfixes As List(Of Patch)
        Public Transpilers As List(Of Patch)
    End Class
    Private Function GetHarmonyPatches()
        Dim patches = Harmony.GetAllPatchedMethods()
        Dim listOfPatches As New Dictionary(Of String, HarmonyPatchInformation)
        For Each p In patches
            Dim patch = Harmony.GetPatchInfo(p)
            Dim patchInfo As New HarmonyPatchInformation
            With patchInfo
                .Finalisers = patch.Finalizers.ToList()
                .Postfixes = patch.Postfixes.ToList()
                .Prefixes = patch.Prefixes.ToList()
                .Transpilers = patch.Transpilers.ToList()
            End With
            listOfPatches(p.FullDescription()) = patchInfo
        Next
        Return JsonConvert.SerializeObject(listOfPatches)
    End Function
    Private Function GetGameLogDateTime()
        Dim dataPath = GetBanenrlordProgramDataPath() + "\logs\"
        Dim biggestFile = New DirectoryInfo(dataPath).EnumerateFiles() _
                                                     .Where(Function(x) x.Name.StartsWith("rgl_log_")) _
                                                     .OrderByDescending(Function(x) x.LastWriteTimeUtc) _
                                                     .FirstOrDefault()
        If biggestFile IsNot Nothing Then
            Return biggestFile.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss 'GMT'z")
        End If
        Return ""
    End Function
    Private Function GetModulePathFromAssembly(dll As Assembly) As String
        Dim location = dll.Location
        location = Path.GetDirectoryName(location)
        Dim realLocation = Path.GetFullPath(location + "\..\..\")
        Return realLocation
    End Function

    Private Function ReadXmlAsJson(path As String) As Object
        Dim data = File.ReadAllText(path)
        Dim xmldata As New XmlDocument()
        xmldata.LoadXml(data)
        Return JsonConvert.DeserializeObject(JsonConvert.SerializeXmlNode(xmldata))
    End Function

    Private Sub AddXMLDiagResult(filename As String, filepath As String, Optional errorStr As String = "")
        If errorStr <> "" Then
            widget.Document.InvokeScript("addXMLDiagResult", New String() {filename, filepath, errorStr})
        Else
            widget.Document.InvokeScript("addXMLDiagResult", New String() {filename, filepath})
        End If
    End Sub

    Private Function GetAssembliesList(searchAlsoInGameBins As Boolean) As List(Of String)
        Dim asm = AppDomain.CurrentDomain.GetAssemblies()
        Dim out As New List(Of String)
        For Each x In asm
            Try
                If Not x.Location.ToLower().Contains("Mount & Blade II Bannerlord".ToLower()) Then
                    Continue For
                End If
                Dim name = x.GetName().Name
                Dim assmeblyVer = x.GetName().Version
                Dim location = x.Location
                Dim link = x.Location.Replace("\", "\\")
                Dim version = assmeblyVer
                Dim li = $"Assembly {name}, version: {version}. Location: <a href='#' onclick='window.external.OpenPath(""{link}"")'>{location}</a>"
                Dim output = ""
                If name.StartsWith("TaleWorlds") Or
                    name.StartsWith("SandBox") Or
                    name.StartsWith("StoryMode") Or
                    name.StartsWith("Steamworks") Then
                    output = $"<li class='taleworlds_ul'>{li}</li>" & vbNewLine
                Else
                    output = $"<li>{li}</li>" & vbNewLine
                End If
                out.Add(output)
            Catch ex As Exception

            End Try
        Next
        Return out
    End Function

    Public Sub DisableProblematicModules()
        Dim myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        Dim xmlPath = myDocument + "\Mount and Blade II Bannerlord\Configs\LauncherData.xml"
        Dim loadedModuleJSON = ReadXmlAsJson(xmlPath)
        Debug.Print(JsonConvert.SerializeObject(loadedModuleJSON))
        Dim nativeModules = {"Native", "SandBoxCore", "CustomBattle", "StoryMode"}
        Dim modulesString = ""
        For Each x In problematicModules
            modulesString = modulesString + x + ","
        Next
        If problematicModules.Count() = 0 Then
            MsgBox("Unable to fix this. We can't determine faulting modules.", MsgBoxStyle.Critical)
            Exit Sub
        End If
        modulesString = modulesString.Substring(0, modulesString.Length() - 1)
        Dim prompt = MsgBox("Are you sure to disable these modules: " + vbNewLine +
               modulesString,
               MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo,
               "Disable mods"
        )
        If prompt = MsgBoxResult.Yes Then
            Dim userModData = loadedModuleJSON("UserData")("SingleplayerData")("ModDatas")("UserModData")
            For Each x In userModData
                Dim moduleId = x("Id").ToString()
                For Each y In problematicModules
                    If moduleId = y AndAlso Not nativeModules.Contains(y) Then
                        x("IsSelected") = "false"
                        Exit For
                    End If
                Next
            Next
            Dim jsonString = JsonConvert.SerializeObject(loadedModuleJSON)
            Debug.Print(jsonString)
            Dim xmlData As XmlDocument = JsonConvert.DeserializeXmlNode(jsonString)
            Dim stringXml = New IO.StringWriter()
            Dim xmlWriter = New XmlTextWriter(stringXml)
            xmlData.WriteTo(xmlWriter)
            Debug.Print(stringXml.ToString())
            File.WriteAllText(xmlPath, stringXml.ToString())
            MsgBox("mods have been disabled")
        End If
    End Sub

    Public Function AnalyseModules()
        Dim modulePath = Path.GetFullPath(BewDir & "\..\..\Modules\")
        Dim myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        Dim loadedModuleJSON = ReadXmlAsJson(myDocument + "\Mount and Blade II Bannerlord\Configs\LauncherData.xml")
        Dim loadedModule = loadedModuleJSON("UserData")("SingleplayerData")("ModDatas")("UserModData")
        Dim loadedModuleList As New List(Of String)
        Dim analysedModuleJSON As New List(Of Object)
        For Each x In loadedModule
            If x("IsSelected") Then
                loadedModuleList.Add(x("Id"))
            End If
        Next

        Dim directories = Directory.GetDirectories(modulePath)
        For Each x In directories
            Dim jsondata
            Try
                jsondata = ReadXmlAsJson(x + "\SubModule.xml")
            Catch ex As Exception
                Continue For
            End Try
            jsondata("isModLoaded") = loadedModuleList.Any(Function(f) f.Contains(jsondata("Module")("Id")("@value")))
            jsondata("isPDBIncluded") = False
            jsondata("location") = x
            jsondata("manifest") = x + "\SubModule.xml"
            'MAYBE NOT AN ARRAY BUT PLAIN OBJECT INSTEAD
            Dim maybeArray = True
            Try
                Dim subModuleClassType As String = jsondata("Module")("SubModules")("SubModule")("Name")("@value")
                'Dim subModuleClassType As String = jsondata("Module")("SubModules")("SubModule")("SubModuleClassType")("@value")
                'subModuleClassType = subModuleClassType.Substring(0, subModuleClassType.IndexOf("."))
                Dim modId = jsondata("Module")("Id")("@value")
                jsondata("isFaultingMod") = exceptionData.StackTrace.Contains(subModuleClassType) And jsondata("isModLoaded")
                If exceptionData.InnerException IsNot Nothing Then
                    jsondata("isFaultingMod") = jsondata("isFaultingMod") Or exceptionData.InnerException.StackTrace.Contains(subModuleClassType)
                End If
                If (jsondata("isFaultingMod")) Then
                    problematicModules.Add(modId)
                End If
                maybeArray = False
            Catch ex As Exception
            End Try
            Try
                If maybeArray Then
                    For Each dll In jsondata("Module")("SubModules")("SubModule")
                        Dim name As String = dll("Name")("@value")
                        Dim modId = jsondata("Module")("Id")("@value")
                        'subModuleClassType = subModuleClassType.Substring(0, subModuleClassType.IndexOf("."))
                        jsondata("isFaultingMod") = exceptionData.StackTrace.Contains(name) And jsondata("isModLoaded")
                        Dim result = CheckIsAssemblyLoaded(dll("DLLName")("@value"))
                        If result Then
                            Debug.Print("ok")
                            Debug.Print(dll("DLLName")("@value"))
                            dll("isLoadedInMemory") = True
                        Else
                            dll("isLoadedInMemory") = False
                        End If
                        If (jsondata("isFaultingMod")) Then
                            problematicModules.Add(modId)
                        End If
                    Next
                End If
            Catch ex As Exception
            End Try

            'widget.Document.InvokeScript("DisplayModulesReport", New String() {JsonConvert.SerializeObject(jsondata)})
            analysedModuleJSON.Add(jsondata)
        Next
        Return JsonConvert.SerializeObject(analysedModuleJSON)
    End Function

    Public Sub ScanAndLintXmls()
        Dim errorDetected = False
        Dim modulePath = Path.GetFullPath(BewDir & "..\..\Modules\")
        Dim files = Directory.GetFiles(modulePath, "*.xml", SearchOption.AllDirectories)

        For Each x In files
            Dim filename = Path.GetFileName(x)
            Try
                Dim doc As New XmlDocument()
                doc.Load(New StreamReader(x))
                Debug.Print(x)
                widget.Document.InvokeScript("addXMLDiagResult", New String() {filename, x})
            Catch ex As XmlException
                widget.Document.InvokeScript("addXMLDiagResult", New String() {filename, x, ex.Message})
                errorDetected = True
            End Try
            Application.DoEvents() 'bad, but i dont care, TASK.RUN CompleteWith DOESN'T WORK FOR SOME REASONS
        Next
        widget.Document.InvokeScript("finishSearch", New Object() {errorDetected})
    End Sub
    Public Function IsCampaignRunning()
        Return TaleWorlds.CampaignSystem.Campaign.Current IsNot Nothing
    End Function
    Public Function ForceSave(filename As String)
        'TaleWorlds.Core.MBSaveLoad.SaveAsCurrentGame()
        Try
            If TaleWorlds.Core.Game.Current Is Nothing OrElse
                TaleWorlds.Core.Game.Current.GameType Is Nothing Then
                MsgBox("Unable to save as this is not a campaign!", MsgBoxStyle.Critical)
                Return False
            Else
                'https://stackoverflow.com/questions/135443/how-do-i-use-reflection-to-invoke-a-private-method
                Dim campaignMetaData = TaleWorlds.CampaignSystem.Campaign.Current.SaveHandler
                Dim dynamicMethodGetMetaData = campaignMetaData.GetType().GetMethod("GetSaveMetaData", BindingFlags.NonPublic Or BindingFlags.Instance)
                Dim CampaignSaveMetaData As CampaignSaveMetaDataArgs = dynamicMethodGetMetaData.Invoke(campaignMetaData, New Object() {})

                'convert to savemetadata in e1.8.0
                Dim dynamicMethodGetSaveMetaData = GetType(MBSaveLoad).GetMethod("GetSaveMetaData", BindingFlags.NonPublic Or BindingFlags.Static)
                Dim SaveMetaData As MetaData = dynamicMethodGetSaveMetaData.Invoke(Nothing, {CampaignSaveMetaData})

                'https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo.getvalue?view=net-6.0
                'find the save driver
                Dim dynamicSaveDriver = GetType(MBSaveLoad).GetField("_saveDriver", BindingFlags.NonPublic Or BindingFlags.Static)
                Dim saveDriver As ISaveDriver = CType(dynamicSaveDriver.GetValue(Nothing), ISaveDriver)

                Return SaveManager.Save(Game.Current, SaveMetaData, filename, saveDriver).Successful = True
            End If
        Catch ex As Exception
            MsgBox("error while saving! " + vbCrLf + ex.Message, MsgBoxStyle.Critical)
            Return False
        End Try
    End Function

    Private Sub widget_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles widget.Navigating
        Dim isUri = Uri.IsWellFormedUriString(e.Url.ToString(), UriKind.RelativeOrAbsolute)
        If (isUri AndAlso (e.Url.ToString().StartsWith("http://") Or e.Url.ToString().StartsWith("https://"))) Then
            e.Cancel = True
            Try
                Process.Start(e.Url.ToString())
            Catch ex As Exception
            End Try
            Exit Sub
            Try
                Process.Start("firefox.exe", e.Url.ToString())
            Catch ex As Exception
            End Try
            Exit Sub
            Try
                Process.Start("chrome.exe", e.Url.ToString())
            Catch ex As Exception
            End Try
        End If
    End Sub
    Dim dnspyInstall As New DnspyInstaller
    Public Sub InstallDnspy()
        dnspyInstall.Download(
            Sub() widget.Invoke(
                Sub()
                    widget.Document.InvokeScript("Callback_InstallDnspyComplete",
                                                 New String() {1, Nothing})
                End Sub),
            Sub(prog As DnspyInstaller.Progress) widget.Invoke(
            Sub()
                widget.Document.InvokeScript("Callback_InstallDnspyProgress",
                                             New String() {
                                             prog.PercentageDownload,
                                             prog.TotalDownloadedInByte,
                                             prog.SizeInByte})
            End Sub),
            Sub(ex As Exception) widget.Invoke(
                Sub()
                    widget.Document.InvokeScript("Callback_InstallDnspyComplete",
                                                 New String() {0, ex.Message})
                End Sub))
    End Sub
    Public Sub StartDnspyDebugger()
        RestartAndAttachDnspy()
    End Sub
    Public Function IsUnderdebugger()
        Return Debugger.IsAttached
    End Function
    Public Function IsdnSpyAvailable()
        Return Util.IsDnspyAvailable()
    End Function

    Private Sub widget_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles widget.DocumentCompleted
        'AnalyseModules()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        widget.Document.ExecCommand("Copy", False, Nothing)
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        widget.Document.ExecCommand("SelectAll", False, Nothing)
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        widget.Document.InvokeScript("window.print", Nothing)
    End Sub
End Class
