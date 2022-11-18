Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices
'https://www.codeguru.com/visual-basic/creating-a-memory-dump-process-in-visual-basic/
Public Module MemoryDumper
    Enum MINIDUMP_TYPE
        DumpNormal = 0
        DumpWithDataSegs = 1
        DumpWithFullMemory = 2
        DumpWithHandleData = 4
        DumpFilterMemory = 8
        DumpScanMemory = 10
        DumpWithUnloadedModules = 20
        DumpWithIndirectlyReferencedMemory = 40
        DumpFilterModulePaths = 80
        DumpWithProcessThreadData = 100
        DumpWithPrivateReadWriteMemory = 200
        DumpWithoutOptionalData = 400
        DumpWithFullMemoryInfo = 800
        DumpWithThreadInfo = 1000
        DumpWithCodeSegs = 2000
    End Enum
    Private Declare Function MiniDumpWriteDump Lib "dbghelp.dll" (ByVal hProcess As IntPtr,
                                             ByVal ProcessId As Int32,
                                             ByVal hFile As IntPtr,
                                             ByVal DumpType As MINIDUMP_TYPE,
                                             ByVal ExceptionParam As IntPtr,
                                             ByVal UserStreamParam As IntPtr,
                                             ByVal CallackParam As IntPtr) As Boolean
    Public Function DumpFile(strDumpFile As String, Optional dumpFullMemory As Boolean = False) As Boolean

        'Create An IO Stream
        Dim ioDumpFile As IO.FileStream = Nothing

        'Check Existance
        If (IO.File.Exists(strDumpFile)) Then
            ioDumpFile = IO.File.Open(strDumpFile, IO.FileMode.Append)
        Else
            ioDumpFile = IO.File.Create(strDumpFile)
        End If

        'Get Current Process
        Dim ProcToDump As Process = Process.GetCurrentProcess()

        'Get And Write Dump Info
        Dim dumptype As MINIDUMP_TYPE = IIf(dumpFullMemory, MINIDUMP_TYPE.DumpWithFullMemory, MINIDUMP_TYPE.DumpNormal)
        DumpFile = MiniDumpWriteDump(ProcToDump.Handle,
                          ProcToDump.Id,
                          ioDumpFile.SafeFileHandle.DangerousGetHandle(),
                          dumptype,
                          IntPtr.Zero,
                          IntPtr.Zero,
                          IntPtr.Zero)

        ioDumpFile.Close()
    End Function
End Module
