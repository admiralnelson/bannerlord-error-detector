Imports System.Runtime.CompilerServices
Imports TaleWorlds.Library
Imports TaleWorlds.Library.Debug

Public Class DebugManager
    Implements IDebugManager
    Dim TaleWorldsConsoleColorToWindowsColor As New Dictionary(Of DebugColor, ConsoleColor) From
        {
            {DebugColor.White, ConsoleColor.White},
            {DebugColor.Red, ConsoleColor.Red},
            {DebugColor.Blue, ConsoleColor.Blue},
            {DebugColor.Green, ConsoleColor.Green},
            {DebugColor.Yellow, ConsoleColor.Yellow},
            {DebugColor.Cyan, ConsoleColor.Cyan},
            {DebugColor.Magenta, ConsoleColor.Magenta},
            {DebugColor.Purple, ConsoleColor.Magenta},
            {DebugColor.BrightWhite, ConsoleColor.White},
            {DebugColor.DarkRed, ConsoleColor.DarkRed},
            {DebugColor.DarkBlue, ConsoleColor.DarkBlue},
            {DebugColor.DarkGreen, ConsoleColor.DarkGreen},
            {DebugColor.DarkCyan, ConsoleColor.DarkCyan},
            {DebugColor.DarkYellow, ConsoleColor.DarkYellow}
        }

    Private Sub SetColorToYellow()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Yellow
    End Sub
    Private Sub SetColorToRed()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Red
    End Sub

    Private Function GetColor(c As DebugColor) As ConsoleColor
        If TaleWorldsConsoleColorToWindowsColor.ContainsKey(c) Then Return TaleWorldsConsoleColorToWindowsColor(c)
        Return ConsoleColor.White
    End Function
    Public Sub ShowWarning(message As String) Implements IDebugManager.ShowWarning
        SetColorToYellow()
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Public Sub Assert(condition As Boolean, message As String, <CallerFilePath> Optional callerFile As String = "", <CallerMemberName> Optional callerMethod As String = "", <CallerLineNumber> Optional callerLine As Integer = 0) Implements IDebugManager.Assert
        If Not condition Then
            SetColorToRed()
            Console.Write("Assertion FAILED: ")
            Console.WriteLine(message)
            If callerFile <> "" Then Console.WriteLine("File: " & callerFile)
            If callerMethod <> "" Then Console.WriteLine("Procedure: " & callerMethod)
            If callerLine > 0 Then Console.WriteLine("At line: " & callerLine)
            Console.ResetColor()
        End If
    End Sub

    Public Sub SilentAssert(condition As Boolean, Optional message As String = "", Optional getDump As Boolean = False, <CallerFilePath> Optional callerFile As String = "", <CallerMemberName> Optional callerMethod As String = "", <CallerLineNumber> Optional callerLine As Integer = 0) Implements IDebugManager.SilentAssert
        If Not condition Then
            SetColorToYellow()
            Console.Write("Silent Assertion FAILED: ")
            Console.WriteLine(message)
            If callerFile <> "" Then Console.WriteLine("File: " & callerFile)
            If callerMethod <> "" Then Console.WriteLine("Procedure: " & callerMethod)
            If callerLine > 0 Then Console.WriteLine("At line: " & callerLine)
            Console.ResetColor()
        End If
    End Sub

    Public Sub Print(message As String, Optional logLevel As Integer = 0, Optional color As Debug.DebugColor = Debug.DebugColor.White, Optional debugFilter As ULong = 17592186044416) Implements IDebugManager.Print
        Console.ForegroundColor = GetColor(color)
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Public Sub PrintError([error] As String, stackTrace As String, Optional debugFilter As ULong = 17592186044416) Implements IDebugManager.PrintError
        SetColorToRed()
        Console.WriteLine("ERROR " & [error])
        Console.WriteLine("Traceback " & stackTrace)
        Console.ResetColor()
    End Sub

    Public Sub PrintWarning(warning As String, Optional debugFilter As ULong = 17592186044416) Implements IDebugManager.PrintWarning
        SetColorToYellow()
        Console.WriteLine("WARNI " & warning)
        Console.ResetColor()
    End Sub

    Public Sub ShowError(message As String) Implements IDebugManager.ShowError
        SetColorToRed()
        Console.WriteLine("ERRO " & message)
        Console.ResetColor()
    End Sub

    Public Sub ShowMessageBox(lpText As String, lpCaption As String, uType As UInteger) Implements IDebugManager.ShowMessageBox
        MsgBox(lpText, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, lpCaption)
    End Sub

    Public Sub DisplayDebugMessage(message As String) Implements IDebugManager.DisplayDebugMessage
        Print(message)
    End Sub

    Public Sub WatchVariable(name As String, value As Object) Implements IDebugManager.WatchVariable
        Console.WriteLine("Variable ", name, " = ", value)
    End Sub

    Public Sub WriteDebugLineOnScreen(message As String) Implements IDebugManager.WriteDebugLineOnScreen
        Util.Print(message)
    End Sub

    Public Sub RenderDebugLine(position As Vec3, direction As Vec3, Optional color As UInteger = UInteger.MaxValue, Optional depthCheck As Boolean = False, Optional time As Single = 0) Implements IDebugManager.RenderDebugLine

    End Sub

    Public Sub RenderDebugSphere(position As Vec3, radius As Single, Optional color As UInteger = UInteger.MaxValue, Optional depthCheck As Boolean = False, Optional time As Single = 0) Implements IDebugManager.RenderDebugSphere

    End Sub

    Public Sub RenderDebugText3D(position As Vec3, text As String, Optional color As UInteger = UInteger.MaxValue, Optional screenPosOffsetX As Integer = 0, Optional screenPosOffsetY As Integer = 0, Optional time As Single = 0) Implements IDebugManager.RenderDebugText3D

    End Sub

    Public Sub RenderDebugFrame(frame As MatrixFrame, lineLength As Single, Optional time As Single = 0) Implements IDebugManager.RenderDebugFrame

    End Sub

    Public Sub RenderDebugText(screenX As Single, screenY As Single, text As String, Optional color As UInteger = UInteger.MaxValue, Optional time As Single = 0) Implements IDebugManager.RenderDebugText

    End Sub

    Public Sub RenderDebugRectWithColor(left As Single, bottom As Single, right As Single, top As Single, Optional color As UInteger = UInteger.MaxValue) Implements IDebugManager.RenderDebugRectWithColor

    End Sub

    Public Sub SetCrashReportCustomString(customString As String) Implements IDebugManager.SetCrashReportCustomString

    End Sub

    Public Sub SetCrashReportCustomStack(customStack As String) Implements IDebugManager.SetCrashReportCustomStack

    End Sub

    Public Sub SetTestModeEnabled(testModeEnabled As Boolean) Implements IDebugManager.SetTestModeEnabled

    End Sub

    Public Sub AbortGame() Implements IDebugManager.AbortGame
        Util.KillGame()
    End Sub

    Public Sub DoDelayedexit(returnCode As Integer) Implements IDebugManager.DoDelayedexit

    End Sub

    Public Function GetDebugVector() As Vec3 Implements IDebugManager.GetDebugVector
        Return New Vec3(0, 0, 0)
    End Function

    Public Sub ReportMemoryBookmark(message As String) Implements IDebugManager.ReportMemoryBookmark

    End Sub
End Class
