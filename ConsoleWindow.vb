Imports System.IO
Imports System.Text

Public Module ConsoleWindow
    Dim consoleSpawned = False
    Dim loggerStarted = False
    Declare Function AllocConsole Lib "kernel32.dll" () As Boolean
    Declare Function GetStdHandle Lib "kernel32.dll" (nStdHandle As Integer) As IntPtr
    Declare Function SetStdHandle Lib "kernel32.dll" (nStdHandle As Integer, hHandle As IntPtr) As Boolean

    Dim consoleCopyInstance
    Public Sub StartLogger()
        If loggerStarted Then Exit Sub
        Exit Sub
        'FIX ME!
        Dim filename = String.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.log", DateTime.Now)
        Directory.CreateDirectory(BewBasePath & "\logs\")
        Dim logPath = BewBasePath & "\logs\" & filename
        consoleCopyInstance = New ConsoleCopy(logPath)
    End Sub
    Public Sub SpawnConsole()
        If consoleSpawned Then Exit Sub
        AllocConsole()
        Dim writer As New StreamWriter(Console.OpenStandardOutput())
        writer.AutoFlush = True
        Console.SetOut(writer)

        Console.Title = "Bannerlord stdout/stdin console. Do not close this window!"

        Trace.Listeners.Clear()

        TaleWorlds.Library.Debug.DebugManager = New DebugManager()
        loggerStarted = True
        consoleSpawned = True
    End Sub
End Module

Class ConsoleCopy
    Implements IDisposable

    Private fileStream As FileStream
    Private fileWriter As StreamWriter
    Private doubleWriterz As TextWriter
    Private oldOut As TextWriter

    Class DoubleWriter
        Inherits TextWriter

        Private one As TextWriter
        Private two As TextWriter

        Public Sub New(ByVal one As TextWriter, ByVal two As TextWriter)
            Me.one = one
            Me.two = two
        End Sub

        Public Overrides ReadOnly Property Encoding As Encoding
            Get
                Return one.Encoding
            End Get
        End Property

        Public Overrides Sub Flush()
            one.Flush()
            two.Flush()
        End Sub

        Public Overrides Sub Write(ByVal value As Char)
            one.Write(value)
            two.Write(value)
        End Sub
    End Class

    Public Sub New(ByVal path As String)
        oldOut = Console.Out

        Try
            fileStream = File.Create(path)
            fileWriter = New StreamWriter(fileStream)
            fileWriter.AutoFlush = True
            doubleWriterz = New DoubleWriter(fileWriter, oldOut)
        Catch e As Exception
            Console.WriteLine("Cannot open file for writing")
            Console.WriteLine(e.Message)
            Return
        End Try

        Console.SetOut(doubleWriterz)
    End Sub

    Public Sub Dispose()
        Console.SetOut(oldOut)

        If fileWriter IsNot Nothing Then
            fileWriter.Flush()
            fileWriter.Close()
            fileWriter = Nothing
        End If

        If fileStream IsNot Nothing Then
            fileStream.Close()
            fileStream = Nothing
        End If
    End Sub

    Private Sub IDisposable_Dispose() Implements IDisposable.Dispose
        Dispose()
    End Sub
End Class

