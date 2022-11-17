Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Threading
Imports System.Windows.Forms
Imports HarmonyLib
Imports TaleWorlds
Imports TaleWorlds.DotNet
Imports TaleWorlds.Engine
Imports TaleWorlds.MountAndBlade
Imports TaleWorlds.MountAndBlade.View

Public Module Patches
    Dim functionApplicationTick As DynamicMethod = Nothing
    Dim patches
    Dim originalMethods
    Dim harmony_ As Harmony
    Dim bIsPatched = False

    Public Sub InitPatch()
        If bIsPatched Then Exit Sub
        bIsPatched = True

        harmony_ = New Harmony("org.calradia.admiralnelson.betterexceptionwindow")
        Dim originalmethod = AccessTools.Method(GetType(Managed), "ApplicationTick")
        harmony_.PatchAll()

        'Dim patchedMethod = AccessTools.Method(GetType(OnApplicationTickCorePatch2), "Prefix")
        'functionApplicationTick = harmony_.Patch(originalmethod)
        'harmony_.Patch(originalmethod,,,, New HarmonyMethod(patchedMethod))

        If Not Debugger.IsAttached Then
            AddHandler Application.ThreadException, AddressOf AppDomain_UnhandledExceptionThr
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf AppDomain_UnhandledException

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        End If

        patches = harmony_.GetPatchedMethods()
    End Sub
    Public Sub DisableButterlibException()
        If DisableBewButterlibException Then
            If patches IsNot Nothing Then
                For Each x In patches
                    harmony_.Unpatch(x, HarmonyPatchType.Finalizer, "Bannerlord.ButterLib.ExceptionHandler.BEW")
                Next
            End If
        End If
        Try
            Dim butterlibItSelf = GetAssemblyByDll("Bannerlord.ButterLib.dll")
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
    Public Sub Unpatch()
        harmony_.UnpatchAll()
    End Sub
    Private Sub AppDomain_UnhandledException(o As Object, __exception As UnhandledExceptionEventArgs)
        If CatchTick Then
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception.ExceptionObject
                window.ShowDialog()
            End If
        End If
    End Sub
    Private Sub AppDomain_UnhandledExceptionThr(o As Object, __exception As ThreadExceptionEventArgs)
        If CatchTick Then
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception.Exception
                window.ShowDialog()
            End If
        End If
    End Sub
    'harmony patches goes here....
    Public Class OnApplicationTickCorePatch2
        Private Shared Function Prefix(ByVal dt As Single) As Boolean
            Try
                functionApplicationTick.Invoke(Nothing, New Object() {dt})
            Catch ex As Exception
                If CatchGlobalTick Then
                    Dim window As New ErrorWindow
                    window.exceptionData = ex
                    If window.ShowDialog() = DialogResult.Retry Then
                        Return False
                    End If
                End If
            End Try
            Return False
        End Function
    End Class
    <HarmonyPatch(GetType(Managed), "ApplicationTick")>
    Public Class OnApplicationTickCorePatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(ScriptComponentBehavior), "OnTick")>
    Public Class OnComponentBehaviourTickPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.[Module]), "OnApplicationTick")>
    Public Class OnApplicationTickPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(MissionViews.MissionView), "OnMissionScreenTick")>
    Public Class OnMissionScreenTickPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(ScreenSystem.ScreenManager), "Tick")>
    Public Class OnFrameTickPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                window.bErrorWasUIRelated = True
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(Mission), "Tick")>
    Public Class OnTickMissionPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(MissionBehavior), "OnMissionTick")>
    Public Class OnMissionTickPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
    <HarmonyPatch(GetType(MBSubModuleBase), "OnSubModuleLoad")>
    Public Class OnSubModuleLoadPatch
        <HarmonyPriority(Priority.First)>
        Private Shared Function Finalizer(ByVal __exception As Exception) As Exception
            If __exception IsNot Nothing Then
                Dim window As New ErrorWindow
                window.exceptionData = __exception
                If window.ShowDialog() = DialogResult.Retry Then
                    Return Nothing
                End If
            End If
            Return __exception
        End Function
    End Class
End Module
