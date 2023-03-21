Imports System.Runtime.InteropServices

Public Class NativeCodeHandler
    Private Delegate Function VectoredHandler(ByVal pExceptionInfo As IntPtr) As UInteger
    Private Declare Function AddVectoredExceptionHandler Lib "kernel32.dll" (ByVal First As UInteger, ByVal Handler As VectoredHandler) As IntPtr
    Private Declare Function RemoveVectoredExceptionHandler Lib "kernel32.dll" (ByVal Handler As IntPtr) As UInteger
    Private Const EXCEPTION_ACCESS_VIOLATION As UInteger = &HC0000005UI
    Private Const EXCEPTION_CONTINUE_EXECUTION = 1
    Private Const EXCEPTION_CONTINUE_SEARCH = 0
    Private Shared singleton As NativeCodeHandler = Nothing
    Private handlerDelegate As VectoredHandler

    Public Shared Function Install()
        If IsNothing(singleton) Then singleton = New NativeCodeHandler()
        Return singleton
    End Function
    Private Sub New()
        InstallVectoredExceptionHandler()
    End Sub
    Private Function BewVectoredHandler(ptrExceptionInfo As IntPtr) As UInteger
        Dim ptrRecord = Marshal.ReadIntPtr(ptrExceptionInfo)
        Dim exceptionCode = CUInt(Marshal.ReadInt32(ptrExceptionInfo))
        If exceptionCode <> EXCEPTION_ACCESS_VIOLATION Then
            Return EXCEPTION_CONTINUE_EXECUTION
        End If
        Dim result As MsgBoxResult = MsgBox("An error has occurred. Do you want to retry?", MsgBoxStyle.AbortRetryIgnore, "Error")
        Select Case result
            Case MsgBoxResult.Abort
                KillGame()
                Return EXCEPTION_CONTINUE_SEARCH
            Case MsgBoxResult.Retry
                Return EXCEPTION_CONTINUE_EXECUTION
            Case MsgBoxResult.Ignore
                Return EXCEPTION_CONTINUE_SEARCH
        End Select
        Return EXCEPTION_CONTINUE_SEARCH
    End Function

    Private Sub InstallVectoredExceptionHandler()
        GC.KeepAlive(Me)
        handlerDelegate = AddressOf BewVectoredHandler
        GC.KeepAlive(handlerDelegate)
        Dim result = AddVectoredExceptionHandler(0, handlerDelegate)
        If result = IntPtr.Zero Then
            Print("failed to install exception handler")
        End If
    End Sub
End Class
