Imports System.IO
Imports System.Runtime.InteropServices

Public Class NativeCodeHandler
    Private Delegate Function VectoredHandler(pExceptionInfo As IntPtr) As UInteger
    Private Declare Function AddVectoredExceptionHandler Lib "kernel32.dll" (ByVal First As UInteger, ByVal Handler As VectoredHandler) As IntPtr
    Private Declare Function RemoveVectoredExceptionHandler Lib "kernel32.dll" (ByVal Handler As IntPtr) As UInteger
    Private Const EXCEPTION_ACCESS_VIOLATION As UInteger = &HC0000005UI
    Private Const EXCEPTION_CONTINUE_SEARCH = 0
    Private Const EXCEPTION_EXECUTE_HANDLER = 1
    Private Const EXCEPTION_CONTINUE_EXECUTION = -1
    Private Const REDIRECT = &HF00000FDUI

    Private Shared singleton As NativeCodeHandler = Nothing
    Private handlerDelegate As VectoredHandler

    ' Define the EXCEPTION_RECORD structure
    <StructLayout(LayoutKind.Sequential)>
    Public Structure EXCEPTION_RECORD
        Public ExceptionCode As UInteger
        Public ExceptionFlags As UInteger
        Public ExceptionRecord As IntPtr
        Public ExceptionAddress As IntPtr
        Public NumberParameters As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=15)>
        Public ExceptionInformation As UInteger()
    End Structure

    ' Define the EXCEPTION_POINTERS structure
    <StructLayout(LayoutKind.Sequential)>
    Public Structure EXCEPTION_POINTERS
        Public ExceptionRecord As IntPtr
        Public ContextRecord As IntPtr
    End Structure

    Public Shared Function Install()
        If IsNothing(singleton) Then singleton = New NativeCodeHandler()
        Return singleton
    End Function

    Private Sub New()
        InstallVectoredExceptionHandler()
        TaleWorlds.Engine.Utilities.DetachWatchdog()
        WriteGracefulHandlerIntoGameLauncher()
    End Sub

    Private Function BewVectoredHandler(ptrExceptionInfo As IntPtr) As Integer
        Dim exceptionPointers = CType(Marshal.PtrToStructure(ptrExceptionInfo, GetType(EXCEPTION_POINTERS)), EXCEPTION_POINTERS)
        Dim exceptionRecord = CType(Marshal.PtrToStructure(exceptionPointers.ExceptionRecord, GetType(EXCEPTION_RECORD)), EXCEPTION_RECORD)
        Dim exceptionCode = exceptionRecord.ExceptionCode
        If exceptionCode <> EXCEPTION_ACCESS_VIOLATION Then
            Return EXCEPTION_EXECUTE_HANDLER
        End If
        ' Capture a snapshot of the current call stack
        Dim traceString = ""
        Dim trace As New StackTrace(True)
        For Each frame As StackFrame In trace.GetFrames()
            Console.WriteLine(frame.GetMethod().Name)
            traceString = traceString & frame.GetMethod().Name & vbNewLine
        Next

        Dim result As MsgBoxResult = MsgBox("A critical has occurred from the game engine. Do you want to retry?" & vbNewLine &
                                            "Traceback:" & vbNewLine &
                                            traceString & vbNewLine &
                                            "Abort = Close the program" & vbNewLine &
                                            "Retry = Opens exception window" & vbNewLine &
                                            "Ignore = Crash the program anyway", MsgBoxStyle.AbortRetryIgnore, "Error")
        Select Case result
            Case MsgBoxResult.Abort
                KillGame()
                Return EXCEPTION_CONTINUE_SEARCH
            Case MsgBoxResult.Retry
                'exceptionRecord.ExceptionCode = REDIRECT
                'Marshal.StructureToPtr(exceptionRecord, ptrExceptionInfo, False)
                Return EXCEPTION_EXECUTE_HANDLER
            Case MsgBoxResult.Ignore
                Return EXCEPTION_CONTINUE_SEARCH
        End Select
        Return EXCEPTION_CONTINUE_SEARCH
    End Function

    Private Sub WriteGracefulHandlerIntoGameLauncher()
        Dim config = "
<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <runtime>
    <legacyCorruptedStateExceptionsPolicy enabled=""True""/>
  </runtime>
</configuration>"
        Dim fileToWrite = BaseDir & "\TaleWorlds.MountAndBlade.Launcher.exe.config"
        fileToWrite = Path.GetFullPath(fileToWrite)
        FileIO.FileSystem.WriteAllText(fileToWrite, config, False)

    End Sub

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
