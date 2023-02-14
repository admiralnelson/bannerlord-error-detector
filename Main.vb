Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Threading
Imports System.Windows.Forms
Imports HarmonyLib
Imports TaleWorlds
Imports TaleWorlds.Core
Imports TaleWorlds.DotNet
Imports TaleWorlds.Engine
Imports TaleWorlds.InputSystem
Imports TaleWorlds.MountAndBlade
Imports TaleWorlds.MountAndBlade.View

Namespace Global.BetterExceptionWindow
    Public Class Main
        Inherits MBSubModuleBase
        Private Sub LoadBetterExceptionMCMUI()
            Dim bewUIDllFilePath = BewBinDir & "\BetterExceptionWindowConfigUI.dll"
            If Not File.Exists(bewUIDllFilePath) Then
                Print("Unable to load better exception window mcm ui. BetterExceptionWindowConfigUI.dll was not found. It's ok")
                Exit Sub
            End If
            Dim theDll = Assembly.LoadFrom(bewUIDllFilePath)
            Dim theSpecifiedModule = theDll.GetType("BetterExceptionWindowConfigUI.EntryPoint")
            Dim methodInf = theSpecifiedModule.GetMethod("Start")
            methodInf.Invoke(Nothing, New Object() {})
        End Sub
        Protected Overrides Sub OnBeforeInitialModuleScreenSetAsRoot()
            If DisableBewButterlibException Or IsDebuggedByDnspy Then
                DisableButterlibFull()
            End If
            ControlPanelMcmSupport()
        End Sub
        Private Sub ControlPanelMcmSupport()
            Task.Delay(1000 * 2).ContinueWith(
                Sub()
                    If IsFirstTime Then
                        Task.Delay(1000 * 3).ContinueWith(
                        Sub()
                            MsgBoxBannerlord("Native code to Managed code handler",
                                            "From version 6.1, Better Exception Window now catches exception thrown " &
                                            "from Native code to Managed code transitions, these happen during asset loading in mission and agents interaction during missions." & vbNewLine & vbNewLine &
                                            "If you're experiencing slowdown during combat or mission, you can disable this behaviour by changing CatchNative2Managed to false in config.json" &
                                            IIf(IsMcmLoaded(), " or from Mod Options > Better Exception Window > (uncheck) Enable Native to Managed exception handler", "") & vbNewLine & vbNewLine &
                                            "Do you want to keep this behaviour enabled? (Restart required if you pick 'Disable it')",
                                            Sub()
                                                CatchNative2Managed = True
                                                IsFirstTime = False
                                                SaveSettings()
                                            End Sub,
                                            Sub()
                                                CatchNative2Managed = False
                                                IsFirstTime = False
                                                SaveSettings()
                                            End Sub,
                                            "Keep it enabled", "Disable it")
                        End Sub)
                    End If
                    If Not CheckIsAssemblyLoaded("Bannerlord.ButterLib.dll") Then
                        Exit Sub
                    End If
                    If IsMcmLoaded() Then
                        LoadBetterExceptionMCMUI()
                    End If
                End Sub)
        End Sub
        Private Sub DisableButterlibFull()
            If CheckIsAssemblyLoaded("Bannerlord.ButterLib.dll") Then
                DisableButterlibException()
            End If
        End Sub
        Private Sub DownloadExceptionDefinitions()
            Dim client As New WebClient
            Dim url = DefinitionsDownloadUrl
            Dim file = BewExceptionDefinitionsPath
            ServicePointManager.Expect100Continue = True
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls Or
                SecurityProtocolType.Tls11 Or
                SecurityProtocolType.Tls12 Or
                SecurityProtocolType.Ssl3

            Task.Delay(1000 * 1).ContinueWith(
                Sub()
                    Try
                        Console.WriteLine("Updating exception definitions")
                        client.DownloadFile(New Uri(url), file)
                    Catch ex As Exception
                        Console.WriteLine("Failed to update exception definitons due to this error " + ex.Message)
                    End Try
                End Sub)
        End Sub
        Protected Overrides Sub OnSubModuleLoad()
            ReadConfig()
            If EnableStdoutConsole Then SpawnConsole() Else StartLogger()
            DownloadExceptionDefinitions()
            InitPatch()
            If IsDebuggedByDnspy Then
            Else
                If AllowInDebugger Then InitPatch()
            End If
        End Sub
    End Class
End Namespace
