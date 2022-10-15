Imports System.IO
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

        <HarmonyPatch(GetType(Managed), "ApplicationTick")>
        Public Class OnApplicationTickCorePatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchGlobalTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(ScriptComponentBehavior), "OnTick")>
        Public Class OnComponentBehaviourTickPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchComponentBehaviourTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.[Module]), "OnApplicationTick")>
        Public Class OnApplicationTickPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchOnApplicationTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(MissionViews.MissionView), "OnMissionScreenTick")>
        Public Class OnMissionScreenTickPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchOnMissionScreenTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(ScreenSystem.ScreenManager), "Tick")>
        Public Class OnFrameTickPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchOnFrameTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(Mission), "Tick")>
        Public Class OnTickMissionPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(MissionBehavior), "OnMissionTick")>
        Public Class OnMissionTickPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        <HarmonyPatch(GetType(MBSubModuleBase), "OnSubModuleLoad")>
        Public Class OnSubModuleLoadPatch
            <HarmonyPriority(Priority.First)>
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If CatchTick Then
                    If __exception IsNot Nothing Then
                        Dim window As New ErrorWindow
                        window.exceptionData = __exception
                        window.ShowDialog()
                    End If
                End If
            End Sub
        End Class
        Public Sub AppDomain_UnhandledException(o As Object, __exception As UnhandledExceptionEventArgs)
            If CatchTick Then
                If __exception IsNot Nothing Then
                    Dim window As New ErrorWindow
                    window.exceptionData = __exception.ExceptionObject
                    window.ShowDialog()
                End If
            End If
        End Sub
        Public Sub AppDomain_UnhandledExceptionThr(o As Object, __exception As ThreadExceptionEventArgs)
            If CatchTick Then
                If __exception IsNot Nothing Then
                    Dim window As New ErrorWindow
                    window.exceptionData = __exception.Exception
                    window.ShowDialog()
                End If
            End If
        End Sub
        Dim patches
        Dim harmony_ As Harmony
        Private Sub PatchMe()
            harmony_ = New Harmony("org.calradia.admiralnelson.betterexceptionwindow")
            harmony_.PatchAll()

            If Not Debugger.IsAttached Then
                AddHandler Application.ThreadException, AddressOf AppDomain_UnhandledExceptionThr
                AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf AppDomain_UnhandledException

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
            End If
            patches = harmony_.GetPatchedMethods()
        End Sub
        Private Sub DisableButterlibException()
            Try
                Dim butterlibItSelf = AppDomain.
                                  CurrentDomain.
                                  GetAssemblies().
                                  Where(Function(ass)
                                            Return ass.GetName().Name = "Bannerlord.ButterLib"
                                        End Function).
                                  FirstOrDefault()
                If butterlibItSelf IsNot Nothing Then
                    Dim classItself = butterlibItSelf.GetType("Bannerlord.ButterLib.ExceptionHandler.ExceptionHandlerSubSystem")
                    Dim method = classItself.GetMethod("Disable")
                    Dim instance = classItself.GetProperty("Instance", BindingFlags.Static Or BindingFlags.Public).GetValue(Nothing)
                    method.Invoke(instance, New Object() {})
                End If
            Catch ex As Exception
                MsgBox("unable to disable butterlib exception, please tell admiralnelson about this! " + ex.Message, MsgBoxStyle.Exclamation, "Warning")
            End Try


        End Sub
        Private Sub LoadBetterExceptionMCMUI()
            Dim bewUIDllFilePath = BewBinDir & "\BetterExceptionWindowConfigUI.dll"
            If Not File.Exists(bewUIDllFilePath) Then
                'Print("Unable to load better exception window mcm ui. BetterExceptionWindowConfigUI.dll was not found. It's ok")
                Exit Sub
            End If
            Dim theDll = Assembly.LoadFrom(bewUIDllFilePath)
            Dim theSpecifiedModule = theDll.GetType("BetterExceptionWindowConfigUI.EntryPoint")
            Dim methodInf = theSpecifiedModule.GetMethod("Start")
            methodInf.Invoke(Nothing, New Object() {})
        End Sub
        Protected Overrides Sub OnBeforeInitialModuleScreenSetAsRoot()
            DisableButterlibFull()
            ControlPanelMcmSupport()
        End Sub
        Private Sub ControlPanelMcmSupport()
            Task.Delay(1000 * 2).ContinueWith(
                Sub()
                    If Not CheckIsAssemblyLoaded("Bannerlord.ButterLib.dll") Then
                        Exit Sub
                    End If
                    If CheckIsAssemblyLoaded("MCMv4.dll") AndAlso
                       CheckIsAssemblyLoaded("MCMv4.UI.dll") Then
                        LoadBetterExceptionMCMUI()
                    End If
                    If DisableBewButterlibException Then
                        If patches IsNot Nothing Then
                            For Each x In patches
                                harmony_.Unpatch(x, HarmonyPatchType.Finalizer, "Bannerlord.ButterLib.ExceptionHandler.BEW")
                            Next
                        End If
                    End If
                    Task.Delay(1000 * 3).ContinueWith(
                        Sub()
                            If IsFirstTime And DisableBewButterlibException Then
                                MsgBoxBannerlord("Better Exception Window Behaviour",
                                                "Better Exception Window detected Butterlib is also installed." & vbNewLine &
                                                "It will disable Butterlib exception window starting from this version." & vbNewLine & vbNewLine &
                                                "You can restore to old behaviour by unchecking Disable BewButterlib exception in mod option",
                                                Sub()
                                                    IsFirstTime = False
                                                    SaveSettings()
                                                End Sub, Nothing, "I understand")
                            End If
                        End Sub
                    )
                End Sub)
        End Sub
        Private Sub DisableButterlibFull()
            If CheckIsAssemblyLoaded("Bannerlord.ButterLib.dll") Then
                DisableButterlibException()
            End If
        End Sub
        Protected Overrides Sub OnSubModuleLoad()
            ReadConfig()

            If EnableStdoutConsole Then SpawnConsole() Else StartLogger()

            If Environment.GetCommandLineArgs.Contains("--disablebew") Or Debugger.IsAttached Then
            Else
                PatchMe()
            End If
        End Sub
    End Class
End Namespace
