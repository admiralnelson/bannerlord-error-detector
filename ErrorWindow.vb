Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Xml
Imports Newtonsoft.Json

<PermissionSet(SecurityAction.Demand, Name:="FullTrust")>
<ComVisibleAttribute(True)>
Public Class ErrorWindow
    Public Shared exceptionData As Exception

    Private Sub ErrorWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim menu As New Windows.Forms.ContextMenu()
        menu.MenuItems.Add("none")
        widget.ContextMenu = menu
        Me.TopMost = True
        widget.ObjectForScripting = Me
        Dim html = File.ReadAllText("..\..\Modules\BetterExceptionWindow\errorui.htm")
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

        widget.DocumentText = html
    End Sub

    Public Sub Save()
        Dim filename = Str(DateTime.Now.ToFileTimeUtc()) + ".htm"
        File.WriteAllText(filename, widget.Document.Body.OuterHtml)
        Dim filePath = Path.GetFullPath(SaveLogPath + filename)
        MessageBox.Show(filePath, "Saved to")
    End Sub

    Public Sub CloseProgram()
        Dim pid = Process.GetCurrentProcess().Id
        Dim proc As Process = Process.GetProcessById(pid)
        proc.Kill()
    End Sub

    Public Sub OpenConfig()
        Try
            Dim p = Path.GetFullPath("..\..\Modules\BetterExceptionWindow\config.json")
            Process.Start(p)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub OpenPath(s As String)
        Dim p = Path.GetDirectoryName(s)
        Process.Start(p)
    End Sub

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
                Dim checksum = CalculateMD5(location)
                Dim li = $"Assembly {name}, checksum: {checksum}. Location: <a href='#' onclick='window.external.OpenPath(""{link}"")'>{location}</a>"
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
    Public Sub AnalyseModules()
        Dim modulePath = Path.GetFullPath("..\..\Modules\")
        Dim myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        Dim loadedModuleJSON = ReadXmlAsJson(myDocument + "\Mount and Blade II Bannerlord\Configs\LauncherData.xml")
        Dim loadedModule = loadedModuleJSON("UserData")("SingleplayerData")("ModDatas")("UserModData")
        Dim loadedModuleList As New List(Of String)
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
            'MAYBE NOT AN ARRAY BUT PLAIN OBJECT INSTEAD
            Dim maybeArray = True
            Try
                jsondata("isFaultingMod") = exceptionData.StackTrace.Contains(jsondata("Module")("SubModules")("SubModule")("SubModuleClassType")("@value").ToString())
                maybeArray = False
            Catch ex As Exception
            End Try
            Try
                If maybeArray Then
                    For Each dll In jsondata("Module")("SubModules")
                        jsondata("isFaultingMod") = exceptionData.StackTrace.Contains(dll("SubModule")("SubModuleClassType")("@value").ToString())
                    Next
                End If
            Catch ex As Exception
            End Try

            jsondata("isModLoaded") = loadedModuleList.Any(Function(f) f.Contains(jsondata("Module")("Id")("@value")))
            jsondata("isPDBIncluded") = False
            jsondata("location") = x
            jsondata("manifest") = x + "\SubModule.xml"
            widget.Document.InvokeScript("DisplayModulesReport", New String() {JsonConvert.SerializeObject(jsondata)})
        Next
    End Sub

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

    Private Sub widget_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles widget.DocumentCompleted
        'AnalyseModules()
    End Sub
End Class