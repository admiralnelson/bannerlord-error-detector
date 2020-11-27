Imports System.Threading
Imports System.Windows.Forms
Imports HarmonyLib
Imports TaleWorlds.Core
Imports TaleWorlds.InputSystem
Imports TaleWorlds.MountAndBlade

Namespace Global.BetterExceptionWindow
    Public Class Main
        Inherits MBSubModuleBase


        <HarmonyPatch(GetType(TaleWorlds.DotNet.Managed), "ApplicationTick")>
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
        <HarmonyPatch(GetType(TaleWorlds.Engine.ScriptComponentBehaviour), "OnTick")>
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

        <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.Module), "OnApplicationTick")>
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
        <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.View.Missions.MissionView), "OnMissionScreenTick")>
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
        <HarmonyPatch(GetType(TaleWorlds.Engine.Screens.ScreenManager), "Tick")>
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
        <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.Mission), "Tick")>
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

        <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.MissionBehaviour), "OnMissionTick")>
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


        Public Sub New()

        End Sub

        Protected Overrides Sub OnSubModuleLoad()
            MyBase.OnSubModuleLoad()
            ReadConfig()
            Dim harmony = New Harmony("org.calradia.admiralnelson.betterexceptionwindow")
            If Debugger.IsAttached And Not AllowInDebugger Then

            Else
                harmony.PatchAll()
            End If
        End Sub



        Protected Overrides Sub OnBeforeInitialModuleScreenSetAsRoot()
            MyBase.OnBeforeInitialModuleScreenSetAsRoot()

        End Sub
        Public Overrides Sub OnCampaignStart(gam As Game, starterObject As Object)
            MyBase.OnCampaignStart(gam, starterObject)
            Print("campaign start")

        End Sub
    End Class
End Namespace
