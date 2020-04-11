Imports System.Threading
Imports System.Windows.Forms
Imports HarmonyLib
Imports TaleWorlds.Core
Imports TaleWorlds.MountAndBlade

Namespace Global.BetterExceptionWindow
    Public Class Main
        Inherits MBSubModuleBase

        <HarmonyPatch(GetType(TaleWorlds.MountAndBlade.Module), "OnApplicationTick")>
        Public Class OnApplicationTickPatch
            Private Shared Sub Finalizer(ByVal __exception As Exception)
                If __exception IsNot Nothing Then
                    Dim window As New ErrorWindow
                    window.errorString = __exception.Message
                    window.faultingSource = __exception.Source
                    window.fullStackString = __exception.StackTrace
                    window.ShowDialog()
                End If
            End Sub

        End Class


        Public Sub New()
            'AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
            ' AddHandler Application.ThreadException, New Threading.ThreadExceptionEventHandler(AddressOf Application_ThreadException)
            ' Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
            ' Dim t As New Thread(
            '        Sub()
            '            Dim window As New ErrorWindow
            '            window.errorString = "null reference, fag"
            '            window.faultingSource = "whoopsy doopsy"
            '            window.fullStackString = "whoopsy doopsy assasads  asadsadsad"
            '            window.ShowDialog()
            '        End Sub)
            ' t.SetApartmentState(ApartmentState.STA)
            ' t.IsBackground = True
            ' t.Start()
        End Sub

        Protected Overrides Sub OnSubModuleLoad()
            MyBase.OnSubModuleLoad()
            Dim harmony = New Harmony("xxxx.MyVBPatchProject.example")
            harmony.PatchAll()
        End Sub

        Protected Overrides Sub OnBeforeInitialModuleScreenSetAsRoot()
            MyBase.OnBeforeInitialModuleScreenSetAsRoot()
            '' Dim ver = System.Environment.Version
            '' InformationManager.ShowInquiry(New InquiryData(
            ''     "test test",
            ''     $"running on version {ver}",
            ''     True,
            ''     False,
            ''     "Accept",
            ''     "",
            ''     Sub()
            ''         'Environment.Exit(1)
            ''
            ''     End Sub,
            ''     Sub()
            ''
            ''     End Sub))
        End Sub
    End Class
End Namespace
