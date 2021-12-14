Imports System.Threading
Imports System.Windows.Forms
Imports HarmonyLib
Imports TaleWorlds.Core
Imports TaleWorlds.DotNet
Imports TaleWorlds.Engine
Imports TaleWorlds.InputSystem
Imports TaleWorlds.MountAndBlade
Imports TaleWorlds.MountAndBlade.View.Missions

Namespace Global.BetterExceptionWindow
    Public Class Main
        Inherits MBSubModuleBase


        <HarmonyPatch(GetType(Managed), "ApplicationTick")>
        Public Class OnApplicationTickCorePatch
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

        <HarmonyPatch(GetType([Module]), "OnApplicationTick")>
        Public Class OnApplicationTickPatch
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
        <HarmonyPatch(GetType(MissionView), "OnMissionScreenTick")>
        Public Class OnMissionScreenTickPatch
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
        <HarmonyPatch(GetType(Screens.ScreenManager), "Tick")>
        Public Class OnFrameTickPatch
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
        Public Sub New()

        End Sub
        Private Sub PatchMe()
            Dim harmony = New Harmony("org.calradia.admiralnelson.betterexceptionwindow")
            harmony.PatchAll()

            AddHandler Application.ThreadException, AddressOf AppDomain_UnhandledExceptionThr
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf AppDomain_UnhandledException

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        End Sub
        Protected Overrides Sub OnSubModuleLoad()
            MyBase.OnSubModuleLoad()
            ReadConfig()

            If Debugger.IsAttached Then
                If AllowInDebugger Then
                    PatchMe()
                End If
            Else
                PatchMe()
            End If

        End Sub
    End Class
End Namespace
